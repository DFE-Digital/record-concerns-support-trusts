using AutoMapper;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.Decision
{
	public class ListByCaseUrn
	{
		public class Query : IRequest<DecisionSummaryResponse[]>
		{
			public int concernsCaseUrn { get; set; }
		}

		public class Handler : IRequestHandler<Query, DecisionSummaryResponse[]>
		{
			private readonly ConcernsDbContext _context;

			public Handler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task<DecisionSummaryResponse[]> Handle(Query request, CancellationToken cancellationToken)
			{
				var exp = _context.ConcernsCase.AsNoTracking()
									.Include(x => x.Decisions)
									.ThenInclude(x => x.DecisionTypes)
									.Include(x => x.Decisions)
									.ThenInclude(x => x.Outcome)
									.ThenInclude(x => x.BusinessAreasConsulted);
				
				var concernsCase = exp.FirstOrDefault(c => c.Urn == request.concernsCaseUrn);

				var decisions = concernsCase.Decisions;

				var result = decisions.Select(decision => new DecisionSummaryResponse
				{
					ConcernsCaseUrn = concernsCase.Urn,
					DecisionId = decision.DecisionId,
					Status = (DecisionStatus)decision.Status,
					Outcome = decision.Outcome?.Status,
					CreatedAt = decision.CreatedAt,
					UpdatedAt = decision.UpdatedAt,
					ClosedAt = decision.ClosedAt,
					Title = decision.GetTitle(),
				}).ToArray();

				return result;
			}

		}
	}
}
