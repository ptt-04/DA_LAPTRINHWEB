using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NguyenTienPhat_2280620311.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NguyenTienPhat_2280620311.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var logins = await _userManager.GetLoginsAsync(user);
            ViewBag.IsGoogleUser = logins.Any(l => l.LoginProvider == "Google");
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            if (ModelState.IsValid)
            {
                user.FullName = model.FullName;
                user.Address = model.Address;
                user.Age = model.Age;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Index");
            }
            return View(user);
        }
    }
} 