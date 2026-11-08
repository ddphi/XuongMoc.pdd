using System;
using System.Collections.Generic;

namespace DevXuongMoc.Domain.Entities;

public partial class ContactRequest
{
    public int Id { get; set; }

    public byte Type { get; set; }

    public byte Status { get; set; }

    public int? ProductId { get; set; }

    public string? AssignedToId { get; set; }

    public string? Message { get; set; }

    public string? IpAddress { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual User? AssignedTo { get; set; }

    public virtual ICollection<ContactNote> ContactNotes { get; set; } = new List<ContactNote>();

    public virtual Product? Product { get; set; }
}
