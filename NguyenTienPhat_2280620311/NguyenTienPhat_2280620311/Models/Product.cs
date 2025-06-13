using System.ComponentModel.DataAnnotations;

namespace NguyenTienPhat_2280620311.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        [Range(0.01, 10000000.00, ErrorMessage = "Giá phải từ 0.01 đến 10,000,000")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả sản phẩm")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }

        public List<ProductImage>? Images { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        // Navigation property for Reviews
        public List<Review>? Reviews { get; set; }
    }
}
