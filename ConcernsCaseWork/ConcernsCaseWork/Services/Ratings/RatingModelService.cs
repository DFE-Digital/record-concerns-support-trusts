using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Ratings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Ratings
{
	public sealed class RatingModelService : IRatingModelService
	{
		private readonly IRatingService _ratingService;
		private readonly ILogger<RatingModelService> _logger;

		public RatingModelService(IRatingService ratingService, ILogger<RatingModelService> logger)
		{
			_ratingService = ratingService;
			_logger = logger;
		}

		public async Task<IList<RatingDto>> GetRatings()
		{
			_logger.LogInformation("RatingModelService::GetRatings");
			
			var ratingsDto = await _ratingService.GetRatings();

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

		public async Task<IList<RatingModel>> GetSelectedRatingsModelById(long urn)
		{
			_logger.LogInformation("RatingModelService::GetSelectedRatingsModelByUrn");

			var ratings = await GetRatingsModel();
			
			ratings = ratings.Select(r =>
			{
				if (r.Id.CompareTo(urn) == 0)
				{
					r.Checked = true;
				}
				return r;
			}).ToList();

			return ratings;
		}

		public async Task<RatingModel> GetRatingModelById(long urn)
		{
			_logger.LogInformation("RatingModelService::GetRatingModelByUrn");
			
			var ratingsDto = await GetRatings();
			
			var ratingModel = ratingsDto.Select(RatingMapping.MapDtoToModel).FirstOrDefault(r => r.Id == urn);
			
			return ratingModel;
		}
	}
}