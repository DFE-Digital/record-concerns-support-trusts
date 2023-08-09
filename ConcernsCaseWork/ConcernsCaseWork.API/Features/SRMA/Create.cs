using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.SRMA
{
	using ConcernsCaseWork.API.Contracts.Srma;
	using ConcernsCaseWork.Data.Enums;
	using Microsoft.EntityFrameworkCore;
	using System.ComponentModel.DataAnnotations;

	public class Create
	{

		public class Command : IRequest<int>
		{
			[Required]
			public int Id { get; set; }
			[Required]
			public int CaseUrn { get; set; }
			public DateTime CreatedAt { get; set; }

			[StringLength(300)]
			public string CreatedBy { get; set; }
			public DateTime DateOffered { get; set; }
			public DateTime? DateAccepted { get; set; }
			public DateTime? DateReportSentToTrust { get; set; }
			public DateTime? DateVisitStart { get; set; }
			public DateTime? DateVisitEnd { get; set; }
			public SRMAStatus Status { get; set; }

			[StringLength(SrmaConstants.NotesLength)]
			public string Notes { get; set; }
			public SRMAReasonOffered? Reason { get; set; }
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

			public async Task<int> Handle(Command request, CancellationToken cancellationToken)
			{
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
