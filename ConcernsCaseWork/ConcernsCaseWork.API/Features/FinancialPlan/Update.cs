using ConcernsCaseWork.API.RequestModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.FinancialPlan
{
	public class Update
	{

		public class Command : IRequest<long>
		{
			public PatchFinancialPlanRequest Request { get; }

			public Command(PatchFinancialPlanRequest request)
			{
				Request = request;
			}
		}

		public class CommandHandler : IRequestHandler<Command, long>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;

			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task<long> Handle(Command command, CancellationToken cancellationToken)
			{
				var request = command.Request;
				var fp = new FinancialPlanCase
				{
					Id = request.Id,
					CaseUrn = request.CaseUrn,
					Name = request.Name,
					ClosedAt = request.ClosedAt,
					CreatedAt = request.CreatedAt,
					CreatedBy = request.CreatedBy,
					DatePlanRequested = request.DatePlanRequested,
					DateViablePlanReceived = request.DateViablePlanReceived,
					Notes = request.Notes,
					StatusId = request.StatusId,
					UpdatedAt = request.UpdatedAt,
				};

				var tracked = _context.Update(fp);
				await _context.SaveChangesAsync();

				var updatedNotification = new FinancialPlanUpdatedNotification() { Id = fp.Id, CaseId = fp.CaseUrn, };
				await _mediator.Publish(updatedNotification);

				return fp.Id;
			}
		}


		//ToDo: BB 03/08/2023 Once we have more events decide on specific folder structure to organise events and handles
		public class FinancialPlanUpdatedNotification : INotification
		{
			public long Id { get; set; }
			public int CaseId { get; set; }
		}

		public class FinancialPlanCreatedCaseUpdatedHandler : INotificationHandler<FinancialPlanUpdatedNotification>
		{
			private readonly ConcernsDbContext _context;
			public FinancialPlanCreatedCaseUpdatedHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task Handle(FinancialPlanUpdatedNotification notification, CancellationToken cancellationToken)
			{
				ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.CaseId, cancellationToken: cancellationToken);
				FinancialPlanCase cr = await _context.FinancialPlanCases.SingleOrDefaultAsync(f => f.Id == notification.Id, cancellationToken: cancellationToken);
				cc.CaseLastUpdatedAt = cr.UpdatedAt;
				await _context.SaveChangesAsync();
			}
		}
	}
}
