using System;
using System.Collections.Generic;

namespace DevXuongMoc.Domain.Entities;

public partial class MediaFile
{
    public int Id { get; set; }

    public string Path { get; set; } = null!;

    public DateTime? OrphanedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
