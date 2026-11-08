using System;
using System.Collections.Generic;

namespace DevXuongMoc.Domain.Entities;

public partial class EmailOutbox
{
    public int Id { get; set; }

    public byte Status { get; set; }

    public int Attempts { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
