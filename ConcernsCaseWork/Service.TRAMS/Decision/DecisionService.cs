using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using Service.TRAMS.Helpers;
using Service.TRAMS.Nti;
using Service.TRAMS.Records;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Decision
{
	public class DecisionService :  AbstractService, IDecisionService
	{
		private readonly ILogger<DecisionService> _logger;

		public DecisionService(IHttpClientFactory clientFactory, ILogger<DecisionService> logger) : base(clientFactory, logger)
		{
			_logger = logger;
		}

		public async Task PostDecision(CreateDecisionDto createDecisionDto)
		{
			try
			{
				_logger.LogInformation($"{nameof(DecisionService)}::{LoggingHelpers.EchoCallerName()}");

				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(createDecisionDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);

				// Execute request
				var response = await client.PostAsync(
					$"/{EndpointsVersion}/concerns-cases/{createDecisionDto.ConcernsCaseUrn}/decisions", request);

				// Check status code
				response.EnsureSuccessStatusCode();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(DecisionService)}::{LoggingHelpers.EchoCallerName()}");

				throw;
			}

		}

	}
}

