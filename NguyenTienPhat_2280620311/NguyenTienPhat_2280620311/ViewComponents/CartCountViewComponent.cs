using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NguyenTienPhat_2280620311.Models;
using NguyenTienPhat_2280620311.Services;

namespace NguyenTienPhat_2280620311.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartCountViewComponent(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int cartCount = 0;

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId((System.Security.Claims.ClaimsPrincipal)User);
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                cartCount = cartItems.Sum(x => x.Quantity);
            }

            return View(cartCount);
        }
    }
} 