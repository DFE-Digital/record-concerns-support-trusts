using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.Decision.Outcome
{
	using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
	using ConcernsCaseWork.API.Exceptions;
	using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;

	public class Create
	{
		public class Command : IRequest<CreateDecisionOutcomeResponse>
		{
			public int ConcernsCaseUrn { get; }
			public int DecisionId { get; }
			public CreateDecisionOutcomeRequest Request { get; set; }

			public Command(int concernsCaseUrn, int DecisionId, CreateDecisionOutcomeRequest request)
			{
				this.ConcernsCaseUrn = concernsCaseUrn;
				this.DecisionId = DecisionId;
				this.Request = request;
			}

		}

		public class CommandHandler : IRequestHandler<Command, CreateDecisionOutcomeResponse>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;


			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task<CreateDecisionOutcomeResponse> Handle(Command request, CancellationToken cancellationToken)
			{
				var concernCase = await _context.ConcernsCase
					.Include(x => x.Decisions)
					.ThenInclude(x => x.DecisionTypes)
					.Include(x => x.Decisions)
					.ThenInclude(x => x.Outcome)
					.ThenInclude(x => x.BusinessAreasConsulted)
					.FirstOrDefaultAsync(o => o.Urn == request.ConcernsCaseUrn);

				if (concernCase == null)
				{
					throw new NotFoundException($"Not Found: Concern with id {request.ConcernsCaseUrn}");
				}

				var decision = concernCase.Decisions.FirstOrDefault(d => d.DecisionId == request.DecisionId);

				if (decision == null)
				{
					throw new NotFoundException($"Not Found: Decision with id {request.DecisionId}, Case {request.ConcernsCaseUrn}");
				}

				if (decision.Outcome != null)
				{
					throw new ResourceConflictException($"Conflict: Decision with id {request.DecisionId} already has an outcome, Case {request.ConcernsCaseUrn}");
				}

				var now = DateTime.Now;
				var outcome = new Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome()
				{
					DecisionId = request.DecisionId,
					Status = request.Request.Status,
					TotalAmount = request.Request.TotalAmount,
					DecisionMadeDate = request.Request.DecisionMadeDate,
					DecisionEffectiveFromDate = request.Request.DecisionEffectiveFromDate,
					Authorizer = request.Request.Authorizer,
					BusinessAreasConsulted = request.Request.BusinessAreasConsulted.Select(b => new DecisionOutcomeBusinessAreaMapping()
					{
						DecisionOutcomeBusinessId = b
					}).ToList(),
					CreatedAt = now,
					UpdatedAt = now
				};

				var result = _context.DecisionOutcomes.Add(outcome);
				await _context.SaveChangesAsync(cancellationToken);

				var decisionCreatedNotification = new DecisionOutcomeCreatedNotification() { Id = result.Entity.DecisionOutcomeId, CaseId = request.ConcernsCaseUrn };
				await _mediator.Publish(decisionCreatedNotification);

				return new CreateDecisionOutcomeResponse()
				{
					ConcernsCaseUrn = request.ConcernsCaseUrn,
					DecisionId = request.DecisionId,
					DecisionOutcomeId = result.Entity.DecisionOutcomeId,
				};
			}
		}

		//ToDo: BB 25/07/2023 Once we have more events decide on specific folder structure to organise events and handles
		public class DecisionOutcomeCreatedNotification : INotification
		{
			public int Id { get; set; }
			public int CaseId { get; set; }
		}

		public class DecisionOutcomeCreatedCaseUpdatedHandler : INotificationHandler<DecisionOutcomeCreatedNotification>
		{
			private readonly ConcernsDbContext _context;
			public DecisionOutcomeCreatedCaseUpdatedHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task Handle(DecisionOutcomeCreatedNotification notification, CancellationToken cancellationToken)
			{
				ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.CaseId, cancellationToken: cancellationToken);
                var decisionOutcome = await _context.DecisionOutcomes.SingleOrDefaultAsync(f => f.DecisionOutcomeId == notification.Id, cancellationToken: cancellationToken);
				cc.CaseLastUpdatedAt = decisionOutcome.CreatedAt.DateTime;
				await _context.SaveChangesAsync();
			}
		}
	}
}
