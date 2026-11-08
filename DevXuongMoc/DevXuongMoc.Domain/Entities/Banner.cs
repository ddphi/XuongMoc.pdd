using System;
using System.Collections.Generic;

namespace DevXuongMoc.Domain.Entities;

public partial class Banner
{
    public int Id { get; set; }

    public byte Placement { get; set; }

    public string? LinkUrl { get; set; }

    public DateTime? StartsAt { get; set; }

    public DateTime? EndsAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
