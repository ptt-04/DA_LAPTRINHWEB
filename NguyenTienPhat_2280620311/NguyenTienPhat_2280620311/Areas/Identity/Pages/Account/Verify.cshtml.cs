using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenTienPhat_2280620311.Models;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace NguyenTienPhat_2280620311.Areas.Identity.Pages.Account
{
    public class VerifyModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IEmailSender _emailSender;

        public VerifyModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IUserStore<ApplicationUser> userStore, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailSender = emailSender;
        }

        [BindProperty]
        [Required]
        public string VerifyCode { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var code = TempData["Register_Code"] as string;
            if (VerifyCode != code)
            {
                TempData["Error"] = "Mã xác nhận không đúng!";
                TempData.Keep();
                return Page();
            }
            // Lấy thông tin đăng ký từ TempData
            var fullName = TempData["Register_FullName"] as string;
            var userName = TempData["Register_UserName"] as string;
            var email = TempData["Register_Email"] as string;
            var password = TempData["Register_Password"] as string;
            var role = TempData["Register_Role"] as string;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                TempData["Error"] = "Thiếu thông tin đăng ký!";
                return Page();
            }
            // Kiểm tra username đã tồn tại chưa
            var existedUserByUsername = await _userManager.FindByNameAsync(userName);
            if (existedUserByUsername != null)
            {
                TempData["Error"] = "Tên đăng nhập đã tồn tại!";
                return Page();
            }
            // Kiểm tra email đã tồn tại chưa
            var existedUser = await _userManager.FindByEmailAsync(email);
            if (existedUser != null)
            {
                TempData["Error"] = "Tài khoản đã tồn tại!";
                return Page();
            }
            var user = new ApplicationUser { FullName = fullName, UserName = userName, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("/Index", new { area = "" });
            }
            TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
            return Page();
        }
    }
} 