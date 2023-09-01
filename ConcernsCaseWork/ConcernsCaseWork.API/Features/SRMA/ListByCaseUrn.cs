using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class ListByCaseUrn
	{
		public class Query : IRequest<Result>
		{
			public int caseUrn { get; set; }
		}

		public class Result
		{
			public ICollection<ResultItem> Data { get; set; }

			public class ResultItem
			{
				public int Id { get; set; }
				public int CaseUrn { get; set; }
				public DateTime CreatedAt { get; set; }
				public DateTime DateOffered { get; set; }
				public DateTime? DateAccepted { get; set; }
				public DateTime? DateReportSentToTrust { get; set; }
				public DateTime? DateVisitStart { get; set; }
				public DateTime? DateVisitEnd { get; set; }
				public SRMAStatus Status { get; set; }
				public string Notes { get; set; }
				public SRMAReasonOffered? Reason { get; set; }
				public SRMAStatus CloseStatus { get; set; }
				public DateTime? UpdatedAt { get; set; }
				public DateTime? ClosedAt { get; set; }
				public string CreatedBy { get; set; }
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
				var items = await _context.SRMACases
					.AsNoTracking()
					.Where(f => f.CaseUrn == request.caseUrn).ProjectTo<Result.ResultItem>(_mapperConfiguration).ToListAsync();

				var result = new Result(items);
				return result;
			}

		}
	}
}
