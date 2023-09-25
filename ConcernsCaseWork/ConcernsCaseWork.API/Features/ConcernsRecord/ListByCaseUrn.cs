using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	public class ListByCaseUrn
	{
		public class Query : IRequest<ConcernsRecordResponse[]>
		{
			public int urn { get; set; }
		}

		public class Handler : IRequestHandler<Query, ConcernsRecordResponse[]>
		{
			private readonly ConcernsDbContext _context;
			private readonly MapperConfiguration _mapperConfiguration;

			public Handler(ConcernsDbContext context, MapperConfiguration mapperConfiguration)
			{
				_context = context;
				_mapperConfiguration = mapperConfiguration;
			}

			public async Task<ConcernsRecordResponse[]> Handle(Query request, CancellationToken cancellationToken)
			{
				var result = await _context.ConcernsRecord
					.Include(f=> f.ConcernsRating)
					.Include(f => f.ConcernsType)
					.Include(f => f.ConcernsMeansOfReferral)
					.AsNoTracking()
					.Where(f => f.CaseId == request.urn).ProjectTo<ConcernsRecordResponse>(_mapperConfiguration).ToArrayAsync();
			
				return result;
			}
		}
	}
}
