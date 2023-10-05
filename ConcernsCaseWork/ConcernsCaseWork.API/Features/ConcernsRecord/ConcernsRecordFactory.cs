using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	public static class ConcernsRecordFactory
	{
		public static Data.Models.ConcernsRecord Create(
			ConcernsRecordRequest concernsRecordRequest,
			ConcernsCase concernsCase,
			Data.Models.ConcernsType concernsType,
			Data.Models.ConcernsRating concernsRating,
			ConcernsMeansOfReferral concernsMeansOfReferral = null)
		{
			return new Data.Models.ConcernsRecord
			{
				CreatedAt = concernsRecordRequest.CreatedAt,
				UpdatedAt = concernsRecordRequest.UpdatedAt,
				ReviewAt = concernsRecordRequest.ReviewAt,
				Name = concernsRecordRequest.Name,
				Description = concernsRecordRequest.Description,
				Reason = concernsRecordRequest.Reason,
				ConcernsCase = concernsCase,
				ConcernsType = concernsType,
				ConcernsRating = concernsRating,
				StatusId = concernsRecordRequest.StatusId,
				ConcernsMeansOfReferral = concernsMeansOfReferral
			};
		}

		public static Data.Models.ConcernsRecord Update(
			Data.Models.ConcernsRecord original,
			ConcernsRecordRequest concernsRecordRequest,
			ConcernsCase concernsCase,
			Data.Models.ConcernsType concernsType,
			Data.Models.ConcernsRating concernsRating,
			ConcernsMeansOfReferral concernsMeansOfReferral = null)
		{
			original.CreatedAt = concernsRecordRequest.CreatedAt;
			original.UpdatedAt = concernsRecordRequest.UpdatedAt;
			original.ReviewAt = concernsRecordRequest.ReviewAt;
			original.ClosedAt = concernsRecordRequest.ClosedAt;
			original.Name = concernsRecordRequest.Name ?? original.Name;
			original.Description = concernsRecordRequest.Description ?? original.Name;
			original.Reason = concernsRecordRequest.Reason ?? original.Name;
			original.ConcernsCase = concernsCase;
			original.ConcernsType = concernsType;
			original.ConcernsRating = concernsRating;
			original.StatusId = concernsRecordRequest.StatusId;
			original.RatingId = concernsRecordRequest.RatingId;
			original.TypeId = concernsRecordRequest.TypeId;

			if (concernsMeansOfReferral != null)
			{
				original.MeansOfReferralId = concernsRecordRequest.MeansOfReferralId;
				original.ConcernsMeansOfReferral = concernsMeansOfReferral;
			}

			return original;
		}
	}
}