using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class UpdateDateClosed
	{
		public class Command : IRequest<int>
		{
			public int srmaId { get; set; }
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

				srma.ClosedAt = DateTime.Now;
				srma.UpdatedAt = DateTime.Now;

				await _context.SaveChangesAsync();

				var updatedNotification = new SRMAUpdatedNotification() { Id = srma.Id, CaseId = srma.CaseUrn, };
				await _mediator.Publish(updatedNotification);

				return srma.Id;
			}
		}
	}
}
