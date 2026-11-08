using System;
using System.Collections.Generic;

namespace DevXuongMoc.Domain.Entities;

public partial class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string MaterialName { get; set; } = null!;

    public decimal OriginalPrice { get; set; }

    public decimal SalePrice { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Product Product { get; set; } = null!;
}
