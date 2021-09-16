using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.RecordRatingHistory;
using System.Threading.Tasks;

namespace Service.Redis.RecordRatingHistory
{
	public sealed class RecordRatingHistoryCachedService : CachedService, IRecordRatingHistoryCachedService
	{
		private readonly ILogger<RecordRatingHistoryCachedService> _logger;
		private readonly IRecordRatingHistoryService _recordRatingHistoryService;
		
		public RecordRatingHistoryCachedService(ICacheProvider cacheProvider, IRecordRatingHistoryService recordRatingHistoryService, 
			ILogger<RecordRatingHistoryCachedService> logger) : base(cacheProvider)
		{
			_recordRatingHistoryService = recordRatingHistoryService;
			_logger = logger;
		}

		public async Task<RecordRatingHistoryDto> PostRecordRatingHistory(RecordRatingHistoryDto recordRatingHistoryDto, string caseworker, long caseUrn)
		{
			_logger.LogInformation("RecordRatingHistoryCachedService::PostRecordRatingHistory");
			
			// TODO Enable only when TRAMS API is live
			//var newRrh = await _recordRatingHistoryService.PostRecordRatingHistory(recordRatingHistoryDto);
			//if (newRrh is null) throw new ApplicationException("Error::RecordRatingHistoryCachedService::PostRecordRatingHistory");
			
			// Store in cache for 24 hours (default)
			var caseState = await GetData<UserState>(caseworker);
			if (caseState is null)
			{
				var recordWrapper = new RecordWrapper();
				recordWrapper.RecordsRatingHistory.Add(recordRatingHistoryDto);
				var caseWrapper = new CaseWrapper { Records = { { recordRatingHistoryDto.RecordUrn, recordWrapper } } };
				
				caseState = new UserState
				{
					CasesDetails = { { caseUrn, caseWrapper } }
				};
			}
			else
			{
				if (caseState.CasesDetails.ContainsKey(caseUrn) 
				    && caseState.CasesDetails.TryGetValue(caseUrn, out var caseWrapper)
				    && caseWrapper.Records.TryGetValue(recordRatingHistoryDto.RecordUrn, out var recordWrapper))
				{
					recordWrapper.RecordsRatingHistory.Add(recordRatingHistoryDto);
				}
				else
				{
					recordWrapper = new RecordWrapper();
					recordWrapper.RecordsRatingHistory.Add(recordRatingHistoryDto);
					
					caseWrapper = new CaseWrapper();
					caseWrapper.Records.Add(recordRatingHistoryDto.RecordUrn, recordWrapper);
					
					caseState.CasesDetails.Add(caseUrn, caseWrapper);
				}
			}
			await StoreData(caseworker, caseState);

			return recordRatingHistoryDto;
		}
	}
}