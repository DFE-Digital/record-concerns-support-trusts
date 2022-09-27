using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using Service.TRAMS.Helpers;
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
		private readonly ILogger<NtiService> _logger;
		private const string Url = @"/v2/case-actions/notice-to-improve";

		public NtiService(IHttpClientFactory httpClientFactory, ILogger<NtiService> logger, ICorrelationContext correlationContext) : base(httpClientFactory, logger, correlationContext)
		{
			_logger = logger;
		}

		public async Task<NtiDto> CreateNtiAsync(NtiDto newNti)
		{
			try
			{
				_logger.LogInformation($"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");

				var client = ClientFactory.CreateClient(HttpClientName);
				var request = new HttpRequestMessage(HttpMethod.Post, $"{Url}");

				request.Content = new StringContent(JsonConvert.SerializeObject(newNti),
					Encoding.UTF8, MediaTypeNames.Application.Json);

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiDto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}

		public async Task<ICollection<NtiDto>> GetNtisForCaseAsync(long caseUrn)
		{
			try
			{
				_logger.LogInformation($"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");

				var client = ClientFactory.CreateClient(HttpClientName);
				var request = new HttpRequestMessage(HttpMethod.Get, $"{Url}/case/{caseUrn}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<List<NtiDto>>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}

		public async Task<NtiDto> GetNtiAsync(long ntiId)
		{
			try
			{
				_logger.LogInformation($"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");

				var request = new HttpRequestMessage(HttpMethod.Get, $"{Url}/{ntiId}");

				var client = ClientFactory.CreateClient(HttpClientName);

				var response = await client.SendAsync(request);

				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiDto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}

		public async Task<NtiDto> PatchNtiAsync(NtiDto nti)
		{
			try
			{
				_logger.LogInformation($"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");

				var client = ClientFactory.CreateClient(HttpClientName);
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{Url}");

				request.Content = new StringContent(JsonConvert.SerializeObject(nti),
					Encoding.UTF8, MediaTypeNames.Application.Json);

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				response.EnsureSuccessStatusCode();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiDto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(NtiService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}
	}
}
