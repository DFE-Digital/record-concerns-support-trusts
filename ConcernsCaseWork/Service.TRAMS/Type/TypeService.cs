using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using Service.TRAMS.RecordSrma;
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
				var request = new HttpRequestMessage(HttpMethod.Get, "/types");
				
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
				new TypeDto("Record", "Log information when it is not a Concern", DateTimeOffset.Now, 
					DateTimeOffset.Now, new BigInteger(1)),
				new TypeDto("SRMA", "A proactive SRMA visit", DateTimeOffset.Now, 
					DateTimeOffset.Now, new BigInteger(2)),
				new TypeDto("Safeguarding Incident", "", DateTimeOffset.Now, 
					DateTimeOffset.Now, new BigInteger(3)),
				new TypeDto("Concern", "", DateTimeOffset.Now, 
					DateTimeOffset.Now, new BigInteger(4))
			};
			
			//return Array.Empty<TypeDto>();
		}
	}
}