using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.Decision.Outcome
{
	using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;
	using System.ComponentModel.DataAnnotations;
	using System.Globalization;
	using ConcernsCaseWork.API.Exceptions;

	public class Create
	{
		public class DecisionOutcomeModel
		{
			public DecisionOutcomeModel()
			{
				BusinessAreasConsulted = new List<Contracts.Decisions.Outcomes.DecisionOutcomeBusinessArea>();
			}

			public int DecisionOutcomeId { get; set; }

			public int DecisionId { get; set; }

			[Required]
			[EnumDataType(typeof(API.Contracts.Decisions.Outcomes.DecisionOutcomeStatus), ErrorMessage = "Select a decision outcome status")]
			public API.Contracts.Decisions.Outcomes.DecisionOutcomeStatus Status { get; set; }

			[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "The total amount requested must be zero or greater")]
			public decimal? TotalAmount { get; set; }

			public DateTimeOffset? DecisionMadeDate { get; set; }

			public DateTimeOffset? DecisionEffectiveFromDate { get; set; }

			[EnumDataType(typeof(API.Contracts.Decisions.Outcomes.DecisionOutcomeAuthorizer))]
			public API.Contracts.Decisions.Outcomes.DecisionOutcomeAuthorizer? Authorizer { get; set; }

			public List<Contracts.Decisions.Outcomes.DecisionOutcomeBusinessArea> BusinessAreasConsulted { get; set; }

			public DateTimeOffset CreatedAt { get; set; }
			public DateTimeOffset UpdatedAt { get; set; }
		}

		public class Command : IRequest<CommandResult>
		{
			public int ConcernsCaseUrn { get; }
			public int DecisionId { get; }
			public DecisionOutcomeModel Model { get; set; }

			public Command(int concernsCaseUrn, int DecisionId, DecisionOutcomeModel model)
			{
				this.ConcernsCaseUrn = concernsCaseUrn;
				this.DecisionId = DecisionId;
				this.Model = model;
			}

		}

		public class CommandResult
		{
			public int ConcernsCaseUrn { get; set; }

			public int DecisionId { get; set; }

			public int DecisionOutcomeId { get; set; }

			public Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome Outcome { get; set; }
		}

		public class CommandHandler : IRequestHandler<Command, CommandResult>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;


			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
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
				var outcome = new DecisionOutcome()
				{
					DecisionId = request.DecisionId,
					Status = request.Model.Status,
					TotalAmount = request.Model.TotalAmount,
					DecisionMadeDate = request.Model.DecisionMadeDate,
					DecisionEffectiveFromDate = request.Model.DecisionEffectiveFromDate,
					Authorizer = request.Model.Authorizer,
					BusinessAreasConsulted = request.Model.BusinessAreasConsulted.Select(b => new DecisionOutcomeBusinessAreaMapping()
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

				return new CommandResult()
				{
					ConcernsCaseUrn = request.ConcernsCaseUrn,
					DecisionId = request.DecisionId,
					DecisionOutcomeId = result.Entity.DecisionOutcomeId,
					Outcome = result.Entity
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
                DecisionOutcome decisionOutcome = await _context.DecisionOutcomes.SingleOrDefaultAsync(f => f.DecisionOutcomeId == notification.Id, cancellationToken: cancellationToken);
				cc.CaseLastUpdatedAt = decisionOutcome.CreatedAt.DateTime;
				await _context.SaveChangesAsync();
			}
		}
	}
}
