using System;
using System.Collections.Generic;

namespace DevXuongMoc.Domain.Entities;

public partial class AuditLog
{
    public long Id { get; set; }

    public string? UserId { get; set; }

    public string? Action { get; set; }

    public string? EntityName { get; set; }

    public string? Summary { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual User? User { get; set; }
}
