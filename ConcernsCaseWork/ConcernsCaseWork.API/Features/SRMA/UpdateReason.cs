﻿using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class UpdateReason
	{
		public class Command : IRequest<int>
		{
			public int srmaId { get; private set; }
			public SRMAReasonOffered Reason { get; private set; }

			private Command(int id, SRMAReasonOffered reason)
			{
				this.srmaId = id;
				this.Reason = reason;
			}

			public static Command Create(int id, SRMAReasonOffered reason)
			{
				return new Command(id, reason);
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

				srma.ReasonId = (int)request.Reason;
				srma.UpdatedAt = DateTime.Now;

				await _context.SaveChangesAsync();

				var updatedNotification = new SRMAUpdatedNotification() { Id = srma.Id, CaseId = srma.CaseUrn, };
				await _mediator.Publish(updatedNotification);

				return srma.Id;
			}
		}
	}
}
