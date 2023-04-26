using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQl.GraphQl.Extensions
{
    public static class ObjectFieldExtensions
    {
        public static IObjectFieldDescriptor UseDbContext<TDbContext>(this IObjectFieldDescriptor descriptor)
            where TDbContext : DbContext
            => descriptor.UseScopedService(
                create: service => service.GetRequiredService<IDbContextFactory<TDbContext>>().CreateDbContext(),
                disposeAsync: (_, context) => context.DisposeAsync());
    }
}