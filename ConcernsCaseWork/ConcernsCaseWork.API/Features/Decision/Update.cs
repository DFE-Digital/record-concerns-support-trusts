using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.Decision
{
	using ConcernsCaseWork.API.Contracts.Decisions;
	using ConcernsCaseWork.API.Exceptions;
	using ConcernsCaseWork.Data.Models;
	using ConcernsCaseWork.Data.Models.Decisions;
	using Microsoft.EntityFrameworkCore;

	public class Update
	{
		public class Command : IRequest<UpdateDecisionResponse>
		{
			public int ConcernsCaseUrn { get; }
			public int DecisionId { get; }

			public UpdateDecisionRequest Request { get; set; }

			public Command(int concernsCaseUrn, int DecisionId, UpdateDecisionRequest request)
			{
				this.ConcernsCaseUrn = concernsCaseUrn;
				this.DecisionId = DecisionId;
				this.Request = request;
			}
		}

		public class CommandHandler : IRequestHandler<Command, UpdateDecisionResponse>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;

			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task<UpdateDecisionResponse> Handle(Command request, CancellationToken cancellationToken)
			{
				var concernsCase = await _context.ConcernsCase
						.Include(x => x.Decisions)
						.ThenInclude(x => x.DecisionTypes)
						.Include(x => x.Decisions)
						.SingleOrDefaultAsync(c => c.Id == request.ConcernsCaseUrn);

				if (concernsCase == null)
				{
					throw new NotFoundException($"Concerns case {request.ConcernsCaseUrn}");
				}

				var decision = concernsCase.Decisions.SingleOrDefault(d => d.DecisionId == request.DecisionId);

				if (decision == null)
				{
					throw new NotFoundException($"Decision {request.DecisionId}");
				}

				var decisionTypes = request.Request.DecisionTypes.Select(x => new Data.Models.Decisions.DecisionType(x.Id, x.DecisionDrawdownFacilityAgreedId, x.DecisionFrameworkCategoryId)).Distinct().ToArray();

				var updatedDecision = Decision.CreateNew(new DecisionParameters()
				{
					CrmCaseNumber = request.Request.CrmCaseNumber,
					HasCrmCase = request.Request.HasCrmCase,
					RetrospectiveApproval = request.Request.RetrospectiveApproval,
					SubmissionRequired = request.Request.SubmissionRequired,
					SubmissionDocumentLink = request.Request.SubmissionDocumentLink,
					ReceivedRequestDate = request.Request.ReceivedRequestDate.Value,
					DecisionTypes = decisionTypes,
					TotalAmountRequested = request.Request.TotalAmountRequested,
					SupportingNotes = request.Request.SupportingNotes,
					Now = DateTime.Now
				});

				concernsCase.UpdateDecision(request.DecisionId, updatedDecision, updatedDecision.UpdatedAt);

				await _context.SaveChangesAsync();

				var concernCreatedNotification = new DecisionUpdatedNotification() { Id = request.DecisionId, CaseId = request.ConcernsCaseUrn, };
				await _mediator.Publish(concernCreatedNotification);

				return new UpdateDecisionResponse()
				{
					ConcernsCaseUrn = request.ConcernsCaseUrn,
					DecisionId = request.DecisionId
				};
			}
		}

		//ToDo: BB 25/07/2023 Once we have more events decide on specific folder structure to organise events and handles
		public class DecisionUpdatedNotification : INotification
		{
			public int Id { get; set; }
			public int CaseId { get; set; }
		}

		public class DecisionUpdatedCaseUpdatedHandler : INotificationHandler<DecisionUpdatedNotification>
		{
			private readonly ConcernsDbContext _context;
			public DecisionUpdatedCaseUpdatedHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task Handle(DecisionUpdatedNotification notification, CancellationToken cancellationToken)
			{
				ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.CaseId, cancellationToken: cancellationToken);
				Decision decision = await _context.Decisions.SingleOrDefaultAsync(f => f.DecisionId == notification.Id, cancellationToken: cancellationToken);
				cc.CaseLastUpdatedAt = decision.UpdatedAt.DateTime;
				await _context.SaveChangesAsync();
			}
		}
	}
}
