using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.Rating;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Rating
{
	public sealed class RatingCachedService : CachedService, IRatingCachedService
	{
		private readonly ILogger<RatingCachedService> _logger;
		private readonly IRatingService _ratingService;

		private const string RatingsKey = "Concerns.Ratings";
		
		public RatingCachedService(ICacheProvider cacheProvider, IRatingService ratingService, ILogger<RatingCachedService> logger) 
			: base(cacheProvider)
		{
			_ratingService = ratingService;
			_logger = logger;
		}

		public async Task<IList<RatingDto>> GetRatings()
		{
			_logger.LogInformation("RatingCachedService::GetRatings");
			
			// Check cache
			var ratings = await GetData<IList<RatingDto>>(RatingsKey);
			if (ratings != null) return ratings;

			// Fetch from TRAMS API
			ratings = await _ratingService.GetRatings();

			// Store in cache for 24 hours (default)
			await StoreData(RatingsKey, ratings);
			
			return ratings;
		}
	}
}