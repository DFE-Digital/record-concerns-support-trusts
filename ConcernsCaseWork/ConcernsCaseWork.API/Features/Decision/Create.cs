using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.Decision
{
	using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
	using System.Globalization;

	public class Create
	{
		public class Command : IRequest<CommandResult>
		{
			public int ConcernsCaseUrn { get; set; } 
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
				var now = DateTimeOffset.Now;
				var exp = _context.ConcernsCase
							.Include(x => x.Decisions)
							.ThenInclude(x => x.DecisionTypes)
							.Include(x => x.Decisions)
							.ThenInclude(x => x.Outcome)
							.ThenInclude(x => x.BusinessAreasConsulted);

				var concernsCase = exp.FirstOrDefault(c => c.Urn == request.ConcernsCaseUrn);

				var decisionTypes = request.DecisionTypes.Select(x => new DecisionType((ConcernsCaseWork.Data.Enums.Concerns.DecisionType)x.Id, (API.Contracts.Decisions.DrawdownFacilityAgreed?)x.DecisionDrawdownFacilityAgreedId, (API.Contracts.Decisions.FrameworkCategory?)x.DecisionFrameworkCategoryId)).Distinct().ToArray();

				var decision = Decision.CreateNew(request.CrmCaseNumber, request.RetrospectiveApproval,
					request.SubmissionRequired, request.SubmissionDocumentLink, (DateTimeOffset)request.ReceivedRequestDate,
					decisionTypes, request.TotalAmountRequested, request.SupportingNotes, now);


				concernsCase.AddDecision(decision, now);

				await _context.SaveChangesAsync();

				var DecisionCreatedNotification = new DecisionCreatedNotification() { Id = decision.DecisionId, CaseId = concernsCase.Id };
				await _mediator.Publish(DecisionCreatedNotification);


				return new CommandResult()
				{
					ConcernsCaseUrn = decision.ConcernsCaseId,
					DecisionId = decision.DecisionId
				};
			}
		}

		//ToDo: BB 25/07/2023 Once we have more events decide on specific folder structure to organise events and handles
		public class DecisionCreatedNotification : INotification
		{
			public int Id { get; set; }
			public int CaseId { get; set; }
		}

		public class DecisionCreatedCaseUpdatedHandler : INotificationHandler<DecisionCreatedNotification>
		{
			private readonly ConcernsDbContext _context;
			public DecisionCreatedCaseUpdatedHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task Handle(DecisionCreatedNotification notification, CancellationToken cancellationToken)
			{
				ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.CaseId, cancellationToken: cancellationToken);
				Decision decision = await _context.Decisions.SingleOrDefaultAsync(f => f.DecisionId == notification.Id, cancellationToken: cancellationToken);
				cc.CaseLastUpdatedAt = decision.CreatedAt.DateTime;
				await _context.SaveChangesAsync();
			}
		}
	}
}
