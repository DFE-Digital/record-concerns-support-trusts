using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	public class NtiService : AbstractService, INtiService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<NtiService> _logger;
		private const string Url = @"/v2/case-actions/nti";

		public NtiService(IHttpClientFactory httpClientFactory, ILogger<NtiService> logger) : base(httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		public async Task<NtiDto> CreateNti(NtiDto ntiDto)
		{
			try
			{
				var client = _httpClientFactory.CreateClient(HttpClientName);
				var request = new HttpRequestMessage(HttpMethod.Post, $"{Url}");

				request.Content = new StringContent(JsonConvert.SerializeObject(ntiDto),
					Encoding.UTF8, MediaTypeNames.Application.Json);

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiDto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to create SRMA");
				throw;
			}
		}

		public async Task<ICollection<NtiDto>> GetNtisForCase(long caseUrn)
		{
			try
			{
				var client = _httpClientFactory.CreateClient(HttpClientName);
				var request = new HttpRequestMessage(HttpMethod.Get, $"{Url}/case/{caseUrn}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<List<NtiDto>>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to GetSRMAById");
				throw;
			}
		}

		public async Task<NtiDto> GetNTIUnderConsiderationById(long underConsiderationId)
		{
			_logger.LogInformation("NTIUnderConsiderationService::GetNTIUnderConsiderationById");

			// Create a request
			var request = new HttpRequestMessage(HttpMethod.Get,
				$"{Url}/{underConsiderationId}");

			// Create http client
			var client = ClientFactory.CreateClient(HttpClientName);

			// Execute request
			var response = await client.SendAsync(request);

			// Check status code
			response.EnsureSuccessStatusCode();

			// Read response content
			var content = await response.Content.ReadAsStringAsync();

			// Deserialize content to POJO
			var apiWrapperNTIUnderConsiderationDto = JsonConvert.DeserializeObject<ApiWrapper<NtiDto>>(content);

			// Unwrap response
			if (apiWrapperNTIUnderConsiderationDto is { Data: { } })
			{
				return apiWrapperNTIUnderConsiderationDto.Data;
			}

			throw new Exception("Academies API error unwrap response");
		}

		public async Task<NtiDto> PatchNti(NtiDto ntiDto)
		{
			try
			{
				var client = _httpClientFactory.CreateClient(HttpClientName);
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{Url}");

				request.Content = new StringContent(JsonConvert.SerializeObject(ntiDto),
					Encoding.UTF8, MediaTypeNames.Application.Json);

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				response.EnsureSuccessStatusCode();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiDto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to patch NTI");
				throw;
			}
		}

		public Task<NtiDto> GetNti(long ntiId)
		{
			throw new NotImplementedException();
		}
	}
}
