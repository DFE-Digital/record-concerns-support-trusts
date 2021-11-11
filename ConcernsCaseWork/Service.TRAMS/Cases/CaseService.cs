﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Service.TRAMS.Cases
{
	public sealed class CaseService : AbstractService, ICaseService
	{
		private readonly ILogger<CaseService> _logger;

		public CaseService(IHttpClientFactory clientFactory, ILogger<CaseService> logger) : base(clientFactory)
		{
			_logger = logger;
		}
		
		public async Task<IList<CaseDto>> GetCasesByCaseworkerAndStatus(string caseworker, long statusUrn)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByCaseworkerAndStatus");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"/{EndpointsVersion}/{EndpointPrefix}/owner/{caseworker}?status={statusUrn}");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var casesDto = JsonConvert.DeserializeObject<IList<CaseDto>>(content);

				return casesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseService::GetCasesByCaseworkerAndStatus::Exception message::{Message}", ex.Message);
			}
			
			return Array.Empty<CaseDto>();
		}

		public async Task<CaseDto> GetCaseByUrn(long urn)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/{EndpointPrefix}/urn/{urn}");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
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
				_logger.LogInformation("CaseService::GetCasesByTrustUkPrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/{EndpointPrefix}/ukprn/{caseTrustSearch.TrustUkPrn}?{BuildRequestUri(caseTrustSearch)}");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
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

		public async Task<IList<CaseDto>> GetCasesByPagination(CaseSearch caseSearch)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByPagination");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/cases?page={caseSearch.Page}");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var casesDto = JsonConvert.DeserializeObject<IList<CaseDto>>(content);

				return casesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseService::GetCasesByPagination::Exception message::{Message}", ex.Message);
			}
			
			return Array.Empty<CaseDto>();
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
				var client = ClientFactory.CreateClient("TramsClient");
				
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
				_logger.LogInformation("CaseService::PatchCaseByUrn");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(caseDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PatchAsync($"/{EndpointsVersion}/case/urn/{caseDto.Urn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var updatedCaseDto = JsonConvert.DeserializeObject<CaseDto>(content);

				return updatedCaseDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseService::PatchCaseByUrn::Exception message::{Message}", ex.Message);

				throw;
			}
		}
		
		public static string BuildRequestUri(CaseTrustSearch caseTrustSearch)
		{
			var queryParams = HttpUtility.ParseQueryString(string.Empty);
			queryParams.Add("page", caseTrustSearch.Page.ToString());
			
			return HttpUtility.UrlEncode(queryParams.ToString());
		}
	}
}