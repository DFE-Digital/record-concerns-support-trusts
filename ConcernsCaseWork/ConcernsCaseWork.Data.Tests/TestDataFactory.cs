using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;

namespace ConcernsCaseWork.Data.Tests;

public class TestDataFactory
{
	private readonly RandomGenerator _randomGenerator = new ();
	
	public ConcernsCase BuildOpenCase(int statusId, int ratingId)
		=> new ConcernsCase
		{
			CreatedAt = _randomGenerator.DateTime(),
			UpdatedAt = _randomGenerator.DateTime(),
			ClosedAt = _randomGenerator.DateTime(),
			CreatedBy = _randomGenerator.NextString(3, 10),
			TrustUkprn = _randomGenerator.NextString(3, 10),
			Issue = _randomGenerator.NextString(3, 200),
			DirectionOfTravel = _randomGenerator.NextString(3, 10),
			StatusId = statusId,
			RatingId = ratingId
		};
	
	public ConcernsRecord BuildOpenConcern(int caseId, int statusId, int ratingId, int typeId)
		=> new ConcernsRecord
		{
			CaseId = caseId,
			CreatedAt = _randomGenerator.DateTime(),
			Description = _randomGenerator.NextString(3, 10),
			MeansOfReferralId = 1,
			Name = _randomGenerator.NextString(3, 10),
			RatingId = ratingId,
			Reason = _randomGenerator.NextString(3, 10),
			StatusId = statusId,
			TypeId =  typeId,
			UpdatedAt = _randomGenerator.DateTime()
		};
}