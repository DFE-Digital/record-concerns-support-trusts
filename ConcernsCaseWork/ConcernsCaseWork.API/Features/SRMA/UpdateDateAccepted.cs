using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data;
using MediatR;
using static ConcernsCaseWork.API.Features.SRMA.Create;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class UpdateDateAccepted
	{
		public class Command : IRequest<int>
		{
			public int srmaId { get; private set; }
			public DateTime? AcceptedDate { get; private set; }

			private Command(int id, DateTime? dateTime)
			{
				this.srmaId = id;
				this.AcceptedDate = dateTime;
			}

			public static Command Create(int id, DateTime? dateTime)
			{
				return new Command(id, dateTime);
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

				srma.DateAccepted= request.AcceptedDate;
				srma.UpdatedAt = DateTime.Now;

				await _context.SaveChangesAsync();

				var updatedNotification = new SRMAUpdatedNotification() { Id = srma.Id, CaseId = srma.CaseUrn, };
				await _mediator.Publish(updatedNotification);

				return srma.Id;
			}
		}
	}
}
