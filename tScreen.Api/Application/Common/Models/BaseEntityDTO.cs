using System;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Common.Interfaces;

namespace Application.Common.Models
{
    public class BaseEntityDTO : IEntityDTO
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
        
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}