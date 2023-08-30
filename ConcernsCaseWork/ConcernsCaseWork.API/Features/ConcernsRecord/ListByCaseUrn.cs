using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	public class ListByCaseUrn
	{
		public class Query : IRequest<Result>
		{
			public int urn { get; set; }
		}

		public class Result
		{
			public ICollection<ResultItem> Data { get; set; }

			public class ResultItem
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

			public Result(ICollection<ResultItem> items)
			{
				this.Data = items;
			}
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
				var items = await _context.ConcernsRecord
					.Include(f=> f.ConcernsRating)
					.Include(f => f.ConcernsType)
					.Include(f => f.ConcernsMeansOfReferral)
					.AsNoTracking()
					.Where(f => f.CaseId == request.urn).ProjectTo<Result.ResultItem>(_mapperConfiguration).ToListAsync();
			
				var result = new Result(items);
				return result;
			}
		}
	}
}
