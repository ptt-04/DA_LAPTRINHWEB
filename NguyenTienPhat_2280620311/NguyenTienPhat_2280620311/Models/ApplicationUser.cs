using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NguyenTienPhat_2280620311.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }
    }
}
