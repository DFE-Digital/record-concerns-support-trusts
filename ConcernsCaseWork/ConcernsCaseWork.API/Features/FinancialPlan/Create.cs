using ConcernsCaseWork.API.RequestModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static Azure.Core.HttpHeader;

namespace ConcernsCaseWork.API.Features.FinancialPlan
{
	public class Create
	{
		public class Command : IRequest<long>
		{
			[Required]
			public int CaseUrn { get; set; }

			[StringLength(300)]
			public string Name { get; set; }
			public long? StatusId { get; set; }
			public DateTime? DatePlanRequested { get; set; }
			public DateTime? DateViablePlanReceived { get; set; }
			public DateTime CreatedAt { get; set; }

			[StringLength(300)]
			public string CreatedBy { get; set; }
			public DateTime UpdatedAt { get; set; }
			public DateTime? ClosedAt { get; set; }

			[StringLength(2000)]
			public string Notes { get; set; }
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

			public async Task<long> Handle(Command request, CancellationToken cancellationToken)
			{
				FinancialPlanCase fp = new FinancialPlanCase()
				{
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

				_context.FinancialPlanCases.Add(fp);

				await _context.SaveChangesAsync();
				
				var createdNotification = new FinancialPlanCreatedNotification() { Id = fp.Id, CaseId = fp.CaseUrn, };
				await _mediator.Publish(createdNotification);

				return fp.Id;
			}
		}

		//ToDo: BB 03/08/2023 Once we have more events decide on specific folder structure to organise events and handles
		public class FinancialPlanCreatedNotification : INotification
		{
			public long Id { get; set; }
			public int CaseId { get; set; }
		}

		public class FinancialPlanCreatedCaseUpdatedHandler : INotificationHandler<FinancialPlanCreatedNotification>
		{
			private readonly ConcernsDbContext _context;
			public FinancialPlanCreatedCaseUpdatedHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task Handle(FinancialPlanCreatedNotification notification, CancellationToken cancellationToken)
			{
				ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.CaseId, cancellationToken: cancellationToken);
				FinancialPlanCase cr = await _context.FinancialPlanCases.SingleOrDefaultAsync(f => f.Id == notification.Id, cancellationToken: cancellationToken);
				cc.CaseLastUpdatedAt = cr.CreatedAt;
				await _context.SaveChangesAsync();
			}
		}

	}
}
