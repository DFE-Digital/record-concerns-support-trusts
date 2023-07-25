using ConcernsCaseWork.Data;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	using ConcernsCaseWork.Data.Models;
	using Microsoft.EntityFrameworkCore;

	public class Create
	{
		public class Command : IRequest<int>
		{
			public DateTime CreatedAt { get; set; }
			public DateTime UpdatedAt { get; set; }
			public DateTime ReviewAt { get; set; }
			public DateTime? ClosedAt { get; set; }

			[StringLength(300)]
			public string Name { get; set; }

			[StringLength(300)]
			public string Description { get; set; }

			[StringLength(300)]
			public string Reason { get; set; }
			public int CaseUrn { get; set; }
			public int TypeId { get; set; }
			public int RatingId { get; set; }
			public int StatusId { get; set; }
			public int MeansOfReferralId { get; set; }
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
				ConcernsRecord cr = ConcernsRecord.Create(request.CaseUrn, request.TypeId, request.RatingId, request.MeansOfReferralId, request.StatusId);

				cr.ChangeNameDescriptionAndReason(request.Name, request.Description, request.Reason);
				cr.SetAuditInformation(request.CreatedAt, request.UpdatedAt, request.ReviewAt);

				_context.ConcernsRecord.Add(cr);
							
				await _context.SaveChangesAsync();


				return cr.Id;
			}
		}

	}
}
