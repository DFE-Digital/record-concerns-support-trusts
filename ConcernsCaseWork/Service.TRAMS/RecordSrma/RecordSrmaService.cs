using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
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

		public async Task<IList<RecordSrmaDto>> GetRecordsSrmaByRecordUrn(string recordUrn)
		{
			try
			{
				_logger.LogInformation("RecordSrmaService::GetRecordsSrmaByRecordUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/record-srma/record/urn/{recordUrn}");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var recordsSrmaDto = JsonSerializer.Deserialize<IList<RecordSrmaDto>>(content, options);

				return recordsSrmaDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordSrmaService::GetRecordsSrmaByRecordUrn::Exception message::{ex.Message}");
			}
			
			return Array.Empty<RecordSrmaDto>();
		}

		public async Task<RecordSrmaDto> PostRecordSrmaByRecordUrn(RecordSrmaDto recordSrmaDto)
		{
			try
			{
				_logger.LogInformation("RecordSrmaService::PostRecordSrmaByRecordUrn");
				
				// Create a request
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var request = new StringContent(
					JsonSerializer.Serialize(recordSrmaDto, options),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PostAsync($"/record-srma/record/urn/{recordSrmaDto.RecordUrn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var newRecordSrmaDto = JsonSerializer.Deserialize<RecordSrmaDto>(content, options);

				return newRecordSrmaDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordSrmaService::PostRecordSrmaByRecordUrn::Exception message::{ex.Message}");
			}
			
			return null;
		}

		public async Task<RecordSrmaDto> PatchRecordSrmaByUrn(RecordSrmaDto recordSrmaDto)
		{
			try
			{
				_logger.LogInformation("RecordSrmaService::PatchRecordSrmaByUrn");
				
				// Create a request
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var request = new StringContent(
					JsonSerializer.Serialize(recordSrmaDto, options),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PatchAsync($"/record-srma/urn/{recordSrmaDto.RecordUrn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var updatedRecordSrmaDto = JsonSerializer.Deserialize<RecordSrmaDto>(content, options);

				return updatedRecordSrmaDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordSrmaService::PatchRecordSrmaByUrn::Exception message::{ex.Message}");
			}
			
			return null;
		}
	}
}