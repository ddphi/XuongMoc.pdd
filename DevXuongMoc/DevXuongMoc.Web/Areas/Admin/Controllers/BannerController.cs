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
        // SỬA Ở ĐÂY: Thêm IFormFile? ImageFile vào tham số
        public async Task<IActionResult> Create([Bind("Placement,LinkUrl,StartsAt,EndsAt,IsActive")] Banner banner, IFormFile? ImageFile)
        {
            // Validate logic thời gian
            if (banner.StartsAt.HasValue && banner.EndsAt.HasValue && banner.EndsAt < banner.StartsAt)
            {
                ModelState.AddModelError("EndsAt", "Thời gian kết thúc phải lớn hơn thời gian bắt đầu!");
            }

            if (ModelState.IsValid)
            {
                banner.CreatedAt = DateTime.Now;

                // Xử lý upload ảnh
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "banners");
                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    string filePath = Path.Combine(uploadDir, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create)) { await ImageFile.CopyToAsync(stream); }
                    banner.ImagePath = "/uploads/banners/" + fileName;
                }

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
        // SỬA Ở ĐÂY: Thêm IFormFile? ImageFile và "ImagePath" vào Bind
        public async Task<IActionResult> Edit(int id, [Bind("Id,Placement,LinkUrl,StartsAt,EndsAt,IsActive,CreatedAt,ImagePath")] Banner banner, IFormFile? ImageFile)
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

                    // Xử lý upload ảnh mới (nếu có)
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "banners");
                        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        string filePath = Path.Combine(uploadDir, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create)) { await ImageFile.CopyToAsync(stream); }

                        // Xóa ảnh cũ để tránh rác server
                        if (!string.IsNullOrEmpty(banner.ImagePath))
                        {
                            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", banner.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                        }

                        banner.ImagePath = "/uploads/banners/" + fileName;
                    }

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
                // Xóa ảnh vật lý trước khi xóa DB
                if (!string.IsNullOrEmpty(banner.ImagePath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", banner.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }

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