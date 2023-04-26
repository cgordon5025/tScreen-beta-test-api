using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GraphQl.GraphQl.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}