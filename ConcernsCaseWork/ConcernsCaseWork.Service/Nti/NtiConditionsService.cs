using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Helpers;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Nti
{
	public class NtiConditionsService(IHttpClientFactory httpClientFactory, ILogger<NtiConditionsService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService, IUserTokenService userTokenService) : ConcernsAbstractService(httpClientFactory, logger, correlationContext, userInfoService, userTokenService), INtiConditionsService
	{
		public async Task<ICollection<NtiConditionDto>> GetAllConditionsAsync()
		{
			try
			{
				logger.LogInformation($"{nameof(NtiConditionsService)}::{LoggingHelpers.EchoCallerName()}");

				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/case-actions/notice-to-improve/all-conditions");

				// Create http client
				var client = CreateHttpClient();

				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();

				// Read response content
				var content = await response.Content.ReadAsStringAsync();

				// Deserialize content to POCO
				var apiWrapperRatingsDto = JsonConvert.DeserializeObject<ApiListWrapper<NtiConditionDto>>(content);

				return apiWrapperRatingsDto.Data;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"{nameof(NtiConditionsService)}::{LoggingHelpers.EchoCallerName()}");
				throw;
			}
		}
	}
}
