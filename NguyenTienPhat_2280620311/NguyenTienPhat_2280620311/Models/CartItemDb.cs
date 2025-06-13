using System;

namespace NguyenTienPhat_2280620311.Models
{
    public class CartItemDb
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
        public virtual Product Product { get; set; }
    }
} 