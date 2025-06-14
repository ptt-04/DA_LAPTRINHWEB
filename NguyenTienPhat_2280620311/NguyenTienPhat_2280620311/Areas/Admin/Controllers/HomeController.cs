using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NguyenTienPhat_2280620311.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace NguyenTienPhat_2280620311.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var recentProducts = _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .Take(5)
                .ToList();
            ViewBag.RecentProducts = recentProducts;
            return View();
        }
    }
} 