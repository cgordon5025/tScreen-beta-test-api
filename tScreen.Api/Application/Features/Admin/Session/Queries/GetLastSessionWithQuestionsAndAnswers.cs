using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Core.Extensions;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Session.Queries;

public class GetLastSessionWithQuestionsAndAnswers : IRequest<SessionDTO>
{
    public Guid SessionId { get; init; }
    
    internal sealed class GetLastSessionWithQuestionsAndAnswersHandler
        : IRequestHandler<GetLastSessionWithQuestionsAndAnswers, SessionDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public GetLastSessionWithQuestionsAndAnswersHandler(IDbContextFactory<ApplicationDbContext> contextFactory, 
            IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }
        
        public async Task<SessionDTO> Handle(GetLastSessionWithQuestionsAndAnswers request, 
            CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var entities = await context.AppSessions
                .TagWith(nameof(GetLastSessionWithQuestionsAndAnswers))
                .TagWithCallSiteSafely()
                .Include(e => e.Student)
                .Include(e => e.Answers)
                .ThenInclude(e => e.Question)
                .Where(e => e.Id == request.SessionId)
                .FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<SessionDTO>(entities);
        }
    }
}