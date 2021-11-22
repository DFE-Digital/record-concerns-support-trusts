using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Rating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Rating
{
	public sealed class RatingModelService : IRatingModelService
	{
		private readonly IRatingCachedService _ratingCachedService;
		private readonly ILogger<RatingModelService> _logger;

		public RatingModelService(IRatingCachedService ratingCachedService, ILogger<RatingModelService> logger)
		{
			_ratingCachedService = ratingCachedService;
			_logger = logger;
		}

		public async Task<IList<RatingModel>> GetRatings()
		{
			_logger.LogInformation("RatingModelService::GetRatings");

			var ratingsDto = await _ratingCachedService.GetRatings();
			
			// Filter n/a rating
			ratingsDto = ratingsDto.Where(r => !r.Name.Equals(RatingMapping.NotApplicable, StringComparison.OrdinalIgnoreCase)).ToList();

			// Map dto to model
			var ratingsModel= ratingsDto.OrderBy(r => r.Name).Select(RatingMapping.MapDtoToModel).ToList();

			return ratingsModel;
		}

		public async Task<IList<RatingModel>> GetSelectedRatingsByUrn(long urn)
		{
			_logger.LogInformation("RatingModelService::GetSelectedRatingsByUrn");

			var ratings = await GetRatings();
			ratings.FirstOrDefault(r => r.Urn == urn).Checked = true;

			return ratings;
		}

		public async Task<RatingModel> GetRatingByUrn(long urn)
		{
			_logger.LogInformation("RatingModelService::GetRatingByUrn");
			var ratings = await GetRatings();

			return ratings.FirstOrDefault(r => r.Urn == urn);
		}

	}
}