using MediatR;
using ConcernsCaseWork.Data;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.Decision
{
	using ConcernsCaseWork.API.Contracts.Decisions;
	using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
	using ConcernsCaseWork.Data.Models;
	using Microsoft.EntityFrameworkCore;

	public class Update
	{
		public class DecisionModel
		{
			public DecisionTypeQuestion[] DecisionTypes { get; set; }

			public decimal TotalAmountRequested { get; set; }

			public string SupportingNotes { get; set; }

			public DateTimeOffset? ReceivedRequestDate { get; set; }

			public string SubmissionDocumentLink { get; set; }

			public bool? SubmissionRequired { get; set; }

			public bool? RetrospectiveApproval { get; set; }

			public string CrmCaseNumber { get; set; }
		}

		public class CommandResult
		{
			public int ConcernsCaseUrn { get; set; }
			public int DecisionId { get; set; }
		}

		public class Command : IRequest<CommandResult>
		{
			public int ConcernsCaseUrn { get; }
			public int DecisionId { get; }

			public DecisionModel Model { get; set; }

			public Command(int concernsCaseUrn, int DecisionId, DecisionModel model)
			{
				this.ConcernsCaseUrn = concernsCaseUrn;
				this.DecisionId = DecisionId;
				this.Model = model;
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
						.SingleOrDefaultAsync(c => c.Id == request.ConcernsCaseUrn);

				if (concernsCase == null)
				{
					throw new InvalidOperationException($"Concerns Case {request.ConcernsCaseUrn} not found");
				}

				var decisionTypes = request.Model.DecisionTypes.Select(x => new DecisionType((ConcernsCaseWork.Data.Enums.Concerns.DecisionType)x.Id, (API.Contracts.Decisions.DrawdownFacilityAgreed?)x.DecisionDrawdownFacilityAgreedId, (API.Contracts.Decisions.FrameworkCategory?)x.DecisionFrameworkCategoryId)).Distinct().ToArray();

				var updatedDecision = Decision.CreateNew(request.Model.CrmCaseNumber, request.Model.RetrospectiveApproval, request.Model.SubmissionRequired, request.Model.SubmissionDocumentLink, request.Model.ReceivedRequestDate.Value, decisionTypes, request.Model.TotalAmountRequested, request.Model.SupportingNotes, DateTime.Now);

				concernsCase.UpdateDecision(request.DecisionId, updatedDecision, updatedDecision.UpdatedAt);

				await _context.SaveChangesAsync();

				var concernCreatedNotification = new DecisionUpdatedNotification() { Id = request.DecisionId, CaseId = request.ConcernsCaseUrn, };
				await _mediator.Publish(concernCreatedNotification);

				return new CommandResult()
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
