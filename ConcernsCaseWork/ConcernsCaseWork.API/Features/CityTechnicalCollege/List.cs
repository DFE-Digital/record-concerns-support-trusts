using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.CityTechnicalCollege
{
	public class List
	{
		public class Query : IRequest<Result>
		{
			public string NameUKPRNCHNumber { get; set; }

			public bool HasNameUKPRNCHNumber
			{
				get
				{
					return !String.IsNullOrWhiteSpace(NameUKPRNCHNumber);
				}
			}
		}


			public class Result
		{
			public ICollection<CityTechnologyCollege> Items { get; set; }

			public class CityTechnologyCollege
			{
				public int Id { get; set; }	
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

			public Result(ICollection<CityTechnologyCollege> items)
			{
				this.Items = items;
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
				Func<IQueryable<CityTechnologyCollege>, IQueryable<CityTechnologyCollege>> orderby = f => f.OrderBy(f => f.Name);

				var query = _context.CityTechnologyColleges.AsNoTracking();

				if (request.HasNameUKPRNCHNumber)
				{
					query = query.Where(f => f.Name.Contains(request.NameUKPRNCHNumber) || f.UKPRN.Contains(request.NameUKPRNCHNumber) || f.CompaniesHouseNumber.Contains(request.NameUKPRNCHNumber));
				}

				var items = await orderby(query).ProjectTo<Result.CityTechnologyCollege>(_mapperConfiguration).ToListAsync();
				var result = new Result(items);
				return result;
			}
		}
	}
}
