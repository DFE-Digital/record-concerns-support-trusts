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

		public async Task<IList<RatingModel>> GetRatingsModel()
		{
			_logger.LogInformation("RatingModelService::GetRatings");

			var ratingsDto = await _ratingCachedService.GetRatings();
			
			// Filter n/a rating
			ratingsDto = ratingsDto.Where(r => !r.Name.Equals(RatingMapping.NotApplicable, StringComparison.OrdinalIgnoreCase)).ToList();

			// Map dto to model
			var ratingsModel= ratingsDto.OrderBy(r => r.Name).Select(RatingMapping.MapDtoToModel).ToList();

			return ratingsModel;
		}

		public async Task<IList<RatingModel>> GetSelectedRatingsModelByUrn(long urn)
		{
			_logger.LogInformation("RatingModelService::GetSelectedRatingsByUrn");

			var ratings = await GetRatingsModel();
			ratings = ratings.Where(r => r.Urn.CompareTo(urn) == 0).Select(r =>
			{
				r.Checked = true;
				return r;
			}).ToList();

			return ratings;
		}

		public async Task<RatingModel> GetRatingModelByUrn(long urn)
		{
			_logger.LogInformation("RatingModelService::GetRatingByUrn");
			var ratings = await GetRatingsModel();

			return ratings.FirstOrDefault(r => r.Urn == urn);
		}
	}
}