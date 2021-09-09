using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public sealed class CaseService : AbstractService, ICaseService
	{
		private readonly ILogger<CaseService> _logger;
		
		public CaseService(IHttpClientFactory clientFactory, ILogger<CaseService> logger) : base(clientFactory)
		{
			_logger = logger;
		}
		
		public async Task<IList<CaseDto>> GetCasesByCaseworker(string caseworker)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByCaseworker");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/cases/owner/{caseworker}");
				
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
				var casesDto = JsonSerializer.Deserialize<IList<CaseDto>>(content, options);

				return casesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseService::GetCasesByCaseworker::Exception message::{ex.Message}");
			}
			
			return Array.Empty<CaseDto>();
		}

		public async Task<CaseDto> GetCasesByUrn(string urn)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/case/urn/{urn}");
				
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
				var caseDto = JsonSerializer.Deserialize<CaseDto>(content, options);

				return caseDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseService::GetCasesByUrn::Exception message::{ex.Message}");
			}
			
			return null;
		}

		public async Task<IList<CaseDto>> GetCasesByTrustUkPrn(string trustUkprn)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByTrustUkPrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/cases/trust/{trustUkprn}");
				
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
				var casesDto = JsonSerializer.Deserialize<IList<CaseDto>>(content, options);

				return casesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseService::GetCasesByTrustUkPrn::Exception message::{ex.Message}");
			}
			
			return Array.Empty<CaseDto>();
		}

		public async Task<IList<CaseDto>> GetCasesByPagination(CaseSearch caseSearch)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByPagination");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/cases?page={caseSearch.Page}");
				
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
				var casesDto = JsonSerializer.Deserialize<IList<CaseDto>>(content, options);

				return casesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseService::GetCasesByPagination::Exception message::{ex.Message}");
			}
			
			return Array.Empty<CaseDto>();
		}

		public async Task<CaseDto> PostCase(CaseDto caseDto)
		{
			try
			{
				_logger.LogInformation("CaseService::PostCase");
				
				// Create a request
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var request = new StringContent(
					JsonSerializer.Serialize(caseDto, options),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PostAsync("/case", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var newCaseDto = JsonSerializer.Deserialize<CaseDto>(content, options);

				return newCaseDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseService::PostCase::Exception message::{ex.Message}");

				throw;
			}
		}

		public async Task<CaseDto> PatchCaseByUrn(CaseDto caseDto)
		{
			try
			{
				_logger.LogInformation("CaseService::PatchCaseByUrn");
				
				// Create a request
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var request = new StringContent(
					JsonSerializer.Serialize(caseDto, options),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PatchAsync($"/case/urn/{caseDto.Urn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var updatedCaseDto = JsonSerializer.Deserialize<CaseDto>(content, options);

				return updatedCaseDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseService::PatchCaseByUrn::Exception message::{ex.Message}");

				throw;
			}
		}
	}
}