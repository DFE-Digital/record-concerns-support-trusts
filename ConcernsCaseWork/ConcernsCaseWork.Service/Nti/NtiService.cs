using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace ConcernsCaseWork.Service.Nti
{
	public class NtiService : ConcernsAbstractService, INtiService
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

				var client = CreateHttpClient();
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

				var client = CreateHttpClient();
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

				var client = CreateHttpClient();

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

				var client = CreateHttpClient();
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
