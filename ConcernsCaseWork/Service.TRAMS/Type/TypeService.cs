using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
				var typesDto = JsonConvert.DeserializeObject<IList<TypeDto>>(content);

				return typesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"TypeService::GetTypes::Exception message::{ex.Message}");
			}
			
			// TODO replace return when TRAMS API endpoints are live
			var currentDate = DateTimeOffset.Now;
			return new List<TypeDto>
			{
				new TypeDto("Compliance", "Compliance: Financial reporting", currentDate, 
					currentDate, 1),
				new TypeDto("Compliance", "Compliance: Financial returns", currentDate, 
					currentDate, 2),
				new TypeDto("Financial", "Financial: Deficit", currentDate, 
					currentDate, 3),
				new TypeDto("Financial", "Financial: Projected deficit / Low future surplus", currentDate, 
					currentDate, 4),
				new TypeDto("Financial", "Financial: Cash flow shortfall", currentDate, 
					currentDate, 5),
				new TypeDto("Financial", "Financial: Clawback", currentDate, 
					currentDate, 6),
				new TypeDto("Force Majeure", "", currentDate, 
					currentDate, 7),
				new TypeDto("Governance", "Governance: Governance", currentDate, 
					currentDate, 8),
				new TypeDto("Governance", "Governance: Closure", currentDate, 
					currentDate, 9),
				new TypeDto("Governance", "Governance: Executive Pay", currentDate, 
					currentDate, 10),
				new TypeDto("Governance", "Governance: Safeguarding", currentDate, 
					currentDate, 11),
				new TypeDto("Irregularity", "Irregularity: Allegations and self reported concerns", currentDate, 
					currentDate, 12),
				new TypeDto("Irregularity", "Irregularity: Related party transactions - in year", currentDate, 
					currentDate, 13)
			};
			
			//return Array.Empty<TypeDto>();
		}
	}
}