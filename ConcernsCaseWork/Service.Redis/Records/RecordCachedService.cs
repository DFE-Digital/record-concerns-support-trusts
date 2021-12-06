using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Records;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Redis.Records
{
	public sealed class RecordCachedService : CachedService, IRecordCachedService
	{
		private readonly ILogger<RecordCachedService> _logger;
		private readonly IRecordService _recordService;
		
		private readonly SemaphoreSlim _semaphoreRecordsCase = new SemaphoreSlim(1, 1);
		
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
			
			// If case urn doesn't exist on the cache return empty records
			if (!userState.CasesDetails.TryGetValue(caseUrn, out var caseWrapper)) return recordsDto;
			
			if (caseWrapper.Records.Any())
			{
				recordsDto = caseWrapper.Records.Values.Select(r => r.RecordDto).ToList();
			}
			else 
			{
				recordsDto = await _recordService.GetRecordsByCaseUrn(caseUrn);
				if (!recordsDto.Any()) return recordsDto;
				
				await _semaphoreRecordsCase.WaitAsync();
				
				userState = await GetData<UserState>(caseworker);
				userState ??= new UserState();
				
				if(userState.CasesDetails.TryGetValue(caseUrn, out caseWrapper)) 
				{
					caseWrapper.Records = recordsDto.ToDictionary(recordDto => recordDto.Urn, recordDto => new RecordWrapper { RecordDto = recordDto } );
						
					await StoreData(caseworker, userState);
				}
				
				_semaphoreRecordsCase.Release();
			}

			return recordsDto;
		}
		
		public async Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto, string caseworker)
		{
			_logger.LogInformation("RecordCachedService::PostRecordByCaseUrn {Caseworker} - {CaseUrn}", caseworker, createRecordDto.CaseUrn);
			
			// Create record on Academies API
			var newRecordDto = await _recordService.PostRecordByCaseUrn(createRecordDto);
			
			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(caseworker);
			if (userState is null) return newRecordDto;
			
			// If case urn doesn't exist on the cache return new created record
			if (!userState.CasesDetails.TryGetValue(newRecordDto.CaseUrn, out var caseWrapper)) return newRecordDto;
			
			// Add new record to the records cache
			caseWrapper.Records.Add(newRecordDto.Urn, new RecordWrapper {  RecordDto = newRecordDto });
			
			await StoreData(caseworker, userState);
			
			return newRecordDto;
		}
		
		public async Task PatchRecordByUrn(RecordDto recordDto, string caseworker)
		{
			_logger.LogInformation("RecordCachedService::PatchRecordByUrn {Caseworker} - {CaseUrn}", caseworker, recordDto.CaseUrn);
			
			// Patch record on Academies API
			var patchRecordDto = await _recordService.PatchRecordByUrn(recordDto);
			
			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(caseworker);
			if (userState is null) return;
			
			// If case urn doesn't exist on the cache return
			if (!userState.CasesDetails.TryGetValue(patchRecordDto.CaseUrn, out var caseWrapper)) return;
			
			if (caseWrapper.Records.TryGetValue(patchRecordDto.Urn, out var recordWrapper))
			{
				recordWrapper.RecordDto = patchRecordDto;
			}
			else
			{
				caseWrapper.Records.Add(patchRecordDto.Urn, new RecordWrapper {  RecordDto = patchRecordDto });
			}
			
			await StoreData(caseworker, userState);
		}
	}
}