using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.RequestModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

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

				var existingFinancialPlan = await _context.FinancialPlanCases.SingleOrDefaultAsync(e => e.Id == request.Id && e.CaseUrn == request.CaseUrn);

				if (existingFinancialPlan == null)
				{
					throw new NotFoundException($"Case {request.CaseUrn} financial plan {request.Id}");
				}

				existingFinancialPlan.Name = request.Name;
				existingFinancialPlan.ClosedAt = request.ClosedAt;
				existingFinancialPlan.CreatedAt = request.CreatedAt;
				existingFinancialPlan.CreatedBy = request.CreatedBy;
				existingFinancialPlan.DatePlanRequested = request.DatePlanRequested;
				existingFinancialPlan.DateViablePlanReceived = request.DateViablePlanReceived;
				existingFinancialPlan.Notes = request.Notes;
				existingFinancialPlan.StatusId = request.StatusId;
				existingFinancialPlan.UpdatedAt = request.UpdatedAt;

				var tracked = _context.Update(existingFinancialPlan);
				await _context.SaveChangesAsync();

				var updatedNotification = new FinancialPlanUpdatedNotification() { Id = existingFinancialPlan.Id, CaseId = existingFinancialPlan.CaseUrn, };
				await _mediator.Publish(updatedNotification);

				return existingFinancialPlan.Id;
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
