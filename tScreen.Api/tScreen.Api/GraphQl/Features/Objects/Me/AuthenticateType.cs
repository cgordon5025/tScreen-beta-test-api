using System;
using System.Globalization;
using GraphQl.GraphQl.Models;
using HotChocolate;
using HotChocolate.Types;

namespace GraphQl.GraphQl.Features.Objects.Me;

public class AuthenticateType : ObjectTypeExtension<Models.AuthenticationResult>
{
    protected override void Configure(IObjectTypeDescriptor<AuthenticationResult> descriptor)
    {
        descriptor
            .Field(e => e.Expires)
            .Argument("format", a => a.Type<StringType>())
            .ResolveWith<AuthenticateResolvers>(r => 
                AuthenticateResolvers.FormatExpires(default, default!));
    }

    private class AuthenticateResolvers
    {
        public static string? FormatExpires(string? format, [Parent] AuthenticationResult parent)
        {
            if (!double.TryParse(parent.Expires, out var expires) || string.IsNullOrWhiteSpace(format))
                return parent.Expires;

            if (format.ToUpper() == "ISO")
                return DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(expires))
                    .ToString(CultureInfo.InvariantCulture);

            return DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(expires))
                .ToString(format, CultureInfo.InvariantCulture);
        }
    }
}