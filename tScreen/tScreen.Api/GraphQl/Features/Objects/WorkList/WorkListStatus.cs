using Core;
using Core.Text;

namespace GraphQl.GraphQl.Features.Objects.WorkList;

public class WorkListStatus : Enumeration
{
    public static WorkListStatus Reviewed = new WorkListStatus(0, nameof(Reviewed));
    public static WorkListStatus Unread = new WorkListStatus(1, nameof(Unread));

    protected WorkListStatus(int id, string name) : base(id, name.ToConstCase())
    {
        Value = name;
    }

    public string Value { get; }
}