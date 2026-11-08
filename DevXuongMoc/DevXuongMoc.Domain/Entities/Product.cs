using System;
using System.Collections.Generic;

namespace DevXuongMoc.Domain.Entities;

public partial class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? ShortSpec { get; set; }

    public string? Spec { get; set; }

    public string? Care { get; set; }

    public string? Warranty { get; set; }

    public string? Commitment { get; set; }

    public string? Features { get; set; }

    public int WarrantyMonths { get; set; }

    public byte Rating { get; set; }

    public byte Status { get; set; }

    public bool IsFeatured { get; set; }

    public int ViewCount { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ContactRequest> ContactRequests { get; set; } = new List<ContactRequest>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
