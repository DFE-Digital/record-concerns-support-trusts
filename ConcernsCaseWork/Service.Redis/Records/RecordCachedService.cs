using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Records;
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

			// Create record on TRAMS API
			var newRecord = await _recordService.PostRecordByCaseUrn(createRecordDto);

			// Store in cache for 24 hours (default)
			var caseState = await GetData<UserState>(caseworker);
			if (caseState is null)
			{
				caseState = new UserState
				{
					CasesDetails = { { newRecord.CaseUrn, 
						new CaseWrapper { 
							Records = { { newRecord.Urn, newRecord }} 
						} 
					} }
				};
			}
			else
			{
				if (caseState.CasesDetails.ContainsKey(newRecord.CaseUrn) 
				    && caseState.CasesDetails.TryGetValue(newRecord.CaseUrn, out var caseWrapper))
				{
					caseWrapper.Records.Add(newRecord.Urn, newRecord );
				}
				else
				{
					caseWrapper = new CaseWrapper();
					caseWrapper.Records.Add(newRecord.Urn, newRecord);
					
					caseState.CasesDetails.Add(newRecord.CaseUrn, caseWrapper);
				}
			}
			await StoreData(caseworker, caseState);
			
			return newRecord;
		}
	}
}