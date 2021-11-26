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
	public sealed class CaseService : AbstractService, ICaseService
	{
		private readonly ILogger<CaseService> _logger;

		public CaseService(IHttpClientFactory clientFactory, ILogger<CaseService> logger) : base(clientFactory)
		{
			_logger = logger;
		}
		
		public async Task<ApiListWrapper<CaseDto>> GetCasesByCaseworkerAndStatus(CaseCaseWorkerSearch caseCaseWorkerSearch)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByCaseworkerAndStatus {Caseworker} {StatusUrn}", caseCaseWorkerSearch.CaseWorkerName, caseCaseWorkerSearch.StatusUrn);
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"/{EndpointsVersion}/{EndpointPrefix}/owner/{caseCaseWorkerSearch.CaseWorkerName}?status={caseCaseWorkerSearch.StatusUrn}&page={caseCaseWorkerSearch.Page}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperCasesDto = JsonConvert.DeserializeObject<ApiListWrapper<CaseDto>>(content);

				return apiWrapperCasesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseService::GetCasesByCaseworkerAndStatus::Exception message::{Message}", ex.Message);
				throw;
			}
		}

		public async Task<CaseDto> GetCaseByUrn(long urn)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByUrn {Urn}", urn);
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/{EndpointPrefix}/urn/{urn}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var caseDto = JsonConvert.DeserializeObject<ApiWrapper<CaseDto>>(content);
				
				// Unwrap response
				if (caseDto is { Data: { } })
				{
					return caseDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseService::GetCasesByUrn::Exception message::{Message}", ex.Message);

				throw;
			}
		}

		public async Task<ApiListWrapper<CaseDto>> GetCasesByTrustUkPrn(CaseTrustSearch caseTrustSearch)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByTrustUkPrn {TrustUkPrn} - {Page}", caseTrustSearch.TrustUkPrn, caseTrustSearch.Page);
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/{EndpointPrefix}/ukprn/{caseTrustSearch.TrustUkPrn}?page={caseTrustSearch.Page}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperCasesDto = JsonConvert.DeserializeObject<ApiListWrapper<CaseDto>>(content);

				return apiWrapperCasesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseService::GetCasesByTrustUkPrn::Exception message::{Message}", ex.Message);
				
				throw;
			}
		}

		public async Task<ApiListWrapper<CaseDto>> GetCases(PageSearch pageSearch)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCases {Page}", pageSearch.Page);
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/cases?page={pageSearch.Page}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperCasesDto = JsonConvert.DeserializeObject<ApiListWrapper<CaseDto>>(content);

				return apiWrapperCasesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseService::GetCases::Exception message::{Message}", ex.Message);
				
				throw;
			}
		}

		public async Task<CaseDto> PostCase(CreateCaseDto createCaseDto)
		{
			try
			{
				_logger.LogInformation("CaseService::PostCase");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(createCaseDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.PostAsync($"/{EndpointsVersion}/{EndpointPrefix}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperNewCaseDto = JsonConvert.DeserializeObject<ApiWrapper<CaseDto>>(content);

				// Unwrap response
				if (apiWrapperNewCaseDto is { Data: { } })
				{
					return apiWrapperNewCaseDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseService::PostCase::Exception message::{Message}", ex.Message);

				throw;
			}
		}

		public async Task<CaseDto> PatchCaseByUrn(CaseDto caseDto)
		{
			try
			{
				_logger.LogInformation("CaseService::PatchCaseByUrn {Urn}", caseDto.Urn);
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(caseDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.PatchAsync($"/{EndpointsVersion}/{EndpointPrefix}/{caseDto.Urn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperUpdatedCaseDto = JsonConvert.DeserializeObject<ApiWrapper<CaseDto>>(content);

				// Unwrap response
				if (apiWrapperUpdatedCaseDto is { Data: { } })
				{
					return apiWrapperUpdatedCaseDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseService::PatchCaseByUrn::Exception message::{Message}", ex.Message);

				throw;
			}
		}
	}
}