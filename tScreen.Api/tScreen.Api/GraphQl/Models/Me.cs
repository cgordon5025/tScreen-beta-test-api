using System.Collections.Generic;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models;

[Authorize]
public class Me : BaseEntity
{
    public Company? Company { get; set; }
    public ICollection<Location> Locations { get; set; } = new HashSet<Location>();
    public ICollection<WorkList> WorkLists { get; set; } = new HashSet<WorkList>();
    public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
    public ICollection<History> History { get; set; } = new HashSet<History>();
}