﻿using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace ConcernsCaseWork.Service.NtiUnderConsideration
{
	public class NtiUnderConsiderationService : ConcernsAbstractService, INtiUnderConsiderationService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<NtiUnderConsiderationService> _logger;
		private const string Url = @"/v2/case-actions/nti-under-consideration";

		public NtiUnderConsiderationService(IHttpClientFactory httpClientFactory, ILogger<NtiUnderConsiderationService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService) : base(httpClientFactory, logger, correlationContext, userInfoService)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		public async Task<NtiUnderConsiderationDto> CreateNti(NtiUnderConsiderationDto ntiDto)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Post, $"{Url}");

				request.Content = new StringContent(JsonConvert.SerializeObject(ntiDto),
					Encoding.UTF8, MediaTypeNames.Application.Json);

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiUnderConsiderationDto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to create NTI");
				throw;
			}
		}

		public async Task<ICollection<NtiUnderConsiderationDto>> GetNtisForCase(long caseUrn)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Get, $"{Url}/case/{caseUrn}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<List<NtiUnderConsiderationDto>>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to GetNtisForCase");
				throw;
			}
		}

		public async Task<NtiUnderConsiderationDto> GetNTIUnderConsiderationById(long underConsiderationId)
		{
			_logger.LogInformation("NTIUnderConsiderationService::GetNTIUnderConsiderationById");

			// Create a request
			var request = new HttpRequestMessage(HttpMethod.Get,
				$"{Url}/{underConsiderationId}");

			// Create http client
			var client = CreateHttpClient();

			// Execute request
			var response = await client.SendAsync(request);

			// Check status code
			response.EnsureSuccessStatusCode();

			// Read response content
			var content = await response.Content.ReadAsStringAsync();

			// Deserialize content to POJO
			var apiWrapperNTIUnderConsiderationDto = JsonConvert.DeserializeObject<ApiWrapper<NtiUnderConsiderationDto>>(content);

			// Unwrap response
			if (apiWrapperNTIUnderConsiderationDto is { Data: { } })
			{
				return apiWrapperNTIUnderConsiderationDto.Data;
			}

			throw new Exception("Academies API error unwrap response");
		}

		public async Task<NtiUnderConsiderationDto> PatchNti(NtiUnderConsiderationDto ntiDto)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{Url}");

				request.Content = new StringContent(JsonConvert.SerializeObject(ntiDto),
					Encoding.UTF8, MediaTypeNames.Application.Json);

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				response.EnsureSuccessStatusCode();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiUnderConsiderationDto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to patch NTI");
				throw;
			}
		}

		public async Task<NtiUnderConsiderationDto> GetNti(long ntiId)
		{
			return await GetNTIUnderConsiderationById(ntiId);
		}

		public async Task DeleteNti(long ntiId)
		{
			try
			{
				_logger.LogInformation("NTIUnderConsiderationService::DeleteNti");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Delete,
					$"{Url}/{ntiId}");

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to delete NTI");
				throw;
			}
		}
	}
}
