using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.FinancialPlan
{
	public class GetByID
	{
		public class Query : IRequest<Result>
		{
			public long Id { get; set; }
		}

		public class Result
		{
			public long Id { get; set; }
			public int CaseUrn { get; set; }
			public string Name { get; set; }
			public long? StatusId { get; set; }
			public DateTime? DatePlanRequested { get; set; }
			public DateTime? DateViablePlanReceived { get; set; }
			public DateTime CreatedAt { get; set; }
			public string CreatedBy { get; set; }
			public DateTime UpdatedAt { get; set; }
			public DateTime? ClosedAt { get; set; }
			public string Notes { get; set; }

			public StatusModel Status { get; set; }

		}

		public class StatusModel
		{
			public long Id { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public DateTime CreatedAt { get; set; }
			public DateTime UpdatedAt { get; set; }
			public bool IsClosedStatus { get; set; }
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
				Result result = await _context.FinancialPlanCases.Include(fp => fp.Status).ProjectTo<Result>(_mapperConfiguration).SingleOrDefaultAsync(fp => fp.Id == request.Id);

				return result;
			}
		}
	}
}
