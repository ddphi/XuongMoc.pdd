using System;
using System.Collections.Generic;

namespace DevXuongMoc.Domain.Entities;
public partial class Article
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? BodyHtml { get; set; }

    public byte Status { get; set; }

    public DateTime? PublishedAt { get; set; }

    public string? AuthorId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual User? Author { get; set; }
}
