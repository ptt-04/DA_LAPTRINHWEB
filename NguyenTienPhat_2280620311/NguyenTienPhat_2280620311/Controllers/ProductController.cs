using Microsoft.AspNetCore.Mvc;
using NguyenTienPhat_2280620311.Models;
using NguyenTienPhat_2280620311.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace NguyenTienPhat_2280620311.Controllers
{
    [Authorize(Roles = SD.Role_Customer)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index(string searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => 
                    p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                    p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            // Lọc theo khoảng giá
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            ViewBag.Categories = categories;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(products.ToList());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            // Lấy danh sách mã giảm giá còn hiệu lực, còn số lượng, đang hoạt động
            var now = DateTime.Now;
            var coupons = await _context.Coupons
                .Where(c => c.IsActive && c.Quantity > 0 && c.StartDate <= now && c.EndDate >= now)
                .ToListAsync();
            ViewBag.Coupons = coupons;
            return View(product);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Suggest(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(new List<object>());
            var products = await _productRepository.SearchByNameAsync(q, 5);
            var result = products.Select(p => new {
                id = p.Id,
                name = p.Name,
                image = p.ImageUrl,
                price = p.Price,
                isInStock = p.IsInStock
            });
            return Json(result);
        }
    }
} 