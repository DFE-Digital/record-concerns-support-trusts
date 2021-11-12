using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordSrma
{
	public sealed class RecordSrmaService : AbstractService, IRecordSrmaService
	{
		private readonly ILogger<RecordSrmaService> _logger;
		
		public RecordSrmaService(IHttpClientFactory clientFactory, ILogger<RecordSrmaService> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<IList<RecordSrmaDto>> GetRecordsSrmaByRecordUrn(long recordUrn)
		{
			try
			{
				_logger.LogInformation("RecordSrmaService::GetRecordsSrmaByRecordUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"/{EndpointsVersion}/record-srma/record/urn/{recordUrn}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var recordsSrmaDto = JsonConvert.DeserializeObject<IList<RecordSrmaDto>>(content);

				return recordsSrmaDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordSrmaService::GetRecordsSrmaByRecordUrn::Exception message::{Message}", ex.Message);
			}
			
			return Array.Empty<RecordSrmaDto>();
		}

		public async Task<RecordSrmaDto> PostRecordSrmaByRecordUrn(CreateRecordSrmaDto createRecordSrmaDto)
		{
			try
			{
				_logger.LogInformation("RecordSrmaService::PostRecordSrmaByRecordUrn");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(createRecordSrmaDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.PostAsync(
					$"/{EndpointsVersion}/record-srma/record/urn/{createRecordSrmaDto.RecordUrn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var newRecordSrmaDto = JsonConvert.DeserializeObject<RecordSrmaDto>(content);

				return newRecordSrmaDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordSrmaService::PostRecordSrmaByRecordUrn::Exception message::{Message}", ex.Message);
			}
			
			return null;
		}

		public async Task<RecordSrmaDto> PatchRecordSrmaByUrn(RecordSrmaDto recordSrmaDto)
		{
			try
			{
				_logger.LogInformation("RecordSrmaService::PatchRecordSrmaByUrn");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(recordSrmaDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.PatchAsync(
					$"/{EndpointsVersion}/record-srma/urn/{recordSrmaDto.RecordUrn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var updatedRecordSrmaDto = JsonConvert.DeserializeObject<RecordSrmaDto>(content);

				return updatedRecordSrmaDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordSrmaService::PatchRecordSrmaByUrn::Exception message::{Message}", ex.Message);
			}
			
			return null;
		}
	}
}