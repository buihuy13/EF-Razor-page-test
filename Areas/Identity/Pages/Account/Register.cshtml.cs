// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using EFRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace EFRazor.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<appUser> _signInManager;
        private readonly UserManager<appUser> _userManager;
        private readonly IUserStore<appUser> _userStore;
        private readonly IUserEmailStore<appUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        //Inject những địch vụ cần dùng
        public RegisterModel(
            UserManager<appUser> userManager,
            IUserStore<appUser> userStore,
            SignInManager<appUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        
        //Đường dẫn trả về
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        

        //List các dịch vụ bên ngoài có thể được sử dụng để đăng ký
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        
        //Những thuộc tính của InputModel cần dược submit lên để bind 
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DisplayName("Tên tài khoản")]
            [StringLength(50,MinimumLength =5)]
            public string UserName { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            foreach (var provider in ExternalLogins)
            {
                _logger.LogInformation(provider.Name);
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //Nếu returnUrl = null thì gán giá trị cho returnUrl, không thì thôi
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            //Kiểm tra các thông tin được post lên đã valid hay chưa
            if (ModelState.IsValid)
            {

                //Phương thức để tạo ra 1 user mới (Phương thức ở dưới)
                var user = CreateUser();

                //Kho chứa user sẽ đặt tên cho user
                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                //Đặt email cho user
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                //Tạo ra user với đối tượng user và mật khẩu
                var result = await _userManager.CreateAsync(user, Input.Password);

                //Nếu như tạo ra 1 đối tượng user thành công (appUser)
                if (result.Succeeded)
                {
                    //Thông báo tạo user thành công
                    _logger.LogInformation("User created a new account with password.");

                    //Đoạn mã dưới dây là để sinh ra 1 đường link confirm email cho người dùng sau khi đăng ký thành công

                    //Lấy userId
                    var userId = await _userManager.GetUserIdAsync(user);

                    //Tạo ra 1 mã Token để confirm email (mỗi mã token là riêng cho 1 người dùng) và được lưu dưới biến code
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //Mã token sẽ được encode để có thể đính kèm trên url
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    //Tạo ra đường url để gọi đến trang confirm email
                    //Đường dẫn sẽ là: https://localhost:7157/Account/ConfirmEmail?userId=xxx&code=xyz&returnUrl=abc
                    var callbackUrl = Url.Page(
                        //Url gốc (thêm area = Identity để biết Account/ConfirmEmail nằm trong folder Identity)
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        //Tạo ra handler dưới biến values
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //Gọi đến phương thức SendMailAsync đã được đăng ký vào service ở bài trước
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //Kiểm tra xem services có thiết lập yêu cầu xác nhận tài khoản hay không
                    //Nếu có thì chuyển hướng tới trang yêu cầu xác nhân tài khoản
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        //Điều hướng qua trang RegisterConfirmation
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    //Nếu không thì cho phép đăng nhập vào thẳng luôn
                    else
                    {
                        //isPersistent đang thiết lập = false, nếu thiết lập = true thì sẽ tạo ra cookie để nhớ thông tin lần sau đăng nhập
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                //Nếu như tạo tài khoản không thành công thì sẽ thiết lập hiển thị tất cả các lỗi lên trang
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private appUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<appUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(appUser)}'. " +
                    $"Ensure that '{nameof(appUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<appUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<appUser>)_userStore;
        }
    }
}
