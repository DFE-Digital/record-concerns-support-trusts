using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.TargetedTrustEngagement
{
	using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
	using ConcernsCaseWork.API.Exceptions;
	using ConcernsCaseWork.Data.Exceptions;
	using ConcernsCaseWork.Data.Models;
	using Microsoft.EntityFrameworkCore;

	public class Close
	{
		public class Command : IRequest<CloseTargetedTrustEngagementResponse>
		{
			public int CaseUrn { get; }
			public int TargetedTrustEngagementId { get; }

			public CloseTargetedTrustEngagementRequest Request { get; set; }

			public Command(int concernsCaseUrn, int TargetedTrustEngagementId, CloseTargetedTrustEngagementRequest request)
			{
				this.CaseUrn = concernsCaseUrn;
				this.TargetedTrustEngagementId = TargetedTrustEngagementId;
				this.Request = request;
			}
		}

		public class CommandHandler : IRequestHandler<Command, CloseTargetedTrustEngagementResponse>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;

			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task<CloseTargetedTrustEngagementResponse> Handle(Command command, CancellationToken cancellationToken)
			{
				var concernsCase = await _context.ConcernsCase
						.SingleOrDefaultAsync(c => c.Id == command.CaseUrn);

				if (concernsCase == null)
				{
					throw new NotFoundException($"Concerns Case {command.CaseUrn} not found");
				}

				var request = command.Request;

				try
				{
					var now = DateTime.UtcNow;
					var tte = _context.TargetedTrustEngagements.FirstOrDefault(t => t.Id == command.TargetedTrustEngagementId);

					tte.EngagementOutcomeId = (int)request.OutcomeId;
					tte.EngagementEndDate = request.EngagementEndDate;
					tte.Notes = request.Notes;
					tte.UpdatedAt = now;
					tte.ClosedAt = now;

					await _context.SaveChangesAsync();
				}
				catch (EntityNotFoundException ex)
				{
					throw new NotFoundException(ex.Message);
				}
				catch (StateChangeNotAllowedException ex)
				{
					throw new OperationNotCompletedException(ex.Message);
				}

				var concernCreatedNotification = new TargetedTrustEngagementUpdatedNotification() { Id = command.TargetedTrustEngagementId, CaseId = command.CaseUrn };
				await _mediator.Publish(concernCreatedNotification);

				return new CloseTargetedTrustEngagementResponse()
				{
					CaseUrn = command.CaseUrn,
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
