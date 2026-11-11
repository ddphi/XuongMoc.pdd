using DevXuongMoc.Domain.Entities;
using DevXuongMoc.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DevXuongMoc.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;

        public ArticleController(AppDbContext context)
        {
            _context = context;
        }

        //  Danh sách bài viết
        public async Task<IActionResult> Index()
        {
            var articles = await _context.Articles
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
            return View(articles);
        }

        //  GET: Thêm mới
        public IActionResult Create()
        {
            return View();
        }

        // POST: Thêm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Slug,BodyHtml,Status,PublishedAt")] Article article)
        {
            ModelState.Remove("Slug");

            if (string.IsNullOrEmpty(article.Slug))
            {
                article.Slug = GenerateSlug(article.Title);
            }

            if (_context.Articles.Any(a => a.Slug == article.Slug && !a.IsDeleted))
            {
                ModelState.AddModelError("Slug", "Đường dẫn (Slug) này đã tồn tại, vui lòng nhập tên khác!");
            }

            if (ModelState.IsValid)
            {
                article.CreatedAt = DateTime.Now;
                article.IsDeleted = false;

                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var article = await _context.Articles.FindAsync(id);
            if (article == null || article.IsDeleted) return NotFound();

            return View(article);
        }

        // POST: Chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Slug,BodyHtml,Status,PublishedAt,CreatedAt")] Article article)
        {
            if (id != article.Id) return NotFound();

            ModelState.Remove("Slug");

            if (string.IsNullOrEmpty(article.Slug))
            {
                article.Slug = GenerateSlug(article.Title);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    article.UpdatedAt = DateTime.Now;
                    _context.Entry(article).Property(x => x.CreatedAt).IsModified = false;
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        //POST: Xóa mềm
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                article.IsDeleted = true;
                article.UpdatedAt = DateTime.Now;
                _context.Update(article);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }

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