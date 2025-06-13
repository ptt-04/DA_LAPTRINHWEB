using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenTienPhat_2280620311.Models;
using System.Security.Claims;

namespace NguyenTienPhat_2280620311.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Review/Index
        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return View(reviews);
        }

        // GET: Review/ProductReviews/5
        public async Task<IActionResult> ProductReviews(int id)
        {
            var product = await _context.Products
                .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Review/Create
        [Authorize(Roles = SD.Role_Customer)]
        public async Task<IActionResult> Create(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Kiểm tra xem người dùng đã đánh giá sản phẩm này chưa
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

            if (existingReview != null)
            {
                TempData["Error"] = "Bạn đã đánh giá sản phẩm này rồi. Bạn có thể sửa đánh giá cũ.";
                return RedirectToAction("ProductReviews", new { id = productId });
            }

            var review = new Review
            {
                ProductId = productId
            };

            ViewBag.Product = product;
            return View(review);
        }

        // POST: Review/Create
        [HttpPost]
        [Authorize(Roles = SD.Role_Customer)]
        public async Task<IActionResult> Create([FromForm] int ProductId, [FromForm] int Rating, [FromForm] string Comment)
        {
            if (Rating < 1 || Rating > 5)
                return Json(new { success = false, message = "Số sao đánh giá không hợp lệ!" });
            if (string.IsNullOrWhiteSpace(Comment) || Comment.Length < 10 || Comment.Length > 1000)
                return Json(new { success = false, message = "Bình luận phải từ 10 đến 1000 ký tự!" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingReview = await _context.Reviews.FirstOrDefaultAsync(r => r.ProductId == ProductId && r.UserId == userId);
            if (existingReview != null)
            {
                return Json(new { success = false, message = "Bạn đã đánh giá sản phẩm này rồi. Hãy sửa đánh giá cũ nếu muốn thay đổi." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy thông tin người dùng!" });

            var review = new Review
            {
                ProductId = ProductId,
                Rating = Rating,
                Comment = Comment.Trim(),
                UserId = userId,
                CreatedAt = DateTime.Now
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Đánh giá đã được thêm thành công!",
                review = new
                {
                    id = review.Id,
                    rating = review.Rating,
                    comment = review.Comment,
                    createdAt = review.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                    userFullName = user.FullName,
                    userId = userId
                }
            });
        }

        // GET: Review/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền chỉnh sửa
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(review);
        }

        // POST: Review/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Rating,Comment")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            var existingReview = await _context.Reviews.FindAsync(id);
            if (existingReview == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền chỉnh sửa
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingReview.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingReview.Rating = review.Rating;
                    existingReview.Comment = review.Comment;
                    existingReview.UpdatedAt = DateTime.Now;

                    _context.Update(existingReview);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Đánh giá đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ProductReviews", new { id = review.ProductId });
            }

            review.Product = existingReview.Product;
            return View(review);
        }

        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền xóa
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var productId = review.ProductId;
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đánh giá đã được xóa thành công!";
            return RedirectToAction("ProductReviews", new { id = productId });
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
} 