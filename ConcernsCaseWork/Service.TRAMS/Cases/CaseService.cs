﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
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
		
		public async Task<IList<CaseDto>> GetCasesByCaseworker(string caseworker, string statusName = "Live")
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByCaseworker");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"{EndpointsVersion}/cases/owner/{caseworker}?status={statusName}");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var casesDto = JsonConvert.DeserializeObject<IList<CaseDto>>(content);

				return casesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseService::GetCasesByCaseworker::Exception message::{ex.Message}");
			}
			
			return Array.Empty<CaseDto>();
		}

		public async Task<CaseDto> GetCaseByUrn(long urn)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"{EndpointsVersion}/case/urn/{urn}");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var caseDto = JsonConvert.DeserializeObject<CaseDto>(content);

				return caseDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseService::GetCasesByUrn::Exception message::{ex.Message}");

				throw;
			}
		}

		public async Task<IList<CaseDto>> GetCasesByTrustUkPrn(string trustUkprn)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByTrustUkPrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"{EndpointsVersion}/cases/trust/{trustUkprn}");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var casesDto = JsonConvert.DeserializeObject<IList<CaseDto>>(content);

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
				var request = new HttpRequestMessage(HttpMethod.Get, $"{EndpointsVersion}/cases?page={caseSearch.Page}");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var casesDto = JsonConvert.DeserializeObject<IList<CaseDto>>(content);

				return casesDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseService::GetCasesByPagination::Exception message::{ex.Message}");
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
				var response = await client.PostAsync($"{EndpointsVersion}/case", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var newCaseDto = JsonConvert.DeserializeObject<CaseDto>(content);

				return newCaseDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseService::PostCase::Exception message::{ex.Message}");

				throw;
			}
		}

		public async Task<CaseDto> PatchCaseByUrn(UpdateCaseDto updateCaseDto)
		{
			try
			{
				_logger.LogInformation("CaseService::PatchCaseByUrn");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(updateCaseDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PatchAsync($"{EndpointsVersion}/case/urn/{updateCaseDto.Urn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var updatedCaseDto = JsonConvert.DeserializeObject<CaseDto>(content);

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