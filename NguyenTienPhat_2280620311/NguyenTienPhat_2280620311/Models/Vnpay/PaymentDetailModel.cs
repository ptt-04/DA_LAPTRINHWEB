namespace NguyenTienPhat_2280620311.Models.Vnpay
{
    public class PaymentDetailModel
    {
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string CustomerName { get; set; }
        public DateTime PaymentTime { get; set; }
        public string PaymentStatus { get; set; }
    }
} 