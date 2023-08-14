using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class UpdateOfferedDate
	{
		public class Command : IRequest<int>
		{
			public int srmaId { get; private set; }
			public DateTime OfferedDate { get; private set; }

			private Command(int id, DateTime offeredDateTime)
			{
				this.srmaId = id;
				this.OfferedDate = offeredDateTime;
			}

			public static Command Create(int id, DateTime offeredDateTime)
			{
				return new Command(id, offeredDateTime);
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

				srma.DateOffered = request.OfferedDate;
				srma.UpdatedAt = DateTime.Now;

				await _context.SaveChangesAsync();

				var updatedNotification = new SRMAUpdatedNotification() { Id = srma.Id, CaseId = srma.CaseUrn, };
				await _mediator.Publish(updatedNotification);

				return srma.Id;
			}
		}
	}
}
