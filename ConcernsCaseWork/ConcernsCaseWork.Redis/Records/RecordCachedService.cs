﻿using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.Records;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Records
{
	public sealed class RecordCachedService : CachedService, IRecordCachedService
	{
		private readonly ILogger<RecordCachedService> _logger;
		private readonly IRecordService _recordService;

		public RecordCachedService(ICacheProvider cacheProvider, IRecordService recordService, ILogger<RecordCachedService> logger)
			: base(cacheProvider)
		{
			_recordService = recordService;
			_logger = logger;
		}

		public async Task<IList<RecordDto>> GetRecordsByCaseUrn(string caseworker, long caseUrn)
		{
			_logger.LogInformation("RecordCachedService::GetRecordsByCaseUrn {Caseworker} - {CaseUrn}", caseworker, caseUrn);

			IList<RecordDto> recordsDto = new List<RecordDto>();

			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(caseworker);
			if (userState is null) return recordsDto;

			// If case urn doesn't exist on the cache may mean it belongs to another caseworker
			// Edge case fetching records that don't belong to the logged casework
			if (!userState.CasesDetails.TryGetValue(caseUrn, out var caseWrapper))
			{
				recordsDto = await _recordService.GetRecordsByCaseUrn(caseUrn);

				return recordsDto;
			}

			// Checking records was cached before
			if (caseWrapper.Records != null)
			{
				if (!caseWrapper.Records.Any()) return recordsDto;
				recordsDto = caseWrapper.Records.Values.Select(r => r.RecordDto).ToList();
			}
			else
			{
				recordsDto = await _recordService.GetRecordsByCaseUrn(caseUrn);

				userState = await GetData<UserState>(caseworker);

				if (userState.CasesDetails.TryGetValue(caseUrn, out caseWrapper))
				{
					caseWrapper.Records ??= new Dictionary<long, RecordWrapper>();

					if (recordsDto.Any())
					{
						caseWrapper.Records = recordsDto.ToDictionary(recordDto => recordDto.Id, recordDto => new RecordWrapper { RecordDto = recordDto });
					}

					await StoreData(caseworker, userState);
				}
			}

			return recordsDto;
		}

		public async Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto, string caseworker)
		{
			_logger.LogInformation("RecordCachedService::PostRecordByCaseUrn {Caseworker} - {CaseUrn}", caseworker, createRecordDto.CaseUrn);

			// Create record on Academies API
			var newRecordDto = await _recordService.PostRecordByCaseUrn(createRecordDto);

			var userState = await GetData<UserState>(caseworker);
			if (userState is null) return newRecordDto;

			// If case urn doesn't exist on the cache return new created record
			if (!userState.CasesDetails.TryGetValue(newRecordDto.CaseUrn, out var caseWrapper)) return newRecordDto;

			// Add new record to the records cache
			caseWrapper.Records ??= new ConcurrentDictionary<long, RecordWrapper>();
			caseWrapper.Records.Add(newRecordDto.Id, new RecordWrapper { RecordDto = newRecordDto });

			await StoreData(caseworker, userState);

			return newRecordDto;
		}

		public async Task PatchRecordById(RecordDto recordDto, string caseworker)
		{
			_logger.LogInformation("RecordCachedService::PatchRecordById {Caseworker} - {CaseUrn}", caseworker, recordDto.CaseUrn);

			// Patch record on Academies API
			var patchRecordDto = await _recordService.PatchRecordById(recordDto);

			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(caseworker);
			if (userState is null) return;

			// If case urn doesn't exist on the cache return
			if (!userState.CasesDetails.TryGetValue(patchRecordDto.CaseUrn, out var caseWrapper)) return;

			if (caseWrapper.Records.TryGetValue(patchRecordDto.Id, out var recordWrapper))
			{
				recordWrapper.RecordDto = patchRecordDto;
			}
			else
			{
				caseWrapper.Records.Add(patchRecordDto.Id, new RecordWrapper { RecordDto = patchRecordDto });
			}

			await StoreData(caseworker, userState);
		}
	}
}