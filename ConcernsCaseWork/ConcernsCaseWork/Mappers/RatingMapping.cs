using ConcernsCaseWork.Models;
using Service.TRAMS.Ratings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class RatingMapping
	{
		public const string NotApplicable = "n/a";

		private static readonly Dictionary<string, Tuple<int, IList<string>>> Rags = new Dictionary<string, Tuple<int, IList<string>>>(7)
		{
			{NotApplicable, new Tuple<int, IList<string>>(0, new List<string> { "-" })}, 
			{"Amber-Green", new Tuple<int, IList<string>>(1, new List<string> { "Amber", "Green" })},
			{"Red-Amber", new Tuple<int, IList<string>>(2, new List<string> { "Red", "Amber" })},
			{"Red", new Tuple<int, IList<string>>(3, new List<string> { "Red" })},
			{"Red-Plus", new Tuple<int, IList<string>>(4, new List<string> { "Red Plus" })},
			{"Amber",  new Tuple<int, IList<string>>(5, new List<string> { "Amber" })},
			{"Green",  new Tuple<int, IList<string>>(6, new List<string> { "Green" })}
		};

		private static readonly Dictionary<string, IList<string>> RagsCss = new Dictionary<string, IList<string>>(7)
		{
			{NotApplicable, new List<string> { "" }}, 
			{"Amber-Green", new List<string> { "ragtag__amber", "ragtag__green" }},
			{"Red-Amber", new List<string> { "ragtag__red", "ragtag__amber" }},
			{"Red", new List<string> { "ragtag__red" }},
			{"Red-Plus", new List<string> { "ragtag__redplus" }},
			{"Amber", new List<string> { "ragtag__amber" }},
			{"Green", new List<string> { "ragtag__green" }}
		};
		
		public static Tuple<int, IList<string>> FetchRag(string ratingName)
		{
			var defaultRating = new Tuple<int, IList<string>>(0, new List<string> { NotApplicable });
			return Rags.TryGetValue(ratingName ?? NotApplicable, out var rag) ? rag : defaultRating;
		}
		
		public static IList<string> FetchRagCss(string ratingName)
		{
			var defaultRating = new List<string> { NotApplicable };
			return RagsCss.TryGetValue(ratingName ?? NotApplicable, out var ragCss) ? ragCss : defaultRating;
		}

		public static RatingModel MapDtoToModel(RatingDto ratingDto)
		{
			return new RatingModel
			{
				Name = ratingDto.Name,
				Id = ratingDto.Id,
				RagRating = FetchRag(ratingDto.Name),
				RagRatingCss = FetchRagCss(ratingDto.Name)
			};
		}

		public static List<RatingModel> MapDtoToModelList(IList<RatingDto> ratingsDto)
		{
			return ratingsDto.Select(ratingDto =>
			{
				return new RatingModel
				{
					Name = ratingDto.Name,
					Id = ratingDto.Id,
					RagRating = FetchRag(ratingDto.Name),
					RagRatingCss = FetchRagCss(ratingDto.Name)
				};
			}).ToList();
		}

		public static RatingModel MapDtoToModel(IList<RatingDto> ratingsDto, long id)
		{
			var selectedRatingDto = ratingsDto.FirstOrDefault(t => t.Id.CompareTo(id) == 0) ?? ratingsDto.First();
			return new RatingModel
			{
				Name = selectedRatingDto.Name,
				Id = selectedRatingDto.Id,
				RagRating = FetchRag(selectedRatingDto.Name),
				RagRatingCss = FetchRagCss(selectedRatingDto.Name)
			};
		}
	}
}