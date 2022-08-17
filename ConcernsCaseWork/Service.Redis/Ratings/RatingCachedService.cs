using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using ConcernsCasework.Service.Ratings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.Ratings
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

		public async Task ClearData()
		{
			await ClearData(RatingsKey);
		}

		public async Task<IList<RatingDto>> GetRatings()
		{
			_logger.LogInformation("RatingCachedService::GetRatings");
			
			// Check cache
			var ratings = await GetData<IList<RatingDto>>(RatingsKey);
			if (ratings != null) return ratings;

			// Fetch from Academies API
			ratings = await _ratingService.GetRatings();

			// Store in cache for 24 hours (default)
			await StoreData(RatingsKey, ratings);
			
			return ratings;
		}

		public async Task<RatingDto> GetDefaultRating()
		{
			var ratingsDto = await GetRatings();
			var ratingDto = ratingsDto.FirstOrDefault(r => r.Name.Equals("n/a"));

			return ratingDto;
		}
	}
}