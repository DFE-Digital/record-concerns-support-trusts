﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiWarningLetter
{
	public class NtiWarningLetterService : AbstractService, INtiWarningLetterService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<NtiWarningLetterService> _logger;
		private const string Url = @"/v2/case-actions/nti-warning-letter";

		public NtiWarningLetterService(IHttpClientFactory httpClientFactory, 

			ILogger<NtiWarningLetterService> logger) : base(httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		public async Task<NtiWarningLetterDto> CreateNtiWarningLetterAsync(NtiWarningLetterDto newNtiWarningLetter)
		{
			try
			{
				var client = _httpClientFactory.CreateClient(HttpClientName);
				var request = new HttpRequestMessage(HttpMethod.Post, $"{Url}");

				request.Content = new StringContent(JsonConvert.SerializeObject(newNtiWarningLetter),
					Encoding.UTF8, MediaTypeNames.Application.Json);

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<NtiWarningLetterDto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to create NTI WL");
				throw;
			}
		}

		public async Task<NtiWarningLetterDto> GetNtiWarningLetterAsync(long ntiWarningLetterId)
		{
			// Create a request
			var request = new HttpRequestMessage(HttpMethod.Get, $"{Url}/{ntiWarningLetterId}");

			// Create http client
			var client = ClientFactory.CreateClient(HttpClientName);

			// Execute request
			var response = await client.SendAsync(request);

			// Check status code
			response.EnsureSuccessStatusCode();

			// Read response content
			var content = await response.Content.ReadAsStringAsync();

			// Deserialize content to POJO
			var apiWrapperNTIWarningLetterDto = JsonConvert.DeserializeObject<ApiWrapper<NtiWarningLetterDto>>(content);

			// Unwrap response
			if (apiWrapperNTIWarningLetterDto is { Data: { } })
			{
				return apiWrapperNTIWarningLetterDto.Data;
			}

			throw new Exception("API error unwrap response");
		}

		public async Task<ICollection<NtiWarningLetterDto>> GetNtiWarningLettersForCaseAsync(long caseUrn)
		{
			try
			{
				var client = _httpClientFactory.CreateClient(HttpClientName);
				var request = new HttpRequestMessage(HttpMethod.Get, $"{Url}/case/{caseUrn}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<List<NtiWarningLetterDto>>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to GetNtisForCase");
				throw;
			}
		}

		public Task<NtiWarningLetterDto> PatchNtiWarningLetterAsync(NtiWarningLetterDto ntiWarningLetter)
		{
			throw new NotImplementedException();
		}
	}
}
