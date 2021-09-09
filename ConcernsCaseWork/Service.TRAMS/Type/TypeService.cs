using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.TRAMS.Type
{
	public sealed class TypeService : AbstractService, ITypeService
	{
		private readonly ILogger<TypeService> _logger;
		
		public TypeService(IHttpClientFactory clientFactory, ILogger<TypeService> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<IList<TypeDto>> GetTypes()
		{
			try
			{
				_logger.LogInformation("TypeService::GetTypes");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"{EndpointsVersion}/types");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var typesDto = JsonSerializer.Deserialize<IList<TypeDto>>(content, options);

				return typesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"TypeService::GetTypes::Exception message::{ex.Message}");
			}
			
			// TODO replace return when TRAMS API endpoints are live
			return new List<TypeDto>
			{
				new TypeDto("Compliance", "Compliance: Financial reporting", DateTime.Now, 
					DateTime.Now, new BigInteger(1)),
				new TypeDto("Compliance", "Compliance: Financial returns", DateTime.Now, 
					DateTime.Now, new BigInteger(2)),
				new TypeDto("Financial", "Financial: Deficit", DateTime.Now, 
					DateTime.Now, new BigInteger(3)),
				new TypeDto("Financial", "Financial: Projected deficit / Low future surplus", DateTime.Now, 
					DateTime.Now, new BigInteger(4)),
				new TypeDto("Financial", "Financial: Cash flow shortfall", DateTime.Now, 
					DateTime.Now, new BigInteger(5)),
				new TypeDto("Financial", "Financial: Clawback", DateTime.Now, 
					DateTime.Now, new BigInteger(6)),
				new TypeDto("Force Majeure", "", DateTime.Now, 
					DateTime.Now, new BigInteger(7)),
				new TypeDto("Governance", "Governance: Governance", DateTime.Now, 
					DateTime.Now, new BigInteger(8)),
				new TypeDto("Governance", "Governance: Closure", DateTime.Now, 
					DateTime.Now, new BigInteger(9)),
				new TypeDto("Governance", "Governance: Executive Pay", DateTime.Now, 
					DateTime.Now, new BigInteger(10)),
				new TypeDto("Governance", "Governance: Safeguarding", DateTime.Now, 
					DateTime.Now, new BigInteger(11)),
				new TypeDto("Irregularity", "Irregularity: Allegations and self reported concerns", DateTime.Now, 
					DateTime.Now, new BigInteger(12)),
				new TypeDto("Irregularity", "Irregularity: Related party transactions - in year", DateTime.Now, 
					DateTime.Now, new BigInteger(13))
			};
			
			//return Array.Empty<TypeDto>();
		}
	}
}