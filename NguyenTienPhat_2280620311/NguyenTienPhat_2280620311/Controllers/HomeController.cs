using Microsoft.AspNetCore.Mvc;
using NguyenTienPhat_2280620311.Models;
using NguyenTienPhat_2280620311.Repositories;
using System.Diagnostics;

namespace NguyenTienPhat_2280620311.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;

        public HomeController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Hi?n th? danh sách s?n ph?m
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
    }
}