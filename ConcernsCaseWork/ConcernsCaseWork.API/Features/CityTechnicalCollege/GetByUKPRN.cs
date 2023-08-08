using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.CityTechnicalCollege
{
	public class GetByUKPRN
	{
		public class Query : IRequest<Result>
		{
			public string UKPRN { get; set; }
		}


		public class Result
		{

			public string Name { get; set; }
			public string UKPRN { get; set; }
			public string CompaniesHouseNumber { get; set; }

			public string AddressLine1 { get; set; }
			public string AddressLine2 { get; set; }
			public string AddressLine3 { get; set; }
			public string Town { get; set; }
			public string County { get; set; }
			public string Postcode { get; set; }
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
				Result result = await _context.CityTechnologyColleges.AsNoTracking().ProjectTo<Result>(_mapperConfiguration).SingleOrDefaultAsync(f => f.UKPRN == request.UKPRN);

				return result;
			}
		}
	}
}
