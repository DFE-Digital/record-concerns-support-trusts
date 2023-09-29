using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.FinancialPlan
{
	public class Delete
	{
		public class Command : IRequest
		{
			[Required]
			public long Id { get; set; }
			[Required]
			public int CaseUrn { get; set; }
		}

		public class CommandHandler : IRequestHandler<Command>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;

			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task Handle(Command request, CancellationToken cancellationToken)
			{
				var financialPlan = await _context.FinancialPlanCases.FirstOrDefaultAsync(fp => fp.Id == request.Id);

				if (financialPlan == null)
				{
					throw new NotFoundException($"Not Found: Financial Plan with id {request.Id}");
				}

				financialPlan.DeletedAt = DateTime.Now;

				await _context.SaveChangesAsync();
			}
		}

	}
}
