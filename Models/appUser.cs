using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFRazor.Models
{
    public class appUser : IdentityUser
    {
        [StringLength(100)]
        public string? HomeAddress { get; set; }
    }
}
