using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class UpdateVisitDates
	{
		public class Command : IRequest<int>
		{
			public int srmaId { get; private set; }
			public DateTime? StartDate { get; private set; }
			public DateTime? EndDate { get; private set; }

			private Command(int id, DateTime? startDate, DateTime? endDate)
			{
				this.srmaId = id;
				this.StartDate = startDate;
				this.EndDate = endDate;
			}

			public static Command Create(int id, DateTime? startDate, DateTime? endDate)
			{
				return new Command(id, startDate,endDate);
			}
		}

		public class CommandHandler : IRequestHandler<Command, int>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;

			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task<int> Handle(Command request, CancellationToken cancellationToken)
			{
				var srma = await _context.SRMACases.FindAsync(request.srmaId);

				if (srma == null)
				{
					throw new NotFoundException($"SRMA {request.srmaId}");
				}

				srma.StartDateOfVisit = request.StartDate;
				srma.EndDateOfVisit = request.EndDate;

				srma.UpdatedAt = DateTime.Now;

				await _context.SaveChangesAsync();

				var updatedNotification = new SRMAUpdatedNotification() { Id = srma.Id, CaseId = srma.CaseUrn, };
				await _mediator.Publish(updatedNotification);

				return srma.Id;
			}
		}
	}

}
