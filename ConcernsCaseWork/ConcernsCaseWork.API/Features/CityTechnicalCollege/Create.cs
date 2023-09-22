using ConcernsCaseWork.API.Contracts.Trusts;
using ConcernsCaseWork.Data;
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
			public CityTechnologyCollege Request { get; }

			public Command(CityTechnologyCollege request)
			{
				Request = request;
			}
		}

		public class CommandHandler : IRequestHandler<Command, Int32>
		{
			private readonly ConcernsDbContext _context;

			public CommandHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task<int> Handle(Command command, CancellationToken cancellationToken)
			{
				var request = command.Request;
				var ctc = Data.Models.CityTechnologyCollege.Create(request.Name, request.UKPRN, request.CompaniesHouseNumber);

				ctc.ChangeAddress(request.AddressLine1, request.AddressLine2, request.AddressLine3, request.Town, request.County, request.Postcode);

				_context.CityTechnologyColleges.Add(ctc);
				return await _context.SaveChangesAsync();
			}
		}
	}
}
