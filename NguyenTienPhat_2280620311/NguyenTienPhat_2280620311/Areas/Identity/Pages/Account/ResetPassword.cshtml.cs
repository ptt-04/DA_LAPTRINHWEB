using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenTienPhat_2280620311.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NguyenTienPhat_2280620311.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        public string ConfirmPassword { get; set; }
        public void OnGet() { }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var userId = HttpContext.Session.GetString("ForgotPass_UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Phiên xác thực đã hết hạn!";
                return Page();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại!";
                return Page();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập với mật khẩu mới.";
                HttpContext.Session.Remove("ForgotPass_UserId");
                return RedirectToPage("Login");
            }
            TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
            return Page();
        }
    }
} 