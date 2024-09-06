using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.TargetedTrustEngagement
{
	using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
	using ConcernsCaseWork.API.Exceptions;
	using ConcernsCaseWork.Data.Models;
	using Microsoft.EntityFrameworkCore;

	public class Update
	{
		public class Command : IRequest<UpdateTargetedTrustEngagementResponse>
		{
			public int ConcernsCaseUrn { get; }
			public int TargetedTrustEngagementId { get; }

			public UpdateTargetedTrustEngagementRequest Request { get; set; }

			public Command(int concernsCaseUrn, int targetedTrustEngagementId, UpdateTargetedTrustEngagementRequest request)
			{
				this.ConcernsCaseUrn = concernsCaseUrn;
				this.TargetedTrustEngagementId = targetedTrustEngagementId;
				this.Request = request;
			}
		}

		public class CommandHandler : IRequestHandler<Command, UpdateTargetedTrustEngagementResponse>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;

			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task<UpdateTargetedTrustEngagementResponse> Handle(Command command, CancellationToken cancellationToken)
			{
				var request = command.Request;
				var exp = _context.ConcernsCase;

				var concernsCase = await exp.FirstOrDefaultAsync(c => c.Urn == command.ConcernsCaseUrn);

				if (concernsCase == null)
				{
					throw new NotFoundException($"Concerns case {command.ConcernsCaseUrn}");
				}

				var tte = await _context.TargetedTrustEngagements.Include(a => a.ActivityTypes).FirstOrDefaultAsync(t => t.Id == command.TargetedTrustEngagementId);

				if (tte == null)
				{
					throw new NotFoundException($"Targeted Trust Engagement {command.TargetedTrustEngagementId}");
				}

				var engagementActiviteis = tte.ActivityTypes;

				_context.TrustEngagementActivities.RemoveRange(engagementActiviteis);

				var activityTypes = new List<TargetedTrustEngagementType>();

				if (request.ActivityId.HasValue && request.ActivityTypes.Length == 0)
				{
					var at = new TargetedTrustEngagementType((Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivity)request.ActivityId, null);
					activityTypes.Add(at);
				}
				else
				{
					activityTypes = request.ActivityTypes.Select(x => new TargetedTrustEngagementType((Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivity)request.ActivityId, x)).ToList();
				}

				tte.EngagementStartDate = request.EngagementStartDate;
				tte.ActivityTypes = activityTypes;
				tte.Notes  = request.Notes;
				tte.UpdatedAt = DateTime.UtcNow;

				await _context.SaveChangesAsync();

				var concernCreatedNotification = new TargetedTrustEngagementUpdatedNotification() { Id = command.TargetedTrustEngagementId, CaseId = command.ConcernsCaseUrn, };
				await _mediator.Publish(concernCreatedNotification);

				return new UpdateTargetedTrustEngagementResponse()
				{
					ConcernsCaseUrn = command.ConcernsCaseUrn,
					TargetedTrustEngagementId = command.TargetedTrustEngagementId
				};
			}
		}

		public class TargetedTrustEngagementUpdatedNotification : INotification
		{
			public int Id { get; set; }
			public int CaseId { get; set; }
		}

		public class TargetedTrustEngagementUpdatedCaseUpdatedHandler : INotificationHandler<TargetedTrustEngagementUpdatedNotification>
		{
			private readonly ConcernsDbContext _context;
			public TargetedTrustEngagementUpdatedCaseUpdatedHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task Handle(TargetedTrustEngagementUpdatedNotification notification, CancellationToken cancellationToken)
			{
				ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.CaseId, cancellationToken: cancellationToken);
				TargetedTrustEngagementCase tte = await _context.TargetedTrustEngagements.SingleOrDefaultAsync(f => f.Id == notification.Id, cancellationToken: cancellationToken);
				cc.CaseLastUpdatedAt = tte.UpdatedAt;
				await _context.SaveChangesAsync();
			}
		}
	}
}
