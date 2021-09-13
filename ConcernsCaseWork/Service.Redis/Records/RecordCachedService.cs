using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using Service.TRAMS.Records;
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
		
		public async Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto, string caseworker)
		{
			_logger.LogInformation("RecordCachedService::PostRecordByCaseUrn");

			// TODO Enable only when TRAMS API is live
			// Create record on TRAMS API
			//var newRecord = await _recordService.PostRecordByCaseUrn(createRecordDto);
			//if (newRecord is null) throw new ApplicationException("Error::RecordCachedService::PostRecordByCaseUrn");

			// TODO Remove when TRAMS API is live
			var createRecordDtoStr = JsonConvert.SerializeObject(createRecordDto);
			var newRecord = JsonConvert.DeserializeObject<RecordDto>(createRecordDtoStr);
			
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

		public async Task<IList<RecordDto>> GetRecordsByCaseUrn(CaseDto caseDto)
		{
			_logger.LogInformation("RecordCachedService::GetRecordsByCaseUrn");

			var records = new List<RecordDto>();
			
			// Store in cache for 24 hours (default)
			var caseState = await GetData<UserState>(caseDto.CreatedBy);
			if (caseState is null)
			{
				// TODO Enable only when TRAMS API is live
				// Fetch records from TRAMS API
				// var records = await _recordService.GetRecordsByCaseUrn(caseDto.Urn);
				// if (records is null) throw new ApplicationException("Error::RecordCachedService::GetRecordsByCaseUrn");
				
				// TODO Finish cache logic
			}
			else
			{
				if (caseState.CasesDetails.ContainsKey(caseDto.Urn) && caseState.CasesDetails.TryGetValue(caseDto.Urn, out var caseWrapper))
				{
					return caseWrapper.Records.Values.Select(r => r.RecordDto).ToList();
				}
			}
			await StoreData(caseDto.CreatedBy, caseState);

			return records;
		}
	}
}