using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.TRAMS.Trusts
{
	public sealed class TrustService : AbstractService, ITrustService
	{
		private readonly ILogger<TrustService> _logger;
		
		public TrustService(IHttpClientFactory clientFactory, ILogger<TrustService> logger) : base(clientFactory)
		{
			_logger = logger;
		}
		
		public async Task<IEnumerable<TrustDto>> GetTrustsByPagination(int page)
		{
			// TODO create TrustPagination options class
			
			// Create a request
			// groupName=acer&ukprn=TR01414&companiesHouseNumber=09591931&
			using var request = new HttpRequestMessage(HttpMethod.Get, $"/trusts?page={page}");
			
			// Create http client
			var client = ClientFactory.CreateClient("TramsClient");
				
			// Execute request
			var response = await client.SendAsync(request);


			return null;
		}
	}
}