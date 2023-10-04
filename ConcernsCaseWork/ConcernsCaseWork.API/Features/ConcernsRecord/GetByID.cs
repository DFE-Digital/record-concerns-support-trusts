using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	public class GetByID
	{
			public class Query : IRequest<ConcernsRecordResponse>
			{
				public int Id { get; set; }
			}

		public class Handler : IRequestHandler<Query, ConcernsRecordResponse>
		{
			private readonly ConcernsDbContext _context;
			private readonly MapperConfiguration _mapperConfiguration;

			public Handler(ConcernsDbContext context, MapperConfiguration mapperConfiguration)
			{
				_context = context;
				_mapperConfiguration = mapperConfiguration;
			}

			public async Task<ConcernsRecordResponse> Handle(Query request, CancellationToken cancellationToken)
			{
				var result = await _context.ConcernsRecord.Include(f=> f.ConcernsCase).Where(f => f.Id == request.Id).ProjectTo<ConcernsRecordResponse>(_mapperConfiguration).SingleOrDefaultAsync(f => f.Id == request.Id);
				return result;
			}
		}
	}
}
