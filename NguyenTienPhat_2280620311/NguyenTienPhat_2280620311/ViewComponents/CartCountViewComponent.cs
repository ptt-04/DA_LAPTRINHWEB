using Microsoft.AspNetCore.Mvc;
using NguyenTienPhat_2280620311.Extensions;
using NguyenTienPhat_2280620311.Models;

namespace NguyenTienPhat_2280620311.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            int itemCount = cart?.Items.Sum(i => i.Quantity) ?? 0;
            return View(itemCount);
        }
    }
} 