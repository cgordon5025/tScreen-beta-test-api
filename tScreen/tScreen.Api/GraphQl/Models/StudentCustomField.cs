﻿using System;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class StudentCustomField : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid CustomFieldId { get; set; }
        public string? Value { get; set; }

        public Student? Student { get; set; }

        public CustomField? CustomField { get; set; }
    }
}