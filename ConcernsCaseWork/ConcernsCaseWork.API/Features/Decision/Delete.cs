using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.Decision
{
	public class Delete
	{
		public class Command : IRequest
		{
			public int ConcernsCaseUrn { get; set; }
			public int DecisionId { get; set; }
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
				var concernCase = await _context.ConcernsCase
											.Include(x => x.Decisions)
											.ThenInclude(x => x.DecisionTypes)
											.Include(x => x.Decisions)
											.ThenInclude(x => x.Outcome)
											.ThenInclude(x => x.BusinessAreasConsulted)
											.FirstOrDefaultAsync(o => o.Urn == request.ConcernsCaseUrn);

				if (concernCase == null)
				{
					throw new NotFoundException($"The concerns case for urn {request.ConcernsCaseUrn}, was not found");
				}

				concernCase.DeleteDecision(request.DecisionId, DateTimeOffset.Now);
				await _context.SaveChangesAsync();
			}
		}
	}
}
