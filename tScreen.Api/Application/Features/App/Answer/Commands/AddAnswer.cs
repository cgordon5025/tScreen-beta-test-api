using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.App.Answer.Commands;

public class AddAnswer : IRequest<AnswerDTO>
{
    public AnswerDTO? AnswerDTO { get; set; }
    
    internal sealed class AddAnswerHandler : IRequestHandler<AddAnswer, AnswerDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public AddAnswerHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }
        
        public async Task<AnswerDTO> Handle(AddAnswer request, CancellationToken cancellationToken)
        {
            if (request.AnswerDTO == null)
                throw new NullReferenceException(nameof(request.AnswerDTO));
            
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var entity = _mapper.Map<Domain.Entities.App.Answer>(request.AnswerDTO);
            context.AppAnswers.Add(entity);
            
            await context.SaveChangesAsync(CancellationToken.None);

            return _mapper.Map<AnswerDTO>(entity);
        }
    }
}