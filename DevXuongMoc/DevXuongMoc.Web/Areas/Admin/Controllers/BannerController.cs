using DevXuongMoc.Domain.Entities;
using DevXuongMoc.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevXuongMoc.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : Controller
    {
        private readonly AppDbContext _context;

        public BannerController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var banners = await _context.Banners.OrderByDescending(b => b.Id).ToListAsync();
            return View(banners);
        }

        // GET: Thêm mới
        public IActionResult Create()
        {
            return View();
        }

        // POST: Thêm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Placement,LinkUrl,StartsAt,EndsAt,IsActive")] Banner banner)
        {
            // Validate logic thời gian
            if (banner.StartsAt.HasValue && banner.EndsAt.HasValue && banner.EndsAt < banner.StartsAt)
            {
                ModelState.AddModelError("EndsAt", "Thời gian kết thúc phải lớn hơn thời gian bắt đầu!");
            }

            if (ModelState.IsValid)
            {
                banner.CreatedAt = DateTime.Now;
                _context.Add(banner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(banner);
        }

        // GET: Chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) return NotFound();
            return View(banner);
        }

        // POST: Chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Placement,LinkUrl,StartsAt,EndsAt,IsActive,CreatedAt")] Banner banner)
        {
            if (id != banner.Id) return NotFound();

            if (banner.StartsAt.HasValue && banner.EndsAt.HasValue && banner.EndsAt < banner.StartsAt)
            {
                ModelState.AddModelError("EndsAt", "Thời gian kết thúc phải lớn hơn thời gian bắt đầu!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    banner.UpdatedAt = DateTime.Now;
                    _context.Entry(banner).Property(x => x.CreatedAt).IsModified = false;
                    _context.Update(banner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BannerExists(banner.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(banner);
        }

        // POST: Xóa cứng
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner != null)
            {
                _context.Banners.Remove(banner);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BannerExists(int id)
        {
            return _context.Banners.Any(e => e.Id == id);
        }
    }
}