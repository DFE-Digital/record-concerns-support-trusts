using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordAcademy
{
	public sealed class RecordAcademyService : AbstractService, IRecordAcademyService
	{
		private readonly ILogger<RecordAcademyService> _logger;
		
		public RecordAcademyService(IHttpClientFactory clientFactory, ILogger<RecordAcademyService> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<IList<RecordAcademyDto>> GetRecordsAcademyByRecordUrn(long recordUrn)
		{
			try
			{
				_logger.LogInformation("RecordAcademyService::GetRecordsAcademyByRecordUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"/{EndpointsVersion}/record-academy/record/urn/{recordUrn}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var recordsAcademyDto = JsonConvert.DeserializeObject<IList<RecordAcademyDto>>(content);

				return recordsAcademyDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordAcademyService::GetRecordsAcademyByRecordUrn::Exception message::{Message}", ex.Message);
			}
			
			return Array.Empty<RecordAcademyDto>();
		}

		public async Task<RecordAcademyDto> PostRecordAcademyByRecordUrn(CreateRecordAcademyDto createRecordAcademyDto)
		{
			try
			{
				_logger.LogInformation("RecordAcademyService::PostRecordAcademyByRecordUrn");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(createRecordAcademyDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.PostAsync(
					$"/{EndpointsVersion}/record-academy/record/urn/{createRecordAcademyDto.RecordUrn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var newRecordAcademyDto = JsonConvert.DeserializeObject<RecordAcademyDto>(content);

				return newRecordAcademyDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordAcademyService::PostRecordAcademyByRecordUrn::Exception message::{Message}", ex.Message);
			}
			
			return null;
		}

		public async Task<RecordAcademyDto> PatchRecordAcademyByUrn(RecordAcademyDto recordAcademyDto)
		{
			try
			{
				_logger.LogInformation("RecordAcademyService::PatchRecordAcademyByUrn");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(recordAcademyDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.PatchAsync(
					$"/{EndpointsVersion}/record-academy/urn/{recordAcademyDto.Urn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var updatedRecordAcademyDto = JsonConvert.DeserializeObject<RecordAcademyDto>(content);

				return updatedRecordAcademyDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordAcademyService::PatchRecordAcademyByUrn::Exception message::{Message}", ex.Message);
			}
			
			return null;
		}
	}
}