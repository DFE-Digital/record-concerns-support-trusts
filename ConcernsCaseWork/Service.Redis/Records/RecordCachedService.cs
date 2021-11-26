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
			if (caseState is null || (caseState.CasesDetails.TryGetValue(caseUrn, out var caseWrapper) && !caseWrapper.Records.Any()))
			{
				// Fetch records from Academies API
				var recordsDto = await _recordService.GetRecordsByCaseUrn(caseUrn);
				if (!recordsDto.Any()) return recordsDto;
				
				caseState = new UserState
				{
					CasesDetails = { { caseUrn, 
						new CaseWrapper { 
							Records = recordsDto.ToDictionary(r => r.Urn, r => new RecordWrapper { RecordDto = r } )
						} 
					} }
				};
				
				await StoreData(caseworker, caseState);
				
				return recordsDto;
			}

			if (caseState.CasesDetails.ContainsKey(caseUrn) && caseState.CasesDetails.TryGetValue(caseUrn, out caseWrapper))
			{
				return caseWrapper.Records.Values.Select(r => r.RecordDto).ToList();
			}

			return Array.Empty<RecordDto>();
		}
		
		public async Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto, string caseworker)
		{
			_logger.LogInformation("RecordCachedService::PostRecordByCaseUrn");
			
			// Create record on Academies API
			var newRecordDto = await _recordService.PostRecordByCaseUrn(createRecordDto);
			if (newRecordDto is null) throw new ApplicationException("Error::RecordCachedService::PostRecordByCaseUrn");
			
			// Store in cache for 24 hours (default)
			var caseState = await GetData<UserState>(caseworker);
			if (caseState is null)
			{
				caseState = new UserState
				{
					CasesDetails = { { newRecordDto.CaseUrn, 
						new CaseWrapper { 
							Records = { { newRecordDto.Urn, 
								new RecordWrapper { RecordDto = newRecordDto } }} 
						} 
					} }
				};
			}
			else
			{
				if (caseState.CasesDetails.ContainsKey(newRecordDto.CaseUrn) 
				    && caseState.CasesDetails.TryGetValue(newRecordDto.CaseUrn, out var caseWrapper))
				{
					caseWrapper.Records.Add(newRecordDto.Urn, new RecordWrapper {  RecordDto = newRecordDto });
				}
				else
				{
					caseWrapper = new CaseWrapper();
					caseWrapper.Records.Add(newRecordDto.Urn, new RecordWrapper {  RecordDto = newRecordDto });
					
					caseState.CasesDetails.Add(newRecordDto.CaseUrn, caseWrapper);
				}
			}
			await StoreData(caseworker, caseState);

			return newRecordDto;
		}

		public async Task PatchRecordByUrn(RecordDto recordDto, string caseworker)
		{
			_logger.LogInformation("RecordCachedService::PatchRecordByUrn");
			
			// Patch record on Academies API
			var patchRecordDto = await _recordService.PatchRecordByUrn(recordDto);
			if (patchRecordDto is null) throw new ApplicationException("Error::RecordCachedService::PatchRecordByUrn");
			
			// Store in cache for 24 hours (default)
			var caseState = await GetData<UserState>(caseworker);
			if (caseState is null)
			{
				caseState = new UserState
				{
					CasesDetails = { { patchRecordDto.CaseUrn, 
						new CaseWrapper { 
							Records = { { patchRecordDto.Urn, 
								new RecordWrapper { RecordDto = patchRecordDto } }} 
						} 
					} }
				};
			}
			else
			{
				if (caseState.CasesDetails.ContainsKey(patchRecordDto.CaseUrn) 
				    && caseState.CasesDetails.TryGetValue(patchRecordDto.CaseUrn, out var caseWrapper)
				    && caseWrapper.Records.TryGetValue(patchRecordDto.Urn, out var recordWrapper))
				{
					recordWrapper.RecordDto = patchRecordDto;
				}
				else
				{
					caseWrapper = new CaseWrapper();
					caseWrapper.Records.Add(patchRecordDto.Urn, new RecordWrapper {  RecordDto = patchRecordDto });
					
					caseState.CasesDetails.Add(patchRecordDto.CaseUrn, caseWrapper);
				}
			}
			await StoreData(caseworker, caseState);
		}
	}
}