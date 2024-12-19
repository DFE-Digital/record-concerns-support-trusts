﻿using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace ConcernsCaseWork.Service.NtiWarningLetter
{
	public class NtiWarningLetterService(IHttpClientFactory httpClientFactory, ILogger<NtiWarningLetterService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService, IUserTokenService userTokenService) : ConcernsAbstractService(httpClientFactory, logger, correlationContext, userInfoService, userTokenService), INtiWarningLetterService
	{
		private const string _url = @"/v2/case-actions/nti-warning-letter";

		public async Task<NtiWarningLetterDto> CreateNtiWarningLetterAsync(NtiWarningLetterDto newNtiWarningLetter)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Post, $"{_url}")
				{
					Content = new StringContent(JsonConvert.SerializeObject(newNtiWarningLetter),
					Encoding.UTF8, MediaTypeNames.Application.Json)
				};

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiWarningLetterDto>>(content).Data;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error occured while trying to create NTI Warning Letter");
				throw;
			}
		}

		public async Task<NtiWarningLetterDto> GetNtiWarningLetterAsync(long ntiWarningLetterId)
		{
			// Create a request
			var request = new HttpRequestMessage(HttpMethod.Get, $"{_url}/{ntiWarningLetterId}");

			// Create http client
			var client = CreateHttpClient();

			// Execute request
			var response = await client.SendAsync(request);

			// Check status code
			response.EnsureSuccessStatusCode();

			// Read response content
			var content = await response.Content.ReadAsStringAsync();

			// Deserialize content to POJO
			var apiWrapperNTIWarningLetterDto = JsonConvert.DeserializeObject<ApiWrapper<NtiWarningLetterDto>>(content);

			// Unwrap response
			if (apiWrapperNTIWarningLetterDto is { Data: { } })
			{
				return apiWrapperNTIWarningLetterDto.Data;
			}

			throw new Exception("API error unwrap response");
		}

		public async Task<ICollection<NtiWarningLetterDto>> GetNtiWarningLettersForCaseAsync(long caseUrn)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Get, $"{_url}/case/{caseUrn}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<List<NtiWarningLetterDto>>>(content).Data;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error occured while trying to GetNtisForCase");
				throw;
			}
		}

		public async Task<NtiWarningLetterDto> PatchNtiWarningLetterAsync(NtiWarningLetterDto ntiWarningLetter)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{_url}")
				{
					Content = new StringContent(JsonConvert.SerializeObject(ntiWarningLetter),
					Encoding.UTF8, MediaTypeNames.Application.Json)
				};

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				response.EnsureSuccessStatusCode();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiWarningLetterDto>>(content).Data;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error occured while trying to patch NTI");
				throw;
			}
		}

		public async Task DeleteNtiWarningLetterAsync(long ntiWarningLetterId)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Delete, $"{_url}/{ntiWarningLetterId}");

				var response = await client.SendAsync(request);

				response.EnsureSuccessStatusCode();

			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error occured while trying to delete NTI");
				throw;
			}
		}
	}
}
