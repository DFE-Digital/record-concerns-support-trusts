using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public sealed class CaseHistoryService : AbstractService, ICaseHistoryService
	{
		private readonly ILogger<CaseHistoryService> _logger;
		
		public CaseHistoryService(IHttpClientFactory clientFactory, ILogger<CaseHistoryService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = logger;
		}
		
		public async Task<CaseHistoryDto> PostCaseHistory(CreateCaseHistoryDto createCaseHistoryDto)
		{
			try
			{
				_logger.LogInformation("CaseHistoryService::PostCaseHistory");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(createCaseHistoryDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = CreateHttpClient();
				
				// Execute request
				var response = await client.PostAsync($"/{_endpointsVersion}/concerns-cases-history", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperNewCaseDto = JsonConvert.DeserializeObject<ApiWrapper<CaseHistoryDto>>(content);

				// Unwrap response
				if (apiWrapperNewCaseDto is { Data: { } })
				{
					return apiWrapperNewCaseDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseHistoryService::PostCaseHistory::Exception message::{Message}", ex.Message);

				throw;
			}
		}

		public async Task<ApiListWrapper<CaseHistoryDto>> GetCasesHistory(CaseSearch caseSearch)
		{
			try
			{
				_logger.LogInformation("CaseHistoryService::GetCasesHistory");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{_endpointsVersion}/concerns-cases-history/case/urn/{caseSearch.CaseUrn}?page={caseSearch.Page}");
				
				// Create http client
				var client = CreateHttpClient();
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperCasesHistoryDto = JsonConvert.DeserializeObject<ApiListWrapper<CaseHistoryDto>>(content);

				return apiWrapperCasesHistoryDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseHistoryService::GetCasesHistory::Exception message::{Message}", ex.Message);
				
				throw;
			}
		}
	}
}