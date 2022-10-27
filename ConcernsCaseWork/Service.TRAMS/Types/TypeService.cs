using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.TRAMS.Types
{
	public sealed class TypeService : AbstractService, ITypeService
	{
		private readonly ILogger<TypeService> _logger;
		
		public TypeService(IHttpClientFactory clientFactory, ILogger<TypeService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = logger;
		}

		public async Task<IList<TypeDto>> GetTypes()
		{
			try
			{
				_logger.LogInformation("TypeService::GetTypes");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{_endpointsVersion}/concerns-types");
				
				// Create http client
				var client = CreateHttpClient();
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiListWrapperTypesDto = JsonConvert.DeserializeObject<ApiListWrapper<TypeDto>>(content);

				// Unwrap response
				if (apiListWrapperTypesDto is { Data: { } })
				{
					return apiListWrapperTypesDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("TypeService::GetTypes::Exception message::{Message}", ex.Message);

				throw;
			}
		}
	}
}