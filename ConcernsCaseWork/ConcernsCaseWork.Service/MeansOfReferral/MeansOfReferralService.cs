using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.MeansOfReferral
{
	public sealed class MeansOfReferralService : ConcernsAbstractService, IMeansOfReferralService
	{
		private readonly ILogger<MeansOfReferralService> _logger;
		
		public MeansOfReferralService(IHttpClientFactory clientFactory, ILogger<MeansOfReferralService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = logger;
		}

		public async Task<IList<MeansOfReferralDto>> GetMeansOfReferrals()
		{
			try
			{
				_logger.LogInformation("{ClassName}::{MethodName}", nameof(MeansOfReferralService), nameof(GetMeansOfReferrals));
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/{EndpointsVersion}/concerns-meansofreferral");
				
				// Create http client
				var client = CreateHttpClient();
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperMeansOfReferralsDto = JsonConvert.DeserializeObject<ApiListWrapper<MeansOfReferralDto>>(content);

				// Unwrap response
				if (apiWrapperMeansOfReferralsDto is { Data: { } })
				{
					return apiWrapperMeansOfReferralsDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("{ClassName}::{MethodName}::Exception message::{Message}", nameof(MeansOfReferralService), nameof(GetMeansOfReferrals), ex.Message);

				throw;
			}
		}
	}
}