using DevXuongMoc.Domain.Entities;
using DevXuongMoc.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DevXuongMoc.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Admin/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Slug,IconPath,SortOrder,IsVisible")] Category category)
        {
            // Nếu người dùng để trống Slug, tự động tạo từ Name
            if (string.IsNullOrEmpty(category.Slug))
            {
                category.Slug = GenerateSlug(category.Name);
                // Xóa lỗi validation của trường Slug (nếu có) vì chúng ta đã tự điền
                ModelState.Remove("Slug");
            }

            // Gán thời gian tạo
            category.CreatedAt = DateTime.Now;

            // Kiểm tra xem Slug đã tồn tại chưa (vì trong DB yêu cầu duy nhất)
            if (_context.Categories.Any(c => c.Slug == category.Slug))
            {
                ModelState.AddModelError("Slug", "Đường dẫn (Slug) này đã tồn tại, vui lòng nhập tên khác!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Slug,IconPath,SortOrder,IsVisible,CreatedAt")] Category category)
        {
            if (id != category.Id) return NotFound();

            // Tự động tạo Slug nếu để trống khi sửa
            if (string.IsNullOrEmpty(category.Slug))
            {
                category.Slug = GenerateSlug(category.Name);
                ModelState.Remove("Slug");
            }

            // Gán thời gian cập nhật
            category.UpdatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    // Không update cột CreatedAt để tránh lỗi ghi đè
                    _context.Entry(category).Property(x => x.CreatedAt).IsModified = false;
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                try
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    // Bắt lỗi khóa ngoại (Restrict): Không thể xóa danh mục đang có sản phẩm
                    ModelState.AddModelError("", "Không thể xóa danh mục này vì đang có sản phẩm trực thuộc!");
                    return View(category);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        // Hàm tạo Slug tự động
        private string GenerateSlug(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            text = text.ToLowerInvariant();
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = text.Normalize(System.Text.NormalizationForm.FormD);
            temp = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            temp = Regex.Replace(temp, @"[^a-z0-9\s-]", "");
            temp = Regex.Replace(temp, @"\s+", "-").Trim();
            return temp;
        }
    }
}