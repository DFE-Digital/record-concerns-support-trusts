using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.TargetedTrustEngagement
{
	using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
	using ConcernsCaseWork.API.Exceptions;

	public class Create
	{
		public class Command : IRequest<CreateTargetedTrustEngagementResponse>
		{
			public CreateTargetedTrustEngagementRequest Request { get; }

			public Command(CreateTargetedTrustEngagementRequest request)
			{
				Request = request;
			}
		}

		public class CommandHandler : IRequestHandler<Command, CreateTargetedTrustEngagementResponse>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;


			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task<CreateTargetedTrustEngagementResponse> Handle(Command command, CancellationToken cancellationToken)
			{
				var request = command.Request;

				var exp = _context.ConcernsCase;

				var concernsCase = exp.FirstOrDefault(c => c.Urn == request.CaseUrn);

				if (concernsCase == null)
				{
					throw new NotFoundException($"Concerns case {request.CaseUrn}");
				}

				var activityTypes = new List<TargetedTrustEngagementType>();

				if (request.ActivityId.HasValue && request.ActivityTypes.Length == 0)
				{
					var at = new TargetedTrustEngagementType((TargetedTrustEngagementActivity)request.ActivityId, null);
					activityTypes.Add(at);
				}
				else
				{
					activityTypes = request.ActivityTypes.Select(x => new TargetedTrustEngagementType((TargetedTrustEngagementActivity)request.ActivityId, x)).ToList();
				}

				var now = DateTime.UtcNow;

				var tte = new TargetedTrustEngagementCase()
				{
					CaseUrn = request.CaseUrn,
					EngagementStartDate = request.EngagementStartDate,
					ActivityTypes = activityTypes,
					Notes = request.Notes,
					CreatedAt = now,
					UpdatedAt = now,
					CreatedBy = request.CreatedBy
				};

				_context.TargetedTrustEngagements.Add(tte);
				await _context.SaveChangesAsync();

				var TTECreatedNotification = new TargetedTrustEngagementCreatedNotification() { Id = tte.Id, CaseId = concernsCase.Id };
				await _mediator.Publish(TTECreatedNotification);

				return new CreateTargetedTrustEngagementResponse()
				{
					ConcernsCaseUrn = tte.CaseUrn,
					TargetedTrustEngagementId = tte.Id
				};
			}
		}

		public class TargetedTrustEngagementCreatedNotification : INotification
		{
			public int Id { get; set; }
			public int CaseId { get; set; }
		}

		public class TargetedTrustEngagementCreatedCaseUpdatedHandler : INotificationHandler<TargetedTrustEngagementCreatedNotification>
		{
			private readonly ConcernsDbContext _context;
			public TargetedTrustEngagementCreatedCaseUpdatedHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task Handle(TargetedTrustEngagementCreatedNotification notification, CancellationToken cancellationToken)
			{
				ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.CaseId, cancellationToken: cancellationToken);
				TargetedTrustEngagementCase tte = await _context.TargetedTrustEngagements.SingleOrDefaultAsync(f => f.Id == notification.Id, cancellationToken: cancellationToken);
				cc.CaseLastUpdatedAt = tte.CreatedAt;
				await _context.SaveChangesAsync();
			}
		}
	}
}
