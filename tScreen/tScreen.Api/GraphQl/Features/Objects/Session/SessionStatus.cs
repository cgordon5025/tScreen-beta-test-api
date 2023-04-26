using Core;
using Core.Text;
using HotChocolate.Types;

namespace GraphQl.GraphQl.Features.Objects.Session;

public class SessionStatus : Enumeration
{
    public static readonly SessionStatus Abandoned = new SessionStatus(0, nameof(Abandoned));
    public static readonly SessionStatus Complete = new SessionStatus(1, nameof(Complete));
    public static readonly SessionStatus Incomplete = new SessionStatus(2, nameof(Incomplete));
    public static readonly SessionStatus Pending = new SessionStatus(3, nameof(Pending));
    public static readonly SessionStatus TestingSnakeCase = new SessionStatus(4, nameof(TestingSnakeCase));

    protected SessionStatus(int id, string name) : base(id, name.ToConstCase())
    {
        Value = name.ToPascalCase();
    }

    public string Value { get; }
}

public class SessionStatusType : EnumType<SessionStatus>
{
    protected override void Configure(IEnumTypeDescriptor<SessionStatus> descriptor)
    {
        descriptor.Value(SessionStatus.Abandoned).Name(SessionStatus.Abandoned.Name);
        descriptor.Value(SessionStatus.Complete).Name(SessionStatus.Complete.Name);
        descriptor.Value(SessionStatus.Incomplete).Name(SessionStatus.Incomplete.Name);
        descriptor.Value(SessionStatus.Pending).Name(SessionStatus.Pending.Name);
    }
}