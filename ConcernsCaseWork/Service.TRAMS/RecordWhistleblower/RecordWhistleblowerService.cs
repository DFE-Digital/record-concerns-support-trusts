using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordWhistleblower
{
	public sealed class RecordWhistleblowerService : AbstractService, IRecordWhistleblowerService
	{
		private readonly ILogger<RecordWhistleblowerService> _logger;
		
		public RecordWhistleblowerService(IHttpClientFactory clientFactory, ILogger<RecordWhistleblowerService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = logger;
		}
		
		public async Task<IList<RecordWhistleblowerDto>> GetRecordsWhistleblowerByRecordUrn(long recordUrn)
		{
			try
			{
				_logger.LogInformation("RecordWhistleblowerService::GetRecordsWhistleBlowingByRecordUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"/{EndpointsVersion}/record-whistleblower/record/urn/{recordUrn}");
				
				// Create http client
				var client = CreateHttpClient();
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var recordsWhistleblowerDto = JsonConvert.DeserializeObject<IList<RecordWhistleblowerDto>>(content);

				return recordsWhistleblowerDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordWhistleblowerService::GetRecordsWhistleBlowingByRecordUrn::Exception message::{Message}", ex.Message);
			}
			
			return Array.Empty<RecordWhistleblowerDto>();
		}

		public async Task<RecordWhistleblowerDto> PostRecordWhistleblowerByRecordUrn(CreateRecordWhistleblowerDto createRecordWhistleblowerDto)
		{
			try
			{
				_logger.LogInformation("RecordWhistleblowerService::PostRecordWhistleblowerByRecordUrn");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(createRecordWhistleblowerDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = CreateHttpClient();
				
				// Execute request
				var response = await client.PostAsync(
					$"/{EndpointsVersion}/record-whistleblower/record/urn/{createRecordWhistleblowerDto.RecordUrn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var newRecordWhistleblowerDto = JsonConvert.DeserializeObject<RecordWhistleblowerDto>(content);

				return newRecordWhistleblowerDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordWhistleblowerService::PostRecordWhistleblowerByRecordUrn::Exception message::{Message}", ex.Message);
			}
			
			return null;
		}

		public async Task<RecordWhistleblowerDto> PatchRecordWhistleblowerByUrn(RecordWhistleblowerDto recordWhistleblowerDto)
		{
			try
			{
				_logger.LogInformation("RecordWhistleblowerService::PatchRecordWhistleblowerByUrn");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(recordWhistleblowerDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = CreateHttpClient();
				
				// Execute request
				var response = await client.PatchAsync(
					$"/{EndpointsVersion}/record-whistleblower/urn/{recordWhistleblowerDto.Urn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var updatedRecordWhistleblowerDto = JsonConvert.DeserializeObject<RecordWhistleblowerDto>(content);

				return updatedRecordWhistleblowerDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordWhistleblowerService::PatchRecordWhistleblowerByUrn::Exception message::{Message}", ex.Message);
			}
			
			return null;
		}
	}
}