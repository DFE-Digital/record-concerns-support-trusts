using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.RecordRatingHistory;
using System;
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

		public async Task<RecordRatingHistoryDto> PostRecordRatingHistory(RecordRatingHistoryDto recordRatingHistoryDto, string caseworker)
		{
			_logger.LogInformation("RecordRatingHistoryCachedService::PostRecordRatingHistory");
			
			var newRrh = await _recordRatingHistoryService.PostRecordRatingHistory(recordRatingHistoryDto);
			if (newRrh is null) throw new ApplicationException("Error::RecordRatingHistoryCachedService::PostRecordRatingHistory");

			
			
			

			return null;
		}
	}
}