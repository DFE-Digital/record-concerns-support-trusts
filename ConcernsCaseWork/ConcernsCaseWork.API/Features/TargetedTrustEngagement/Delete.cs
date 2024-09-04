using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.TargetedTrustEngagement
{
	public class Delete
	{
		public class Command : IRequest
		{
			public int ConcernsCaseUrn { get; set; }
			public int TargetedTrustEngagementId { get; set; }
		}

		public class CommandHandler : IRequestHandler<Command>
		{
			private readonly ConcernsDbContext _context;

			public CommandHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task Handle(Command request, CancellationToken cancellationToken)
			{
				var concernCase = await _context.ConcernsCase.FirstOrDefaultAsync(o => o.Urn == request.ConcernsCaseUrn);

				if (concernCase == null)
				{
					throw new NotFoundException($"The concerns case for urn {request.ConcernsCaseUrn}, was not found");
				}

				var now = DateTime.UtcNow;
				var tte = _context.TargetedTrustEngagements.FirstOrDefault(t => t.Id == request.TargetedTrustEngagementId);
				tte.DeletedAt = now;

				await _context.SaveChangesAsync();
			}
		}
	}
}
