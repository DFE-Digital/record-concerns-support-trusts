﻿using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.TRAMS.Type
{
	public sealed class RecordWhistleblowerService : AbstractService, IRecordWhistleblowerService
	{
		private readonly ILogger<RecordWhistleblowerService> _logger;
		
		public RecordWhistleblowerService(IHttpClientFactory clientFactory, ILogger<RecordWhistleblowerService> logger) : base(clientFactory)
		{
			_logger = logger;
		}
		
		public async Task<IList<RecordWhistleblowerDto>> GetRecordsWhistleblowerByRecordUrn(BigInteger recordUrn)
		{
			try
			{
				_logger.LogInformation("RecordWhistleblowerService::GetRecordsWhistleBlowingByRecordUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"{EndpointsVersion}/record-whistleblower/record/urn/{recordUrn}");
				
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
				var recordsWhistleblowerDto = JsonSerializer.Deserialize<IList<RecordWhistleblowerDto>>(content, options);

				return recordsWhistleblowerDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordWhistleblowerService::GetRecordsWhistleBlowingByRecordUrn::Exception message::{ex.Message}");
			}
			
			return Array.Empty<RecordWhistleblowerDto>();
		}

		public async Task<RecordWhistleblowerDto> PostRecordWhistleblowerByRecordUrn(CreateRecordWhistleblowerDto createRecordWhistleblowerDto)
		{
			try
			{
				_logger.LogInformation("RecordWhistleblowerService::PostRecordWhistleblowerByRecordUrn");
				
				// Create a request
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var request = new StringContent(
					JsonSerializer.Serialize(createRecordWhistleblowerDto, options),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PostAsync(
					$"{EndpointsVersion}/record-whistleblower/record/urn/{createRecordWhistleblowerDto.RecordUrn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var newRecordWhistleblowerDto = JsonSerializer.Deserialize<RecordWhistleblowerDto>(content, options);

				return newRecordWhistleblowerDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordWhistleblowerService::PostRecordWhistleblowerByRecordUrn::Exception message::{ex.Message}");
			}
			
			return null;
		}

		public async Task<RecordWhistleblowerDto> PatchRecordWhistleblowerByUrn(RecordWhistleblowerDto recordWhistleblowerDto)
		{
			try
			{
				_logger.LogInformation("RecordWhistleblowerService::PatchRecordWhistleblowerByUrn");
				
				// Create a request
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var request = new StringContent(
					JsonSerializer.Serialize(recordWhistleblowerDto, options),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PatchAsync(
					$"{EndpointsVersion}/record-whistleblower/urn/{recordWhistleblowerDto.Urn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var updatedRecordWhistleblowerDto = JsonSerializer.Deserialize<RecordWhistleblowerDto>(content, options);

				return updatedRecordWhistleblowerDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordWhistleblowerService::PatchRecordWhistleblowerByUrn::Exception message::{ex.Message}");
			}
			
			return null;
		}
	}
}