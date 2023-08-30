using AutoMapper;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	public class Delete
	{
		public class Query : IRequest<Unit>
		{
			public int Id { get; set; }
		}

		public class CommandHandler : IRequestHandler<Query, Unit>
		{
			private readonly ConcernsDbContext _context;
			private readonly MapperConfiguration _mapperConfiguration;

			public CommandHandler(ConcernsDbContext context, MapperConfiguration mapperConfiguration)
			{
				_context = context;
				_mapperConfiguration = mapperConfiguration;
			}

			public async Task<Unit> Handle(Query request, CancellationToken cancellationToken)
			{
				var result = await _context.ConcernsRecord.SingleOrDefaultAsync(f => f.Id == request.Id);
				result.DeletedAt = System.DateTime.Now;
				await _context.SaveChangesAsync();

				return Unit.Value;
			}
		}
	}
}
