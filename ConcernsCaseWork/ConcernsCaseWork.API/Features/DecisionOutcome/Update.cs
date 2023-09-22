using MediatR;
using ConcernsCaseWork.Data;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.Decision.Outcome
{
	using ConcernsCaseWork.API.Contracts.Decisions;
	using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
	using ConcernsCaseWork.Data.Models;
	using Microsoft.EntityFrameworkCore;
	using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;
	using ConcernsCaseWork.API.Exceptions;
	using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;

	public class Update
	{
		public class Command : IRequest<UpdateDecisionOutcomeResponse>
		{
			public int ConcernsCaseUrn { get; }
			public int DecisionId { get; }

			public UpdateDecisionOutcomeRequest Request { get; set; }

			public Command(int concernsCaseUrn, int DecisionId, UpdateDecisionOutcomeRequest request)
			{
				this.ConcernsCaseUrn = concernsCaseUrn;
				this.DecisionId = DecisionId;
				this.Request = request;
			}
		}

		public class CommandHandler : IRequestHandler<Command, UpdateDecisionOutcomeResponse>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;

			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task<UpdateDecisionOutcomeResponse> Handle(Command request, CancellationToken cancellationToken)
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

				if (decision.Outcome == null)
				{
					throw new NotFoundException($"Not Found: Decision with id {request.DecisionId} does not have an outcome, Case {request.ConcernsCaseUrn}");
				}

				decision.Outcome.Status = request.Request.Status;
				decision.Outcome.TotalAmount = request.Request.TotalAmount;
				decision.Outcome.DecisionMadeDate = request.Request.DecisionMadeDate;
				decision.Outcome.DecisionEffectiveFromDate = request.Request.DecisionEffectiveFromDate;
				decision.Outcome.Authorizer = request.Request.Authorizer;
				decision.Outcome.BusinessAreasConsulted = request.Request.BusinessAreasConsulted.Select(b => new DecisionOutcomeBusinessAreaMapping()
				{
					DecisionOutcomeBusinessId = b
				}).ToList();

				var now = DateTimeOffset.Now;
				decision.UpdatedAt = now;
				decision.Outcome.UpdatedAt = now;

				await _context.SaveChangesAsync();

				var concernCreatedNotification = new DecisionOutcomeUpdatedNotification() { Id = decision.Outcome.DecisionOutcomeId, ConcernsCaseUrn = request.ConcernsCaseUrn, };
				await _mediator.Publish(concernCreatedNotification);

				return new UpdateDecisionOutcomeResponse()
				{
					ConcernsCaseUrn = request.ConcernsCaseUrn,
					DecisionId = request.DecisionId,
					DecisionOutcomeId = decision.Outcome.DecisionOutcomeId
				};
			}
		}

		//ToDo: BB 25/07/2023 Once we have more events decide on specific folder structure to organise events and handles
		public class DecisionOutcomeUpdatedNotification : INotification
		{
			public int Id { get; set; }
			public int ConcernsCaseUrn { get; set; }
		}

		public class DecisionOutcomeUpdatedUpdatedHandler : INotificationHandler<DecisionOutcomeUpdatedNotification>
		{
			private readonly ConcernsDbContext _context;
			public DecisionOutcomeUpdatedUpdatedHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task Handle(DecisionOutcomeUpdatedNotification notification, CancellationToken cancellationToken)
			{
				ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.ConcernsCaseUrn, cancellationToken: cancellationToken);
				var decisionOutcome = await _context.DecisionOutcomes.SingleOrDefaultAsync(f => f.DecisionOutcomeId == notification.Id, cancellationToken: cancellationToken);
				cc.CaseLastUpdatedAt = decisionOutcome.UpdatedAt.DateTime;
				await _context.SaveChangesAsync();
			}
		}
	}
}
