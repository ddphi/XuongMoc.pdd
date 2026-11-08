using DevXuongMoc.Domain.Entities;
using DevXuongMoc.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DevXuongMoc.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Danh sách sản phẩm (Chỉ lấy những sản phẩm chưa bị xóa mềm)
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category) // Lấy kèm thông tin Danh mục
                .Include(p => p.ProductImages) // Lấy kèm thông tin Ảnh sản phẩm
                .Where(p => !p.IsDeleted) // Chỉ lấy sản phẩm chưa bị xóa
                .ToListAsync();
            return View(products);
        }

        // GET: Admin/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null || product.IsDeleted) return NotFound();

            return View(product);
        }

        // 2. GET
        public IActionResult Create()
        {
            //do du lieu danh muc vao select
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Thêm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile> ImageFiles)
        {
            ModelState.Remove("Category");
            ModelState.Remove("Slug");
            ModelState.Remove("Variants");
            ModelState.Remove("Images");

            if (string.IsNullOrEmpty(product.Slug))
            {
                product.Slug = GenerateSlug(product.Name);
            }

            if (_context.Products.Any(p => p.Slug == product.Slug && !p.IsDeleted))
            {
                ModelState.AddModelError("Slug", "Đường dẫn (Slug) này đã tồn tại!");
            }

            if (product.Status == 0) product.Status = 1;

            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;
                product.IsDeleted = false;
                product.ViewCount = 0;
                product.Rating = 0;

                //phan su ly anh san pham
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                    bool isFirstImage = true;
                    foreach (var file in ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            // Tạo tên file ngẫu nhiên
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string filePath = Path.Combine(uploadDir, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Thêm vào danh sách ảnh của sản phẩm
                            product.ProductImages.Add(new ProductImage
                            {
                                Path = "/uploads/products/" + fileName,
                                AltText = product.Name, //lay ten sna pham lam mac dinh
                                IsPrimary = isFirstImage, // Ảnh đầu tiên làm ảnh đại diện
                                CreatedAt = DateTime.Now
                            });
                            isFirstImage = false;
                        }
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // 3. GET: Chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null || product.IsDeleted) return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, List<IFormFile> ImageFiles)
        {
            if (id != product.Id) return NotFound();

            ModelState.Remove("Category");
            ModelState.Remove("Slug");
            ModelState.Remove("Variants");
            ModelState.Remove("ProductImages");

            if (string.IsNullOrEmpty(product.Slug))
            {
                product.Slug = GenerateSlug(product.Name);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.UpdatedAt = DateTime.Now;
                    _context.Entry(product).Property(x => x.CreatedAt).IsModified = false;
                    _context.Update(product);
                    if (ImageFiles != null && ImageFiles.Count > 0)
                    {
                        string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                        // Kiểm tra xem sản phẩm đã có ảnh đại diện chưa
                        bool hasPrimary = _context.ProductImages.Any(i => i.ProductId == product.Id && i.IsPrimary);

                        foreach (var file in ImageFiles)
                        {
                            if (file.Length > 0)
                            {
                                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                string filePath = Path.Combine(uploadDir, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                // Thêm ảnh mới vào DB
                                _context.ProductImages.Add(new ProductImage
                                {
                                    ProductId = product.Id,
                                    Path = "/uploads/products/" + fileName,
                                    AltText = product.Name,
                                    IsPrimary = !hasPrimary, // Nếu chưa có ảnh nào thì ảnh này làm đại diện
                                    CreatedAt = DateTime.Now
                                });
                                hasPrimary = true; // Các ảnh tiếp theo không làm đại diện nữa
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }


        // post : thêm biến thể mới 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVariant(int ProductId, string MaterialName, decimal OriginalPrice, decimal SalePrice)
        {
            if (ModelState.IsValid)
            {
                var variant = new ProductVariant
                {
                    ProductId = ProductId,
                    MaterialName = MaterialName,
                    OriginalPrice = OriginalPrice,
                    SalePrice = SalePrice,
                    CreatedAt = DateTime.Now
                };
                _context.ProductVariants.Add(variant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = ProductId });
            }
            return RedirectToAction(nameof(Edit), new { id = ProductId });
        }

        // 4. POST: Soft Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                product.UpdatedAt = DateTime.Now;
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
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