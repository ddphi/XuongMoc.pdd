using DevXuongMoc.Infrastructure.Data;
using DevXuongMoc.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevXuongMoc.Web.Controllers
{
    // [Route] giúp định nghĩa đường dẫn gốc cho toàn bộ Controller này
    [Route("san-pham")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /san-pham
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            // 1. Lấy Banners
            var banners = await _context.Banners
                .Where(b => b.IsActive && b.Placement == 2
                       && (b.StartsAt == null || b.StartsAt <= DateTime.Now)
                       && (b.EndsAt == null || b.EndsAt >= DateTime.Now))
                .Select(b => new BannerItemViewModel
                {
                    Id = b.Id,
                    ImageUrl = b.ImagePath ?? "/images/banner-sanpham-1.jpg",
                    LinkUrl = b.LinkUrl
                })
                .ToListAsync();

            // 2. Lấy danh mục và 4 sản phẩm
            var categories = await _context.Categories
                .Where(c => c.IsVisible)
                .Select(c => new CategoryProductBlockViewModel
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    CategorySlug = c.Slug,
                    Products = c.Products
                        .Where(p => !p.IsDeleted && p.Status == 1)
                        .Take(4)
                        .Select(p => new ProductCardViewModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Slug = p.Slug,
                            CategorySlug = c.Slug,
                            ThumbnailPath = p.ProductImages.Where(i => i.IsPrimary).Select(i => i.Path).FirstOrDefault()
                                            ?? p.ProductImages.Select(i => i.Path).FirstOrDefault()
                                            ?? "/images/no-image.png",
                            AltText = p.Name,
                            Rating = p.Rating,
                            ShortSpec = p.ShortSpec,
                            OriginalPrice = p.ProductVariants.Any() ? p.ProductVariants.Min(v => v.OriginalPrice) : 0,
                            SalePrice = p.ProductVariants.Any() ? p.ProductVariants.Min(v => v.SalePrice) : 0
                        }).ToList()
                })
                .ToListAsync();

            // 3. Gộp vào ViewModel tổng
            var model = new ProductOverviewViewModel
            {
                Banners = banners,
                CategoryBlocks = categories
            };

            return View(model);
        }
    }
}