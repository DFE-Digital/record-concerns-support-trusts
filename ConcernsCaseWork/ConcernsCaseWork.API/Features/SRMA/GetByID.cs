using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class GetByID
	{
		public class Query : IRequest<Result>
		{
			public int srmaId { get; set; }
		}

		public class Result
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
				Result result = await _context.SRMACases.ProjectTo<Result>(_mapperConfiguration).SingleOrDefaultAsync(f => f.Id == request.srmaId);
				return result;
			}
		}
	}
}