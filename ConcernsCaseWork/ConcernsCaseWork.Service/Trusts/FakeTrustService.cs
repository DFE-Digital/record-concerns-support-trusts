using ConcernsCaseWork.API.Contracts.Configuration;
using Microsoft.Extensions.Options;

namespace ConcernsCaseWork.Service.Trusts
{
	public interface IFakeTrustService
	{
		TrustSearchResponseDto? GetTrustsByPagination(string groupName);
		TrustDetailsDto? GetTrustByUkPrn(string ukPrn);
	}

	public class FakeTrustService : IFakeTrustService
	{
		private IOptions<FakeTrustOptions> _options;

		public FakeTrustService(IOptions<FakeTrustOptions> options)
		{
			_options = options;
		}

		public TrustSearchResponseDto? GetTrustsByPagination(string groupName)
		{
			var matchingResult = _options.Value.Trusts.FirstOrDefault(t => t.Name == groupName);

			if (matchingResult == null) return null;

			return new TrustSearchResponseDto()
			{
				Trusts = new[]
				{
					new TrustSearchDto()
					{
						UkPrn = matchingResult.UkPrn,
						CompaniesHouseNumber = matchingResult.CompaniesHouseNumber,
						GroupName = matchingResult.Name
					}
				}
			};
		}

		public TrustDetailsDto? GetTrustByUkPrn(string ukPrn)
		{
			var matchingResult = _options.Value.Trusts.FirstOrDefault(t => t.UkPrn == ukPrn);

			if (matchingResult == null) return null;

			return new TrustDetailsDto()
			{
				GiasData = new GiasDataDto()
				{
					UkPrn = matchingResult.UkPrn,
					CompaniesHouseNumber = matchingResult.CompaniesHouseNumber,
					GroupName = matchingResult.Name,
					GroupContactAddress = new GroupContactAddressDto()
				}
			};
		}
	}
}
