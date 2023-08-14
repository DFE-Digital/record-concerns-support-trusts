﻿using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class UpdateDateReportSent
	{
		public class Command : IRequest<int>
		{
			public int srmaId { get; private set; }
			public DateTime? DateReportSentTo { get; private set; }

			private Command(int id, DateTime? dateTime)
			{
				this.srmaId = id;
				this.DateReportSentTo = dateTime;
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

				srma.DateReportSentToTrust = request.DateReportSentTo;
				srma.UpdatedAt = DateTime.Now;

				await _context.SaveChangesAsync();

				var updatedNotification = new SRMAUpdatedNotification() { Id = srma.Id, CaseId = srma.CaseUrn, };
				await _mediator.Publish(updatedNotification);

				return srma.Id;
			}
		}
	}
}
