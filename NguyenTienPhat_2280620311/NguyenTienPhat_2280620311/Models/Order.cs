using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenTienPhat_2280620311.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        public string ShippingAddress { get; set; }
        public string Notes { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.ChoXuLy;
    }

    public enum OrderStatus
    {
        ChoXuLy, // Chờ xử lí
        DangXuLy, // Đang xử lí
        HoanThanh // Hoàn thành
    }
}
