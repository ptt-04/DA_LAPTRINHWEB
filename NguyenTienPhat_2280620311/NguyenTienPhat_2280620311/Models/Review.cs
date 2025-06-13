using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenTienPhat_2280620311.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn số sao đánh giá")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        [Display(Name = "Đánh giá sao")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung đánh giá")]
        [MinLength(10, ErrorMessage = "Nội dung đánh giá phải có ít nhất 10 ký tự")]
        [MaxLength(1000, ErrorMessage = "Nội dung đánh giá không được vượt quá 1000 ký tự")]
        [Display(Name = "Nội dung đánh giá")]
        public string Comment { get; set; }

        [Display(Name = "Ngày đánh giá")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Cập nhật lúc")]
        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        [Required(ErrorMessage = "Không tìm thấy thông tin sản phẩm")]
        [Display(Name = "Sản phẩm")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Không tìm thấy thông tin người dùng")]
        [Display(Name = "Người dùng")]
        public string UserId { get; set; }

        // Navigation Properties
        public Product? Product { get; set; }
        public ApplicationUser? User { get; set; }
    }
} 