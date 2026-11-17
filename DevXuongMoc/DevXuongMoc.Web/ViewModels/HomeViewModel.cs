namespace DevXuongMoc.Web.ViewModels;

public class HomeViewModel
{
    public List<CategoryItemViewModel> Categories { get; set; } = new();
    public List<ProductCardViewModel> FeaturedProducts { get; set; } = new();
    public List<NewsCardViewModel> LatestNews { get; set; } = new();
    public List<PartnerItemViewModel> Partners { get; set; } = new();
}

public class CategoryItemViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string IconPath { get; set; } = "/images/icons/default.png";
}

public class NewsCardViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = "/images/news-default.jpg";
}

public class PartnerItemViewModel
{
    public string Name { get; set; } = string.Empty;
    public string LogoPath { get; set; } = string.Empty;
}