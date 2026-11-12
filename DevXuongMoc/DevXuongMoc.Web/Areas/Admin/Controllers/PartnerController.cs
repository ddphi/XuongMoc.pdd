using DevXuongMoc.Domain.Entities;
using DevXuongMoc.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevXuongMoc.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PartnerController : Controller
    {
        private readonly AppDbContext _context;

        public PartnerController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var partners = await _context.Partners.ToListAsync();
            return View(partners);
        }

        // Thêm mới
        public IActionResult Create()
        {
            return View();
        }

        // Thêm mới (Có xử lý upload Logo)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ShowOnHome,IsVisible")] Partner partner, IFormFile? LogoFile)
        {
            if (ModelState.IsValid)
            {
                partner.CreatedAt = DateTime.Now;

                // Xử lý Upload Logo
                if (LogoFile != null && LogoFile.Length > 0)
                {
                    string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "partners");
                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(LogoFile.FileName);
                    string filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await LogoFile.CopyToAsync(stream);
                    }
                    partner.LogoPath = "/uploads/partners/" + fileName;
                }

                _context.Add(partner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(partner);
        }

        // GET: Chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var partner = await _context.Partners.FindAsync(id);
            if (partner == null) return NotFound();
            return View(partner);
        }

        // POST: Chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ShowOnHome,IsVisible,LogoPath,CreatedAt")] Partner partner, IFormFile? LogoFile)
        {
            if (id != partner.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    partner.UpdatedAt = DateTime.Now;

                    // Nếu có chọn Logo mới thì cập nhật, không thì giữ nguyên Logo cũ
                    if (LogoFile != null && LogoFile.Length > 0)
                    {
                        string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "partners");
                        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(LogoFile.FileName);
                        string filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await LogoFile.CopyToAsync(stream);
                        }

                        // Xóa logo cũ (nếu có) để tránh rác server
                        if (!string.IsNullOrEmpty(partner.LogoPath))
                        {
                            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", partner.LogoPath.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                        }

                        partner.LogoPath = "/uploads/partners/" + fileName;
                    }

                    _context.Entry(partner).Property(x => x.CreatedAt).IsModified = false;
                    _context.Update(partner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartnerExists(partner.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(partner);
        }

        // POST: Xóa cứng
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner != null)
            {
                // Xóa file logo vật lý
                if (!string.IsNullOrEmpty(partner.LogoPath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", partner.LogoPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }

                _context.Partners.Remove(partner);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PartnerExists(int id)
        {
            return _context.Partners.Any(e => e.Id == id);
        }
    }
}