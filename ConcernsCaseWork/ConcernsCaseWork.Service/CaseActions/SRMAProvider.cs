﻿using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace ConcernsCaseWork.Service.CaseActions
{
	public class SRMAProvider : ConcernsAbstractService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<SRMAProvider> _logger;
		private const string Url = @"/v2/case-actions/srma";
		public SRMAProvider(IHttpClientFactory httpClientFactory, ILogger<SRMAProvider> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService) : base(httpClientFactory, logger, correlationContext, userInfoService)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		public async Task<SRMADto> GetSRMAById(long srmaId)
		{
			var client = CreateHttpClient();
			var request = new HttpRequestMessage(HttpMethod.Get, $"{Url}?srmaId={srmaId}");

			try
			{
				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				var wrapper = JsonConvert.DeserializeObject<ApiWrapper<SRMADto>>(content);
				return wrapper.Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to GetSRMAById");
				throw;
			}
		}

		public async Task<List<SRMADto>> GetSRMAsForCase(long caseUrn)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Get, $"{Url}/case/{caseUrn}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<List<SRMADto>>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to GetSRMAById");
				throw;
			}
		}

		public async Task<SRMADto> SaveSRMA(SRMADto srma)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Post, $"{Url}");

				request.Content = new StringContent(JsonConvert.SerializeObject(srma),
					Encoding.UTF8, MediaTypeNames.Application.Json);

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<SRMADto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to create SRMA");
				throw;
			}
		}

		public async Task<SRMADto> SetDateAccepted(long srmaId, DateTime? acceptedDate)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{Url}/{srmaId}/update-date-accepted?acceptedDate={SerialiseDateTime(acceptedDate)}");

				var response = await client.SendAsync(request); 
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<SRMADto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to set SRMA date accepted");
				throw;
			}
		}

		public async Task<SRMADto> SetDateClosed(long srmaId)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{Url}/{srmaId}/update-closed-date");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<SRMADto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to set SRMA date closed");
				throw;
			}
		}

		public async Task<SRMADto> SetDateReportSent(long srmaId, DateTime? reportSentDate)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{Url}/{srmaId}/update-date-report-sent?dateReportSent={SerialiseDateTime(reportSentDate)}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<SRMADto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to set SRMA SetDateReportSent");
				throw;
			}
		}

		public async Task<SRMADto> SetNotes(long srmaId, string notes)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{Url}/{srmaId}/update-notes?notes={notes ?? String.Empty}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<SRMADto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to set SRMA notes");
				throw;
			}
		}

		public async Task<SRMADto> SetOfferedDate(long srmaId, DateTime offeredDate)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{Url}/{srmaId}/update-offered-date?offeredDate={SerialiseDateTime(offeredDate)}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<SRMADto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to set SRMA OfferedDate");
				throw;
			}
		}

		public async Task<SRMADto> SetReason(long srmaId, SRMAReasonOffered reason)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{Url}/{srmaId}/update-reason?reason={(int)reason}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<SRMADto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to set SRMA reason");
				throw;
			}
		}

		public async Task<SRMADto> SetStatus(long srmaId, SRMAStatus status)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{Url}/{srmaId}/update-status?status={(int)status}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<SRMADto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to set SRMA status");
				throw;
			}
		}

		public async Task<SRMADto> SetVisitDates(long srmaId, DateTime? startDate, DateTime? endDate)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Patch, $"{Url}/{srmaId}/update-visit-dates?startDate={SerialiseDateTime(startDate)}&endDate={SerialiseDateTime(endDate)}");

				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<ApiWrapper<SRMADto>>(content).Data;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to set SRMA OfferedDate");
				throw;
			}
		}

		public async Task DeleteSRMA(long srmaId)
		{
			try
			{
				var client = CreateHttpClient();
				var request = new HttpRequestMessage(HttpMethod.Delete, $"{Url}/{srmaId}");

				await client.SendAsync(request);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error occured while trying to delete SRMA");
				throw;
			}
		}


		private string SerialiseDateTime(DateTime dateTime)
		{
			return dateTime.ToString("dd-MM-yyyy");
		}

		private string SerialiseDateTime(DateTime? dateTime)
		{
			return dateTime == null ? String.Empty : SerialiseDateTime(dateTime.Value);	
		}
	}
}
