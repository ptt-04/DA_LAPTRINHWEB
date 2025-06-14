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
            try
            {
                // Lấy ngày hiện tại
                var today = DateTime.Today;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                var lastSevenDays = Enumerable.Range(0, 7)
                    .Select(i => today.AddDays(-i))
                    .Reverse()
                    .ToList();

                // Thống kê doanh thu hôm nay
                var todayRevenue = _context.Orders
                    .Where(o => o.OrderDate.Date == today)
                    .Sum(o => o.TotalPrice);

                // Thống kê doanh thu tháng này
                var thisMonthRevenue = _context.Orders
                    .Where(o => o.OrderDate >= firstDayOfMonth && o.OrderDate <= today)
                    .Sum(o => o.TotalPrice);

                // Số đơn hàng hôm nay
                var todayOrders = _context.Orders
                    .Count(o => o.OrderDate.Date == today);

                // Số sách đã bán hôm nay
                var todayBooksSold = _context.OrderDetails
                    .Include(od => od.Order)
                    .Where(od => od.Order.OrderDate.Date == today)
                    .Sum(od => od.Quantity);

                // Doanh thu 7 ngày gần đây
                var dailyRevenue = lastSevenDays
                    .Select(date => _context.Orders
                        .Where(o => o.OrderDate.Date == date)
                        .Sum(o => (decimal?)o.TotalPrice) ?? 0m)
                    .ToList();

                // Doanh thu theo tháng trong năm nay
                var monthlyRevenue = Enumerable.Range(1, 12)
                    .Select(month => _context.Orders
                        .Where(o => o.OrderDate.Year == today.Year && o.OrderDate.Month == month)
                        .Sum(o => (decimal?)o.TotalPrice) ?? 0m)
                    .ToList();

                // Top sách nổi bật (lấy từ bảng Product và tính trung bình Rating từ Reviews)
                var topSellingBooks = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                    .Select(p => new
                    {
                        Name = p.Name,
                        Price = p.Price,
                        Category = p.Category.Name,
                        Rating = p.Reviews.Any() ? Math.Round(p.Reviews.Average(r => r.Rating), 1) : 0,
                        ReviewCount = p.Reviews.Count
                    })
                    .OrderByDescending(p => p.Rating)
                    .Take(5)
                    .ToList();

                // Truyền dữ liệu cho view
                ViewBag.TodayRevenue = todayRevenue;
                ViewBag.ThisMonthRevenue = thisMonthRevenue;
                ViewBag.TodayOrders = todayOrders;
                ViewBag.TodayBooksSold = todayBooksSold;
                ViewBag.DailyRevenue = dailyRevenue;
                ViewBag.MonthlyRevenue = monthlyRevenue;
                ViewBag.LastSevenDays = lastSevenDays.Select(d => d.ToString("dd/MM")).ToList();
                ViewBag.TopSellingBooks = topSellingBooks;

                return View();
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Error in HomeController.Index: {ex.Message}");
                // Trả về dữ liệu mặc định
                ViewBag.TodayRevenue = 0;
                ViewBag.ThisMonthRevenue = 0;
                ViewBag.TodayOrders = 0;
                ViewBag.TodayBooksSold = 0;
                ViewBag.DailyRevenue = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
                ViewBag.MonthlyRevenue = new List<decimal> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                ViewBag.LastSevenDays = Enumerable.Range(0, 7)
                    .Select(i => DateTime.Today.AddDays(-i))
                    .Reverse()
                    .Select(d => d.ToString("dd/MM"))
                    .ToList();
                ViewBag.TopSellingBooks = new List<object>();
                return View();
            }
        }
    }
} 