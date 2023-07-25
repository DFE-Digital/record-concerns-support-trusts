using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	public class GetByID
	{
		public class Query : IRequest<Result>
		{
			public int Id { get; set; }
		}

		public class Result
		{
			public DateTime CreatedAt { get; set; }
			public DateTime UpdatedAt { get; set; }
			public DateTime ReviewAt { get; set; }
			public DateTime? ClosedAt { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public string Reason { get; set; }
			public int RatingId { get; set; }
			public int Id { get; set; }
			public int StatusId { get; set; }
			public int TypeId { get; set; }
			public int CaseUrn { get; set; }
			public int? MeansOfReferralId { get; set; }

		}

		public class Handler : IRequestHandler<Query, Result>
		{
			private readonly ConcernsDbContext _context;
			private readonly MapperConfiguration _mapperConfiguration;

			public Handler(ConcernsDbContext context, MapperConfiguration mapperConfiguration)
			{
				_context = context;
				_mapperConfiguration = mapperConfiguration;
			}

			public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
			{
				Result result = await _context.ConcernsRecord.Include(f=> f.ConcernsCase).Where(f => f.Id == request.Id).ProjectTo<Result>(_mapperConfiguration).SingleOrDefaultAsync(f => f.Id == request.Id);
				return result;
			}
		}
	}
}
