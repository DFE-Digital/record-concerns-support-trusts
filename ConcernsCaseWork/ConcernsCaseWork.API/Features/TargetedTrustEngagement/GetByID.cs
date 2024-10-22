using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.TargetedTrustEngagement
{
	public class GetByID
	{
		public class Query : IRequest<GetTargetedTrustEngagementResponse>
		{
			[Range(1, int.MaxValue, ErrorMessage = "The CaseUrn must be greater than zero")]
			public int ConcernsCaseUrn { get; set; }

			[Range(1, int.MaxValue, ErrorMessage = "The TargetedTrustEngagementId must be greater than zero")]
			public int TargetedTrustEngagementId { get; set; }
		}

		public class Handler : IRequestHandler<Query, GetTargetedTrustEngagementResponse>
		{
			private readonly ConcernsDbContext _context;

			public Handler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task<GetTargetedTrustEngagementResponse> Handle(Query request, CancellationToken cancellationToken)	
			{
				var concernCase = await _context.ConcernsCase
					.FirstOrDefaultAsync(o => o.Urn == request.ConcernsCaseUrn);

				if (concernCase == null)
				{
					throw new NotFoundException($"Concerns case with id {request.ConcernsCaseUrn}");
				}

				var targetedTrustEngagement = await _context.TargetedTrustEngagements.Include(x => x.ActivityTypes).FirstOrDefaultAsync(d => d.Id == request.TargetedTrustEngagementId);

				if (targetedTrustEngagement == null)
				{
					throw new NotFoundException($"TargetedTrustEngagement with id {request.TargetedTrustEngagementId}, Case {request.ConcernsCaseUrn}");
				}


				var engagementActivities = targetedTrustEngagement.ActivityTypes;

				var activityTypes = engagementActivities.Where(a => a.ActivityTypeId.HasValue).Select(a => a.ActivityTypeId.Value).ToArray();

				var activityId = engagementActivities.FirstOrDefault()?.ActivityId;

				var result = new GetTargetedTrustEngagementResponse()
				{
					Id = request.TargetedTrustEngagementId,
					CaseUrn = concernCase.Id,
					EngagementStartDate = targetedTrustEngagement.EngagementStartDate,
					ActivityId = activityId,
					ActivityTypes = activityTypes,
					Notes = targetedTrustEngagement.Notes,
					EngagementEndDate = targetedTrustEngagement.EngagementEndDate,
					EngagementOutcomeId = (TargetedTrustEngagementOutcome?)targetedTrustEngagement.EngagementOutcomeId,
					CreatedAt = targetedTrustEngagement.CreatedAt,
					ClosedAt = targetedTrustEngagement.ClosedAt,
					UpdatedAt = targetedTrustEngagement.UpdatedAt,
					CreatedBy = targetedTrustEngagement.CreatedBy,
					Title = targetedTrustEngagement.GetTitle()
				};

				return result;
			
			}
		
		}
	}
}
