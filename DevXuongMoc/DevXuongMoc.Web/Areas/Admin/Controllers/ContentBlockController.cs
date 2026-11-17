using DevXuongMoc.Domain.Entities;
using DevXuongMoc.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevXuongMoc.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContentBlockController : Controller
    {
        private readonly AppDbContext _context;

        public ContentBlockController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Danh sách khối nội dung
        public async Task<IActionResult> Index()
        {
            var blocks = await _context.ContentBlocks
                .OrderBy(b => b.Group)
                .ThenBy(b => b.SortOrder)
                .ToListAsync();
            return View(blocks);
        }

        // GET: Thêm mới
        public IActionResult Create()
        {
            return View();
        }

        // POST: Thêm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Group,Key,ContentHtml,SortOrder")] ContentBlock block)
        {
            // Kiểm tra xem Key đã tồn tại chưa (vì Key dùng để gọi ra ở giao diện)
            if (_context.ContentBlocks.Any(b => b.Key == block.Key))
            {
                ModelState.AddModelError("Key", "Mã định danh (Key) này đã tồn tại, vui lòng chọn Key khác!");
            }

            if (ModelState.IsValid)
            {
                block.CreatedAt = DateTime.Now;
                _context.Add(block);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(block);
        }

        // GET: Chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var block = await _context.ContentBlocks.FindAsync(id);
            if (block == null) return NotFound();
            return View(block);
        }

        // POST: Chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Group,Key,ContentHtml,SortOrder,CreatedAt")] ContentBlock block)
        {
            if (id != block.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    block.UpdatedAt = DateTime.Now;
                    _context.Entry(block).Property(x => x.CreatedAt).IsModified = false;
                    _context.Update(block);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlockExists(block.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(block);
        }

        // 4. POST: Xóa cứng (Không có IsDeleted trong DB)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var block = await _context.ContentBlocks.FindAsync(id);
            if (block != null)
            {
                _context.ContentBlocks.Remove(block);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BlockExists(int id)
        {
            return _context.ContentBlocks.Any(e => e.Id == id);
        }
    }
}