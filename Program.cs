using EFRazor.Models;
using EFRazor.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Security.Requirements;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Thêm vào DbContext BlogContext vào dịch vụ
builder.Services.AddDbContext<BlogContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("BlogContext");
    options.UseMySQL(connectionString);
});

//Kích hoạt sử dụng Options
builder.Services.AddOptions();
//Đăng ký dịch vụ cho MailSettings
var mailSettingsOption = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettingsOption);
builder.Services.AddSingleton<IEmailSender,SendMailService>();

//Đăng ký dịch vụ cho Identity
builder.Services.AddIdentity<appUser, IdentityRole>()
                .AddEntityFrameworkStores<BlogContext>()
                .AddDefaultTokenProviders();

//Đăng ký ủy quyền truy cập khi vào 1 trang
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự cho phép trong tên của user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất và chỉ được sử dụng 1 lần

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;     //Xác thực tài khoản sau khi đăng ký
});

builder.Services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    var gConfig = builder.Configuration.GetSection("Authentication:Google");
                    googleOptions.ClientId = gConfig["ClientId"];
                    googleOptions.ClientSecret = gConfig["ClientSecret"];
                    // default https://localhost:7157/signin-google
                    googleOptions.CallbackPath = "/dang-nhap-tu-google";
                });

builder.Services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();
//Thêm vào các policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowEditRole", policyBuilder =>
    {
        //Điều kiện của chính sách
        //Yêu cầu Role nào đó
        policyBuilder.RequireRole("Admin");
        policyBuilder.RequireRole("Editor");
        //Phải đăng nhập
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireClaim("canedit", new string[]
        {
            "user",
            "post"
        });
    });

    options.AddPolicy("InGenZ", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.Requirements.Add(new GenZRequirement());
    });

    options.AddPolicy("ShowAdminMenu", policyBuilder =>
    {
        policyBuilder.RequireRole("Admin");
    });

    options.AddPolicy("CanUpdate", policyBuilder =>
    {
        policyBuilder.Requirements.Add(new CanUpdateRequirement());
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

