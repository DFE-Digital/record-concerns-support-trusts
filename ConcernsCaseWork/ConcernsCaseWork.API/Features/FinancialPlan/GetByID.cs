using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.API.ResponseModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.FinancialPlan
{
	public class GetByID
	{
		public class Query : IRequest<FinancialPlanResponse>
		{
			public long Id { get; set; }
		}

		public class Handler : IRequestHandler<Query, FinancialPlanResponse>
		{
			private readonly ConcernsDbContext _context;
			private readonly MapperConfiguration _mapperConfiguration;

			public Handler(ConcernsDbContext context, MapperConfiguration mapperConfiguration)
			{
				_context = context;
				_mapperConfiguration = mapperConfiguration;
			}

			public async Task<FinancialPlanResponse> Handle(Query request, CancellationToken cancellationToken)
			{
				var result = await _context.FinancialPlanCases.Include(fp => fp.Status).ProjectTo<FinancialPlanResponse>(_mapperConfiguration).SingleOrDefaultAsync(fp => fp.Id == request.Id);

				return result;
			}
		}
	}
}
