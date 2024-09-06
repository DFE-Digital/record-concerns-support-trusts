using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.TargetedTrustEngagement
{
	public class ListByCaseUrn
	{
		public class Query : IRequest<TargetedTrustEngagementSummaryResponse[]>
		{
			public int concernsCaseUrn { get; set; }
		}

		public class Handler : IRequestHandler<Query, TargetedTrustEngagementSummaryResponse[]>
		{
			private readonly ConcernsDbContext _context;

			public Handler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task<TargetedTrustEngagementSummaryResponse[]> Handle(Query request, CancellationToken cancellationToken)
			{
				var ttes = _context.TargetedTrustEngagements.Include(a => a.ActivityTypes).Where(x => x.CaseUrn == request.concernsCaseUrn).ToArray();

				var result = ttes.Select(t => new TargetedTrustEngagementSummaryResponse()
				{
					CaseUrn = t.CaseUrn,
					TargetedTrustEngagementId = t.Id,
					CreatedAt = t.CreatedAt,
					UpdatedAt = t.UpdatedAt,
					Title = t.GetTitle(),
					Outcome = t.EngagementOutcomeId,
					ClosedAt = t.ClosedAt,
				}).ToArray();

				return result;
			}
		}
	}
}
