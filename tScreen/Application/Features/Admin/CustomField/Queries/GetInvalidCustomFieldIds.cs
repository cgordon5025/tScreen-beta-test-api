using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.CustomField.Queries
{
    public class GetInvalidCustomFieldIds : IRequest<IEnumerable<Guid>>
    {
        public Guid LocationId { get; set; }
        public IEnumerable<Guid>? Ids { get; init; }

        internal sealed class
            ValidateCustomFieldExistHandler : IRequestHandler<GetInvalidCustomFieldIds, IEnumerable<Guid>>
        {
            private readonly ApplicationDbContext _context;

            public ValidateCustomFieldExistHandler(IDbContextFactory<ApplicationDbContext> context)
            {
                _context = context.CreateDbContext();
            }

            public async Task<IEnumerable<Guid>> Handle(GetInvalidCustomFieldIds request, CancellationToken cancellationToken)
            {
                if (request.Ids == null)
                    throw new NullReferenceException(nameof(request.Ids));

                var customFields = await _context.CustomField
                    .TagWith(nameof(GetInvalidCustomFieldIds))
                    .TagWithCallSiteSafely()
                    .Where(e => e.LocationId == request.LocationId)
                    .Where(e => e.DeletedAt == null)
                    .Select(e => e.Id)
                    .ToListAsync(cancellationToken);

                var ids = new List<Guid>();

                if (!customFields.Any())
                    return ids;

                ids.AddRange(
                    request.Ids
                        .Where(id => !customFields.Contains(id)));

                return ids;
            }
        }
    }
}