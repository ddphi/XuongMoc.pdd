using System;
using System.Collections.Generic;

namespace DevXuongMoc.Domain.Entities;

public partial class SiteSetting
{
    public string Key { get; set; } = null!;

    public string? Value { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
