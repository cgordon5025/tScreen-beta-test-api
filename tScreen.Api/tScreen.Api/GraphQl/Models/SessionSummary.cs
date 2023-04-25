using System;
using System.Collections.Generic;

namespace GraphQl.GraphQl.Models;


public class SessionSummary
{
    public Guid Id { get; set; }
    public string? LivingSituation { get; set; }
    public int NumberOfSiblings { get; set; } = 0;
    public ICollection<string> PeopleCountedOn { get; set; } = new List<string>();
    public string? LivesMainlyWith { get; set; }
    
    public string? Status { get; set; }
}