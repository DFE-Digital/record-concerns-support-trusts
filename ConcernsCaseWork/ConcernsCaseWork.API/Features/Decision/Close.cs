using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.Decision
{
	using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
	using ConcernsCaseWork.API.Exceptions;
	using ConcernsCaseWork.Data.Exceptions;
	using ConcernsCaseWork.Data.Models;
	using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
	using Microsoft.EntityFrameworkCore;

	public class Close
	{
		public class CommandResult
		{
			public int CaseUrn { get; set; }
			public int DecisionId { get; set; }
		}

		public class Command : IRequest<CommandResult>
		{
			public int CaseUrn { get; }
			public int DecisionId { get; }

			public CloseDecisionRequest Request { get; set; }

			public Command(int concernsCaseUrn, int DecisionId, CloseDecisionRequest request)
			{
				this.CaseUrn = concernsCaseUrn;
				this.DecisionId = DecisionId;
				this.Request = request;
			}
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
				var concernsCase = await _context.ConcernsCase
						.Include(x => x.Decisions)
						.ThenInclude(x => x.DecisionTypes)
						.Include(x => x.Decisions)
						.ThenInclude(x => x.Outcome)
						.ThenInclude(x => x.BusinessAreasConsulted)
						.SingleOrDefaultAsync(c => c.Id == request.CaseUrn);

				if (concernsCase == null)
				{
					throw new NotFoundException($"Concerns Case {request.CaseUrn} not found");
				}

				try
				{
					concernsCase.CloseDecision(request.DecisionId, request.Request.SupportingNotes, DateTime.Now);
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

				var concernCreatedNotification = new DecisionUpdatedNotification() { Id = request.DecisionId, CaseId = request.CaseUrn };
				await _mediator.Publish(concernCreatedNotification);

				return new CommandResult()
				{
					CaseUrn = request.CaseUrn,
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
