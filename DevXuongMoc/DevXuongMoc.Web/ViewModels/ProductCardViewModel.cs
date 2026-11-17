namespace DevXuongMoc.Web.ViewModels;

public class ProductCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = "/images/no-image.png";
    public string AltText { get; set; } = string.Empty;
    public byte Rating { get; set; } = 5;
    public string? ShortSpec { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal SalePrice { get; set; }
}