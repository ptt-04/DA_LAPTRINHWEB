using Microsoft.AspNetCore.Mvc;
using NguyenTienPhat_2280620311.Models.Vnpay;
using NguyenTienPhat_2280620311.Vnpay;

namespace NguyenTienPhat_2280620311.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IConfiguration _configuration;

        public CheckoutController(IVnPayService vnPayService, IConfiguration configuration)
        {
            _vnPayService = vnPayService;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Payment(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            var paymentDetail = new PaymentDetailModel
            {
                OrderId = response.OrderId,
                TransactionId = response.TransactionId,
                PaymentMethod = response.PaymentMethod,
                OrderDescription = response.OrderDescription,
                PaymentTime = DateTime.Now,
                PaymentStatus = response.Success ? "Thành công" : "Thất bại"
            };

            // Lấy thông tin từ Session hoặc Cache nếu cần
            // paymentDetail.CustomerName = HttpContext.Session.GetString("CustomerName");
            // paymentDetail.Amount = Convert.ToDouble(HttpContext.Session.GetString("Amount"));

            // Tạm thời gán giá trị mẫu
            paymentDetail.CustomerName = "Nguyễn Văn A";
            paymentDetail.Amount = 1000000;

            return View("PaymentDetail", paymentDetail);
        }
    }
}
