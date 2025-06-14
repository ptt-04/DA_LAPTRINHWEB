using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NguyenTienPhat_2280620311.Models;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace NguyenTienPhat_2280620311.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<dynamic>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FullName,
                    user.PhoneNumber,
                    Role = string.Join(", ", roles),
                    Status = user.LockoutEnd != null && user.LockoutEnd > DateTime.Now ? "Locked" : "Active"
                });
            }
            ViewBag.Users = userList;
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Role = string.Join(", ", roles);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string userName, string email, string phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            user.UserName = userName;
            user.Email = email;
            user.PhoneNumber = phoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Role = string.Join(", ", roles);
            return View(user);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Role = string.Join(", ", roles);
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Đã xóa tài khoản thành công!";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Xóa tài khoản thất bại!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var allRoles = new List<string> { "Admin", "Customer" };
            var userRoles = await _userManager.GetRolesAsync(user);
            ViewBag.AllRoles = allRoles;
            ViewBag.UserRoles = userRoles;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);
            TempData["Success"] = "Cập nhật vai trò thành công!";
            return RedirectToAction("Index");
        }
    }
} 