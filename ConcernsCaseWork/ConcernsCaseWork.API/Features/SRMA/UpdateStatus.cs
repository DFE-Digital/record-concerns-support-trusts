using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class UpdateStatus
	{
		public class Command : IRequest<int>
		{
			public int srmaId { get; private set; }
			public SRMAStatus status { get; private set; }

			private Command(int id, SRMAStatus status)
			{
				this.srmaId = id;
				this.status = status;
			}

			public static Command Create(int id, SRMAStatus status)
			{
				return new Command(id, status);
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

				srma.StatusId = (int)request.status;
				srma.UpdatedAt = DateTime.Now;

				await _context.SaveChangesAsync();

				var updatedNotification = new SRMAUpdatedNotification() { Id = srma.Id, CaseId = srma.CaseUrn, };
				await _mediator.Publish(updatedNotification);

				return srma.Id;
			}
		}
	}
}
