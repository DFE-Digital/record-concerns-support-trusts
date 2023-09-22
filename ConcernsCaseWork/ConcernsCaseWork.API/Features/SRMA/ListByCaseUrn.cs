using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class ListByCaseUrn
	{
		public class Query : IRequest<SRMAResponse[]>
		{
			public int caseUrn { get; set; }
		}

		public class Handler : IRequestHandler<Query, SRMAResponse[]>
		{
			private readonly ConcernsDbContext _context;
			private readonly MapperConfiguration _mapperConfiguration;

			public Handler(ConcernsDbContext context, MapperConfiguration mapperConfiguration)
			{
				_context = context;
				_mapperConfiguration = mapperConfiguration;
			}

			public async Task<SRMAResponse[]> Handle(Query request, CancellationToken cancellationToken)
			{
				var result = await _context.SRMACases
					.AsNoTracking()
					.Where(f => f.CaseUrn == request.caseUrn).ProjectTo<SRMAResponse>(_mapperConfiguration).ToArrayAsync();

				return result;
			}
		}
	}
}
