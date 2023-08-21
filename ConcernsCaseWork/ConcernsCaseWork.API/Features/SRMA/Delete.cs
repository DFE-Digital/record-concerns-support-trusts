using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class Delete
	{
		public class Command : IRequest
		{
			public int srmaId { get; set; }
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
				var srma = await _context.SRMACases.FindAsync(request.srmaId);

				if (srma == null)
				{
					throw new NotFoundException($"Not Found: SRMA with id {request.srmaId}");
				}

				srma.DeletedAt = DateTime.Now;

				await _context.SaveChangesAsync();
			}
		}
	}
}
