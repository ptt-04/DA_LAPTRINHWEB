using NguyenTienPhat_2280620311.Models.Vnpay;

namespace NguyenTienPhat_2280620311.Vnpay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
