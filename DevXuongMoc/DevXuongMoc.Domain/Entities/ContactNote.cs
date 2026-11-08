using System;
using System.Collections.Generic;

namespace DevXuongMoc.Domain.Entities;
public partial class ContactNote
{
    public int Id { get; set; }

    public int ContactRequestId { get; set; }

    public string? Content { get; set; }

    public bool IsSystem { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ContactRequest ContactRequest { get; set; } = null!;
}
