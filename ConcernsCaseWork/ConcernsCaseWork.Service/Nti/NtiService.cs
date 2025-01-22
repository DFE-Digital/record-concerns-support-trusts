using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Helpers;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace ConcernsCaseWork.Service.Nti
{
	public class NtiService(IHttpClientFactory httpClientFactory, ILogger<NtiService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService, IUserTokenService userTokenService) : ConcernsAbstractService(httpClientFactory, logger, correlationContext, userInfoService, userTokenService), INtiService
	{
		private const string _url = @"/v2/case-actions/notice-to-improve";

		public async Task<NtiDto> CreateNtiAsync(NtiDto newNti)
		{
			try
			{
				logger.LogInformation($"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");

				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Post, $"{_url}")
				{
					Content = new StringContent(JsonConvert.SerializeObject(newNti),
					Encoding.UTF8, MediaTypeNames.Application.Json)
				};

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiDto>>(content).Data;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}

		public async Task<ICollection<NtiDto>> GetNtisForCaseAsync(long caseUrn)
		{
			try
			{
				logger.LogInformation($"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");

				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Get, $"{_url}/case/{caseUrn}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<List<NtiDto>>>(content).Data;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}

		public async Task<NtiDto> GetNtiAsync(long ntiId)
		{
			try
			{
				logger.LogInformation($"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");

				var request = new HttpRequestMessage(HttpMethod.Get, $"{_url}/{ntiId}");

				var client = CreateHttpClient();

				var response = await client.SendAsync(request);

				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiDto>>(content).Data;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}

		public async Task<NtiDto> PatchNtiAsync(NtiDto nti)
		{
			try
			{
				logger.LogInformation($"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");

				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{_url}")
				{
					Content = new StringContent(JsonConvert.SerializeObject(nti),
					Encoding.UTF8, MediaTypeNames.Application.Json)
				};

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				response.EnsureSuccessStatusCode();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiDto>>(content).Data;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}

		public async Task DeleteNtiAsync(long ntiId)
		{
			try
			{
				logger.LogInformation($"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");

				var request = new HttpRequestMessage(HttpMethod.Delete, $"{_url}/{ntiId}");

				var client = CreateHttpClient();

				var response = await client.SendAsync(request);

				response.EnsureSuccessStatusCode();

			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}
	}
}
