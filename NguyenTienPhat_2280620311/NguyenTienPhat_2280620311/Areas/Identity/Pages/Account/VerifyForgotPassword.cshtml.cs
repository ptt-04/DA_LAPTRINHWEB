using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace NguyenTienPhat_2280620311.Areas.Identity.Pages.Account
{
    public class VerifyForgotPasswordModel : PageModel
    {
        [BindProperty]
        [Required]
        public string VerifyCode { get; set; }
        public void OnGet() { }
        public IActionResult OnPost()
        {
            var code = TempData["ForgotPass_Code"] as string;
            if (VerifyCode != code)
            {
                TempData["Error"] = "Mã xác nhận không đúng!";
                TempData.Keep();
                return Page();
            }
            TempData.Keep();
            return RedirectToPage("ResetPassword");
        }
    }
} 