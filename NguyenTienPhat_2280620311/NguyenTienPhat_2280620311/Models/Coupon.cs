using System;
using System.ComponentModel.DataAnnotations;

namespace NguyenTienPhat_2280620311.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        [Display(Name = "Mã giảm giá")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên mã giảm giá")]
        [Display(Name = "Tên mã giảm giá")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Display(Name = "Số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm")]
        [Display(Name = "Giá trị giảm")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn 0")]
        public decimal DiscountAmount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại giảm giá")]
        [Display(Name = "Loại giảm giá")]
        public DiscountType DiscountType { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá trị đơn hàng tối thiểu")]
        [Display(Name = "Giá trị đơn hàng tối thiểu")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị đơn hàng tối thiểu không được âm")]
        public decimal MinimumSpend { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Số lần đã sử dụng")]
        public int UsedCount { get; set; } = 0;
    }

    public enum DiscountType
    {
        [Display(Name = "Giảm theo số tiền")]
        FixedAmount,
        [Display(Name = "Giảm theo phần trăm")]
        Percentage
    }
} 