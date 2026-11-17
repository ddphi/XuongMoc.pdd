using DevXuongMoc.Infrastructure.Data;
using DevXuongMoc.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevXuongMoc.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsVisible)
            .OrderBy(c => c.SortOrder)
            .Take(8)
            .Select(c => new CategoryItemViewModel
            {
                Name = c.Name,
                Slug = c.Slug,
                IconPath = string.IsNullOrWhiteSpace(c.IconPath) ? "/images/icons/cat-default.png" : c.IconPath
            })
            .ToListAsync();

        var featuredProducts = await _context.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.IsFeatured && p.Status == 1)
            .OrderByDescending(p => p.Id)
            .Take(8)
            .Select(p => new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                CategorySlug = p.Category.Slug,
                ShortSpec = p.ShortSpec,
                Rating = p.Rating,
                ThumbnailPath = p.ProductImages
                    .OrderByDescending(i => i.IsPrimary)
                    .Select(i => i.Path)
                    .FirstOrDefault() ?? "/images/no-image.png",
                AltText = p.ProductImages
                    .OrderByDescending(i => i.IsPrimary)
                    .Select(i => i.AltText)
                    .FirstOrDefault() ?? p.Name,
                OriginalPrice = p.ProductVariants
                    .OrderByDescending(v => v.IsDefault)
                    .Select(v => v.OriginalPrice)
                    .FirstOrDefault(),
                SalePrice = p.ProductVariants
                    .OrderByDescending(v => v.IsDefault)
                    .Select(v => v.SalePrice)
                    .FirstOrDefault()
            })
            .ToListAsync();

        var latestNews = await _context.Articles
            .AsNoTracking()
            .Where(a => a.Status == 1)
            .OrderByDescending(a => a.PublishedAt ?? a.CreatedAt)
            .Take(4)
            .Select(a => new NewsCardViewModel
            {
                Title = a.Title,
                Slug = a.Slug,
                Excerpt = a.Title,
                ThumbnailPath = "/images/news-default.jpg"
            })
            .ToListAsync();

        var partners = await _context.Partners
            .AsNoTracking()
            .Where(p => p.IsVisible && p.ShowOnHome)
            .OrderBy(p => p.Id)
            .Take(7)
            .Select(p => new PartnerItemViewModel
            {
                Name = p.Name,
                LogoPath = p.LogoPath ?? "/images/partner-default.png"
            })
            .ToListAsync();

        var vm = new HomeViewModel
        {
            Categories = categories,
            FeaturedProducts = featuredProducts,
            LatestNews = latestNews,
            Partners = partners
        };

        return View(vm);
    }
}