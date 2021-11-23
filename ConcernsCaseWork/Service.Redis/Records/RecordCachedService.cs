using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.Records
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
			_logger.LogInformation("RecordCachedService::GetRecordsByCaseUrn");
			
			// Store in cache for 24 hours (default)
			var caseState = await GetData<UserState>(caseworker);
			if (caseState is null)
			{
				// Fetch records from Academies API
				var records = await _recordService.GetRecordsByCaseUrn(caseUrn);
				if (records is null) throw new ApplicationException("Error::RecordCachedService::GetRecordsByCaseUrn");
				
				caseState = new UserState
				{
					CasesDetails = { { caseUrn, 
						new CaseWrapper { 
							Records = records.ToDictionary(r => r.Urn, r => new RecordWrapper { RecordDto = r } )
						} 
					} }
				};
				
				await StoreData(caseworker, caseState);
				
				return records;
			}

			if (caseState.CasesDetails.ContainsKey(caseUrn) && caseState.CasesDetails.TryGetValue(caseUrn, out var caseWrapper))
			{
				return caseWrapper.Records.Values.Select(r => r.RecordDto).ToList();
			}

			return Array.Empty<RecordDto>();
		}
		
		public async Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto, string caseworker)
		{
			_logger.LogInformation("RecordCachedService::PostRecordByCaseUrn");
			
			// Create record on Academies API
			var newRecord = await _recordService.PostRecordByCaseUrn(createRecordDto);
			if (newRecord is null) throw new ApplicationException("Error::RecordCachedService::PostRecordByCaseUrn");
			
			// Store in cache for 24 hours (default)
			var caseState = await GetData<UserState>(caseworker);
			if (caseState is null)
			{
				caseState = new UserState
				{
					CasesDetails = { { newRecord.CaseUrn, 
						new CaseWrapper { 
							Records = { { newRecord.Urn, 
								new RecordWrapper { RecordDto = newRecord } }} 
						} 
					} }
				};
			}
			else
			{
				if (caseState.CasesDetails.ContainsKey(newRecord.CaseUrn) 
				    && caseState.CasesDetails.TryGetValue(newRecord.CaseUrn, out var caseWrapper))
				{
					caseWrapper.Records.Add(newRecord.Urn, new RecordWrapper {  RecordDto = newRecord });
				}
				else
				{
					caseWrapper = new CaseWrapper();
					caseWrapper.Records.Add(newRecord.Urn, new RecordWrapper {  RecordDto = newRecord });
					
					caseState.CasesDetails.Add(newRecord.CaseUrn, caseWrapper);
				}
			}
			await StoreData(caseworker, caseState);

			return newRecord;
		}

		public async Task PatchRecordByUrn(RecordDto recordDto, string caseworker)
		{
			_logger.LogInformation("RecordCachedService::PatchRecordByUrn");
			
			// Patch record on Academies API
			var patchRecord = await _recordService.PatchRecordByUrn(recordDto);
			if (patchRecord is null) throw new ApplicationException("Error::RecordCachedService::PatchRecordByUrn");
			
			// Store in cache for 24 hours (default)
			var caseState = await GetData<UserState>(caseworker);
			if (caseState is null)
			{
				caseState = new UserState
				{
					CasesDetails = { { patchRecord.CaseUrn, 
						new CaseWrapper { 
							Records = { { patchRecord.Urn, 
								new RecordWrapper { RecordDto = patchRecord } }} 
						} 
					} }
				};
			}
			else
			{
				if (caseState.CasesDetails.ContainsKey(patchRecord.CaseUrn) 
				    && caseState.CasesDetails.TryGetValue(patchRecord.CaseUrn, out var caseWrapper)
				    && caseWrapper.Records.TryGetValue(patchRecord.Urn, out var recordWrapper))
				{
					recordWrapper.RecordDto = patchRecord;
				}
				else
				{
					caseWrapper = new CaseWrapper();
					caseWrapper.Records.Add(patchRecord.Urn, new RecordWrapper {  RecordDto = patchRecord });
					
					caseState.CasesDetails.Add(patchRecord.CaseUrn, caseWrapper);
				}
			}
			await StoreData(caseworker, caseState);
		}
	}
}