using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class GetByID
	{
		public class Query : IRequest<SRMAResponse>
		{
			public int srmaId { get; set; }
		}

		public class Handler : IRequestHandler<Query, SRMAResponse>
		{
			private readonly ConcernsDbContext _context;
			private readonly MapperConfiguration _mapperConfiguration;

			public Handler(ConcernsDbContext context, MapperConfiguration mapperConfiguration)
			{
				_context = context;
				_mapperConfiguration = mapperConfiguration;
			}

			public async Task<SRMAResponse> Handle(Query request, CancellationToken cancellationToken)
			{
				SRMAResponse result = await _context.SRMACases.ProjectTo<SRMAResponse>(_mapperConfiguration).SingleOrDefaultAsync(f => f.Id == request.srmaId);
				return result;
			}
		}
	}
}