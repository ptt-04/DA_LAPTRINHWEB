using Microsoft.AspNetCore.Mvc;
using NguyenTienPhat_2280620311.Models;
using NguyenTienPhat_2280620311.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace NguyenTienPhat_2280620311.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;

        public HomeController(IProductRepository productRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        // Hi?n th? danh s�ch s?n ph?m
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();

            // Get top 5 selling books
            var topSellingBooks = await _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => od.Product)
                .Select(g => new
                {
                    g.Key.Id,
                    g.Key.Name,
                    g.Key.ImageUrl,
                    g.Key.Price,
                    SoldCount = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(5)
                .ToListAsync();

            // Get top 5 rated books
            var topRatedBooks = await _context.Products
                .Include(p => p.Reviews)
                .Where(p => p.Reviews.Any())
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.ImageUrl,
                    p.Price,
                    Rating = p.Reviews.Average(r => r.Rating)
                })
                .OrderByDescending(p => p.Rating)
                .Take(5)
                .ToListAsync();

            ViewBag.TopSellingBooks = topSellingBooks;
            ViewBag.TopRatedBooks = topRatedBooks;

            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}