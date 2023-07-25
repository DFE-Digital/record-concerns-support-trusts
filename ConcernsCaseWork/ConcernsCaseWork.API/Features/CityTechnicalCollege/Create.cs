using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using FluentValidation;
using MediatR;

namespace ConcernsCaseWork.API.Features.CityTechnicalCollege
{
	public class Create
	{
		public class Query : IRequest<Command>
		{

		}
		public class Command : IRequest<int>
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

		public class CommandHandler : IRequestHandler<Command, Int32>
		{
			private readonly ConcernsDbContext _context;

			public CommandHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task<int> Handle(Command request, CancellationToken cancellationToken)
			{
				var ctc = CityTechnologyCollege.Create(request.Name, request.UKPRN, request.CompaniesHouseNumber);

				ctc.ChangeAddress(request.AddressLine1, request.AddressLine2, request.AddressLine3, request.Town, request.County, request.Postcode);

				_context.CityTechnologyColleges.Add(ctc);
				return await _context.SaveChangesAsync();
			}
		}
	}
}
