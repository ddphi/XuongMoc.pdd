using System;
using System.Collections.Generic;

namespace DevXuongMoc.Domain.Entities;

public partial class Partner
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? LogoPath { get; set; }

    public bool ShowOnHome { get; set; }

    public bool IsVisible { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
