using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NguyenTienPhat_2280620311.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NguyenTienPhat_2280620311.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CouponController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var coupons = await _context.Coupons.ToListAsync();
            return View(coupons);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra mã giảm giá đã tồn tại chưa
                if (await _context.Coupons.AnyAsync(c => c.Code == coupon.Code))
                {
                    ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại");
                    return View(coupon);
                }

                coupon.CreatedAt = DateTime.Now;
                _context.Add(coupon);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mã giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupon coupon)
        {
            if (id != coupon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra mã giảm giá đã tồn tại chưa (trừ mã hiện tại)
                    if (await _context.Coupons.AnyAsync(c => c.Code == coupon.Code && c.Id != coupon.Id))
                    {
                        ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại");
                        return View(coupon);
                    }

                    _context.Update(coupon);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật mã giảm giá thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponExists(coupon.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa mã giảm giá thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.Id == id);
        }
    }
} 