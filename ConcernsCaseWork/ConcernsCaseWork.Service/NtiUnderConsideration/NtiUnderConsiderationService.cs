using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace ConcernsCaseWork.Service.NtiUnderConsideration
{
	public class NtiUnderConsiderationService(IHttpClientFactory httpClientFactory, ILogger<NtiUnderConsiderationService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService, IUserTokenService userTokenService) : ConcernsAbstractService(httpClientFactory, logger, correlationContext, userInfoService, userTokenService), INtiUnderConsiderationService
	{
		private const string _url = @"/v2/case-actions/nti-under-consideration";

		public async Task<NtiUnderConsiderationDto> CreateNti(NtiUnderConsiderationDto ntiDto)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Post, $"{_url}")
				{
					Content = new StringContent(JsonConvert.SerializeObject(ntiDto),
					Encoding.UTF8, MediaTypeNames.Application.Json)
				};

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiUnderConsiderationDto>>(content).Data;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error occured while trying to create NTI");
				throw;
			}
		}

		public async Task<ICollection<NtiUnderConsiderationDto>> GetNtisForCase(long caseUrn)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Get, $"{_url}/case/{caseUrn}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<List<NtiUnderConsiderationDto>>>(content).Data;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error occured while trying to GetNtisForCase");
				throw;
			}
		}

		public async Task<NtiUnderConsiderationDto> GetNTIUnderConsiderationById(long underConsiderationId)
		{
			logger.LogInformation("NTIUnderConsiderationService::GetNTIUnderConsiderationById");

			// Create a request
			var request = new HttpRequestMessage(HttpMethod.Get,
				$"{_url}/{underConsiderationId}");

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
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{_url}");

				request.Content = new StringContent(JsonConvert.SerializeObject(ntiDto),
					Encoding.UTF8, MediaTypeNames.Application.Json);

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				response.EnsureSuccessStatusCode();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiUnderConsiderationDto>>(content).Data;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error occured while trying to patch NTI");
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
				logger.LogInformation("NTIUnderConsiderationService::DeleteNti");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Delete,
					$"{_url}/{ntiId}");

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
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
