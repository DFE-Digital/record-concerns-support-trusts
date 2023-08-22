using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.NTIUnderConsideration
{
	public class Delete
	{
		public class Command : IRequest
		{
			public long underConsiderationId { get; set; }
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
				var nTIUnderConsideration = await _context.NTIUnderConsiderations.FindAsync(request.underConsiderationId);

				if (nTIUnderConsideration == null)
				{
					throw new NotFoundException($"Not Found: NTI Under Consideration with id {request.underConsiderationId}");
				}

				nTIUnderConsideration.DeletedAt = DateTime.Now;

				await _context.SaveChangesAsync();
			}
		}
	}
}
