using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Ratings;
using Service.TRAMS.Ratings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Ratings
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

		public async Task<IList<RatingDto>> GetRatings()
		{
			_logger.LogInformation("RatingModelService::GetRatings");
			
			var ratingsDto = await _ratingCachedService.GetRatings();

			return ratingsDto;
		}
		
		public async Task<IList<RatingModel>> GetRatingsModel()
		{
			_logger.LogInformation("RatingModelService::GetRatingsModel");

			var ratingsDto = await GetRatings();
			
			// Filter n/a rating
			ratingsDto = ratingsDto.Where(r => !r.Name.Equals(RatingMapping.NotApplicable, StringComparison.OrdinalIgnoreCase)).ToList();

			// Map dto to model, Order by Rating Mapping Order
			var ratingsModel= ratingsDto.Select(RatingMapping.MapDtoToModel).OrderBy(r => r.RagRating.Item1).ToList();

			return ratingsModel;
		}

		public async Task<IList<RatingModel>> GetSelectedRatingsModelByUrn(long urn)
		{
			_logger.LogInformation("RatingModelService::GetSelectedRatingsModelByUrn");

			var ratings = await GetRatingsModel();
			
			ratings = ratings.Select(r =>
			{
				if (r.Urn.CompareTo(urn) == 0)
				{
					r.Checked = true;
				}
				return r;
			}).ToList();

			return ratings;
		}

		public async Task<RatingModel> GetRatingModelByUrn(long urn)
		{
			_logger.LogInformation("RatingModelService::GetRatingModelByUrn");
			
			var ratingDtos = await GetRatings();

			var rating = ratingDtos.First(x => x.Urn == urn);
			return RatingMapping.MapDtoToModel(rating);
		}
	}
}