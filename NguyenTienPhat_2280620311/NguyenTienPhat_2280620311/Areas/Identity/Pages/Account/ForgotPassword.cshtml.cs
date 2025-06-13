using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenTienPhat_2280620311.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NguyenTienPhat_2280620311.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        [BindProperty]
        [Required]
        public string UserNameOrEmail { get; set; }
        public void OnGet() { }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            ApplicationUser user = null;
            if (UserNameOrEmail.Contains("@"))
                user = await _userManager.FindByEmailAsync(UserNameOrEmail);
            else
                user = await _userManager.FindByNameAsync(UserNameOrEmail);
            if (user == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại!";
                return Page();
            }
            var code = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("ForgotPass_UserId", user.Id);
            TempData["ForgotPass_Code"] = code;
            TempData["ForgotPass_Email"] = user.Email;
            TempData.Keep();
            await _emailSender.SendEmailAsync(user.Email, "Mã xác nhận đặt lại mật khẩu", $"Mã xác nhận của bạn là: <b>{code}</b>");
            TempData["Success"] = "Đã gửi mã xác nhận về email!";
            return RedirectToPage("VerifyForgotPassword");
        }
    }
} 