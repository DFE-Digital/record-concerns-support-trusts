using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using MediatR;

namespace ConcernsCaseWork.API.Features.SRMA
{
	using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
	using ConcernsCaseWork.Data.Enums;

	public class Create
	{

		public class Command : IRequest<int>
		{
			public CreateSRMARequest Request { get; }

			public Command(CreateSRMARequest request)
			{
				Request = request;
			}
		}

		public class CommandHandler : IRequestHandler<Command, Int32>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;

			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task<int> Handle(Command command, CancellationToken cancellationToken)
			{
				var request = command.Request;
				var srma =  new SRMACase()
				{
					Id = request.Id,
					CaseUrn = request.CaseUrn,
					CreatedAt = request.CreatedAt,
					DateOffered = request.DateOffered,
					DateReportSentToTrust = request.DateReportSentToTrust,
					StartDateOfVisit = request.DateVisitStart,
					EndDateOfVisit = request.DateVisitEnd,
					DateAccepted = request.DateAccepted,
					StatusId = (int)request.Status,
					ReasonId = (int?)(request.Reason == SRMAReasonOffered.Unknown ? null : request.Reason),
					Notes = request.Notes,
					CreatedBy = request.CreatedBy
				};

				_context.SRMACases.Add(srma);

				await _context.SaveChangesAsync();


				var createdNotification = new SRMACreatedNotification() { Id = srma.Id, CaseId = srma.CaseUrn, };
				await _mediator.Publish(createdNotification);

				return srma.Id;
			}
		}
	}
}
