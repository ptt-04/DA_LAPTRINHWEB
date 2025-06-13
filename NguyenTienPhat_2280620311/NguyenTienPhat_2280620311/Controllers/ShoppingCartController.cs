using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NguyenTienPhat_2280620311.Extensions;
using NguyenTienPhat_2280620311.Models;
using NguyenTienPhat_2280620311.Repositories;
using NguyenTienPhat_2280620311.Services;

namespace NguyenTienPhat_2280620311.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cartService;

        public ShoppingCartController(
            IProductRepository productRepository,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ICartService cartService)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            
            var cart = new ShoppingCart();
            foreach (var item in cartItems)
            {
                cart.AddItem(new CartItem
                {
                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    Quantity = item.Quantity,
                    ImageUrl = item.Product.ImageUrl
                });
            }
            
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int productId)
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            var item = cartItems.FirstOrDefault(i => i.ProductId == productId);
            
            if (item != null)
            {
                await _cartService.UpdateQuantityAsync(userId, productId, item.Quantity + 1);
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int productId)
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            var item = cartItems.FirstOrDefault(i => i.ProductId == productId);
            
            if (item != null && item.Quantity > 1)
            {
                await _cartService.UpdateQuantityAsync(userId, productId, item.Quantity - 1);
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            await _cartService.RemoveFromCartAsync(userId, productId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            await _cartService.AddToCartAsync(userId, productId, quantity);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            var order = new Order();
            // Gán tạm danh sách sản phẩm vào ViewBag để View dùng
            ViewBag.CartItems = cartItems;
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            
            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }

            order.UserId = userId;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cartItems.Sum(i => i.Product.Price * i.Quantity);
            order.OrderDetails = cartItems.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Product.Price
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            // Clear cart after successful order
            await _cartService.ClearCartAsync(userId);

            return View("OrderCompleted", order.Id);
        }

        private async Task<Product> GetProductFromDatabase(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }
    }
}