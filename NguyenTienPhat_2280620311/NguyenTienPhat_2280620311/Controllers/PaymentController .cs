using Microsoft.AspNetCore.Mvc;
using NguyenTienPhat_2280620311.Models.Vnpay;
using NguyenTienPhat_2280620311.Vnpay;
using NguyenTienPhat_2280620311.Extensions;
using NguyenTienPhat_2280620311.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using NguyenTienPhat_2280620311.Services;
using Microsoft.EntityFrameworkCore;

namespace NguyenTienPhat_2280620311.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cartService;

        public PaymentController(
            IVnPayService vnPayService, 
            ApplicationDbContext context,
            ILogger<PaymentController> logger,
            UserManager<ApplicationUser> userManager,
            ICartService cartService)
        {
            _vnPayService = vnPayService;
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrlVnpay(Order order, string couponCode, decimal discountAmount = 0)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                if (cartItems == null || !cartItems.Any())
                {
                    return RedirectToAction("Index", "ShoppingCart");
                }

                var totalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity);
                decimal originalTotal = totalAmount;

                // Xử lý mã giảm giá nếu có
                if (!string.IsNullOrEmpty(couponCode) && discountAmount > 0)
                {
                    var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == couponCode);
                    if (coupon != null)
                    {
                        if (coupon.IsActive &&
                            coupon.StartDate <= DateTime.Now &&
                            coupon.EndDate >= DateTime.Now &&
                            coupon.UsedCount < coupon.Quantity &&
                            originalTotal >= coupon.MinimumSpend)
                        {
                            decimal calculatedDiscount;
                            if (coupon.DiscountType == DiscountType.FixedAmount)
                            {
                                calculatedDiscount = coupon.DiscountAmount;
                            }
                            else
                            {
                                calculatedDiscount = (originalTotal * coupon.DiscountAmount) / 100;
                            }
                            if (Math.Abs(calculatedDiscount - discountAmount) < 1)
                            {
                                // Chỉ cập nhật UsedCount khi callback thành công
                                totalAmount = originalTotal - discountAmount;
                                // Lưu couponCode và discountAmount vào session để callback xử lý UsedCount
                                HttpContext.Session.SetString("CouponCode", couponCode);
                                HttpContext.Session.SetString("DiscountAmount", discountAmount.ToString());
                            }
                            else
                            {
                                discountAmount = 0;
                                totalAmount = originalTotal;
                                HttpContext.Session.Remove("CouponCode");
                                HttpContext.Session.Remove("DiscountAmount");
                            }
                        }
                    }
                }
                else
                {
                    HttpContext.Session.Remove("CouponCode");
                    HttpContext.Session.Remove("DiscountAmount");
                }

                // Gán thông tin sản phẩm vào order
                order.UserId = userId;
                order.OrderDate = DateTime.UtcNow;
                order.TotalPrice = totalAmount;
                order.OrderDetails = cartItems.Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Product.Price
                }).ToList();

                // Tạo thông tin thanh toán cho VNPay
                var paymentInfo = new PaymentInformationModel
                {
                    OrderType = "billpayment",
                    Amount = (double)totalAmount,
                    OrderDescription = $"Thanh toan don hang {DateTime.Now.Ticks}",
                    Name = User.Identity?.Name ?? "Khach hang"
                };

                _logger.LogInformation("Bắt đầu tạo URL thanh toán VNPay với thông tin: {@PaymentInfo}", paymentInfo);

                // Gọi API để lấy URL thanh toán VNPay
                var url = _vnPayService.CreatePaymentUrl(paymentInfo, HttpContext);

                _logger.LogInformation("Đã tạo URL thanh toán VNPay: {Url}", url);

                // Lưu thông tin đơn hàng vào Session để xử lý sau khi thanh toán
                HttpContext.Session.SetObjectAsJson("PendingOrder", order);

                // Chuyển hướng đến trang thanh toán VNPay
                _logger.LogInformation("Chuyển hướng người dùng đến trang thanh toán VNPay");
                return Redirect(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo URL thanh toán VNPay");
                TempData["PaymentError"] = "Có lỗi xảy ra khi tạo thanh toán. Vui lòng thử lại.";
                return RedirectToAction("Checkout", "ShoppingCart");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                _logger.LogInformation("Nhận callback từ VNPay với query string: {Query}", Request.QueryString.Value);
                var response = _vnPayService.PaymentExecute(Request.Query);
                _logger.LogInformation("Kết quả xử lý callback VNPay: {@Response}", response);
            
                if (response.Success)
                {
                    // Lấy thông tin đơn hàng từ Session
                    var order = HttpContext.Session.GetObjectFromJson<Order>("PendingOrder");
                    var user = await _userManager.GetUserAsync(User);
                    
                    if (user == null)
                    {
                        throw new Exception("Không tìm thấy thông tin người dùng");
                    }

                    // Lấy giỏ hàng từ database
                    var cartItems = await _cartService.GetCartItemsAsync(user.Id);
                    var totalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity);

                    if (order == null)
                    {
                        // Nếu không tìm thấy đơn hàng trong session, vẫn hiển thị thanh toán thành công
                        var paymentDetail = new PaymentDetailModel
                        {
                            Amount = (double)totalAmount,
                            PaymentStatus = "Thành công",
                            PaymentTime = DateTime.Now
                        };
                        return View("PaymentSuccess", paymentDetail);
                    }

                    try
                    {
                        // Cập nhật thông tin đơn hàng
                        order.UserId = user.Id;
                        order.OrderDate = DateTime.UtcNow;
                        // Lấy discountAmount và couponCode từ session nếu có
                        var discountAmountStr = HttpContext.Session.GetString("DiscountAmount");
                        var couponCode = HttpContext.Session.GetString("CouponCode");
                        decimal discountAmount = 0;
                        if (!string.IsNullOrEmpty(discountAmountStr))
                        {
                            decimal.TryParse(discountAmountStr, out discountAmount);
                        }
                        order.TotalPrice = totalAmount - discountAmount;
                        // Đảm bảo các trường bắt buộc không null
                        if (string.IsNullOrEmpty(order.ShippingAddress))
                        {
                            order.ShippingAddress = "Địa chỉ chưa cập nhật";
                        }
                        if (string.IsNullOrEmpty(order.Notes))
                        {
                            order.Notes = "Không có ghi chú";
                        }

                        // Cập nhật chi tiết đơn hàng từ giỏ hàng trong database
                        order.OrderDetails = cartItems.Select(i => new OrderDetail
                        {
                            ProductId = i.ProductId,
                            Quantity = i.Quantity,
                            Price = i.Product.Price
                        }).ToList();

                        // Nếu có mã giảm giá, cập nhật UsedCount
                        if (!string.IsNullOrEmpty(couponCode) && discountAmount > 0)
                        {
                            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == couponCode);
                            if (coupon != null && coupon.IsActive && coupon.UsedCount < coupon.Quantity)
                            {
                                coupon.UsedCount++;
                                coupon.Quantity--;
                                _context.Coupons.Update(coupon);
                            }
                        }

                        // Lưu đơn hàng vào database
                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Đã lưu đơn hàng thành công: {OrderId}", order.Id);

                        // Xóa giỏ hàng sau khi đặt hàng thành công
                        await _cartService.ClearCartAsync(user.Id);

                        // Xóa thông tin đơn hàng khỏi Session
                        HttpContext.Session.Remove("PendingOrder");
                        HttpContext.Session.Remove("CouponCode");
                        HttpContext.Session.Remove("DiscountAmount");

                        // Tạo model cho view thanh toán thành công
                        var paymentDetail = new PaymentDetailModel
                        {
                            OrderId = order.Id.ToString(),
                            TransactionId = response.TransactionId,
                            PaymentMethod = response.PaymentMethod,
                            Amount = (double)order.TotalPrice,
                            OrderDescription = response.OrderDescription,
                            CustomerName = user.UserName,
                            PaymentTime = DateTime.Now,
                            PaymentStatus = "Thành công"
                        };

                        return View("PaymentSuccess", paymentDetail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi xử lý đơn hàng sau thanh toán");
                        // Vẫn hiển thị thanh toán thành công nhưng ghi log lỗi
                        var paymentDetail = new PaymentDetailModel
                        {
                            Amount = (double)totalAmount,
                            PaymentStatus = "Thành công",
                            PaymentTime = DateTime.Now
                        };
                        return View("PaymentSuccess", paymentDetail);
                    }
                }
                
                // Chỉ khi VNPay trả về không thành công
                TempData["PaymentError"] = "Thanh toán không thành công. Vui lòng thử lại.";
                return RedirectToAction("Checkout", "ShoppingCart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý callback từ VNPay");
                TempData["PaymentError"] = "Có lỗi xảy ra trong quá trình xử lý thanh toán. Vui lòng thử lại.";
                return RedirectToAction("Checkout", "ShoppingCart");
            }
        }
    }
}
