using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;

namespace ConcernsCaseWork.Data.Tests.TestData;

public class TestDataFactory
{
	private readonly RandomGenerator _randomGenerator = new ();
	
	public ConcernsCase BuildOpenCase(int statusId, int ratingId)
		=> new ConcernsCase
		{
			CreatedAt = _randomGenerator.DateTime(),
			UpdatedAt = _randomGenerator.DateTime(),
			ClosedAt = DateTime.MinValue,
			CreatedBy = _randomGenerator.NextString(3, 10),
			DirectionOfTravel = _randomGenerator.NextString(3, 10),
			StatusId = statusId,
			RatingId = ratingId
		};
	
	public ConcernsRecord BuildOpenConcern(int caseId, int statusId, int ratingId, int typeId)
		=> new ConcernsRecord
		{
			CaseId = caseId,
			CreatedAt = _randomGenerator.DateTime(),
			RatingId = ratingId,
			StatusId = statusId,
			TypeId =  typeId,
			UpdatedAt = _randomGenerator.DateTime()
		};

	public FinancialPlanCase BuildOpenFinancialPlan(int caseId, int planStatusId)
		=> new FinancialPlanCase
		{
			CaseUrn = caseId,
			StatusId = planStatusId,
			CreatedAt = _randomGenerator.DateTime(),
			UpdatedAt = _randomGenerator.DateTime(),
			ClosedAt = null
		};
	
	public SRMACase BuildOpenSrmaCase(int caseId, int srmaStatusId)
		=> new SRMACase
		{
			CaseUrn = caseId,
			StatusId = srmaStatusId,
			DateOffered = _randomGenerator.DateTime(),
			CreatedAt = _randomGenerator.DateTime(),
			UpdatedAt = _randomGenerator.DateTime(),
			ClosedAt = null,
			CloseStatusId = null
		};
	
	public NoticeToImprove BuildOpenNoticeToImprove(int caseId, int noticeToImproveStatusId)
		=> new NoticeToImprove
		{
			CaseUrn = caseId,
			StatusId = noticeToImproveStatusId,
			CreatedAt = _randomGenerator.DateTime(),
			UpdatedAt = _randomGenerator.DateTime(),
			ClosedAt = null
		};
		
	public NTIUnderConsideration BuildOpenNtiUnderConsideration(int caseId)
		=> new NTIUnderConsideration
		{
			CaseUrn = caseId,
			CreatedAt = _randomGenerator.DateTime(),
			UpdatedAt = _randomGenerator.DateTime(),
			ClosedStatusId = null,
			ClosedAt = null,
		};
			
	public NTIWarningLetter BuildOpenNtiWarningLetter(int caseId, int ntiWarningLetterStatusId)
		=> new NTIWarningLetter
		{
			CaseUrn = caseId,
			StatusId = ntiWarningLetterStatusId,
			ClosedStatusId = null,
			ClosedAt = null,
			CreatedAt = _randomGenerator.DateTime(),
			UpdatedAt = _randomGenerator.DateTime()
		};
}