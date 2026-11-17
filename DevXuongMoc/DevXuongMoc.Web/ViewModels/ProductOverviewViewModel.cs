namespace DevXuongMoc.Web.ViewModels;

public class ProductOverviewViewModel
{
    public List<BannerItemViewModel> Banners { get; set; } = new();
    public List<CategoryProductBlockViewModel> CategoryBlocks { get; set; } = new();
}

public class BannerItemViewModel
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = "/images/banner-sanpham-1.jpg";
    public string? LinkUrl { get; set; }
}

public class CategoryProductBlockViewModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public List<ProductCardViewModel> Products { get; set; } = new();
}