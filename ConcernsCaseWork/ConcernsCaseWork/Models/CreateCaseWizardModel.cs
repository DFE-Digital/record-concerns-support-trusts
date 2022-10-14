using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models;
[Serializable]
public class CreateCaseWizardModel
{
	public int CaseType { get;  set; }

	public BuildCaseModel Case { get; set; }

	public void SetTrustUkPrn(string trustUkPrn)
	{
		//Guard.Against.NullOrEmpty(trustUkPrn);
		
		Case.SetTrustUkPrn(trustUkPrn);
	}
	
	public void SetCaseType(int caseType)
	{
		if (caseType == 2)
		{
			throw new NotImplementedException();
		}

		var existingTrustUkPrn = Case?.TrustUkPrn;
		
		CaseType = caseType;
		Case = caseType switch
		{
			0 => null,
			1 => new BuildConcernsCaseModel(),
			2 => new BuildNonConcernsCaseModel(),
			_ => null
		};

		Case?.SetTrustUkPrn(existingTrustUkPrn);
	}

	public static CreateCaseWizardModel Empty()
	{
		var model = new CreateCaseWizardModel { Case = new BuildCaseModel() };
		return model;
	}

	private void Initialise()
	{
		SetTrustUkPrn(string.Empty);
		SetCaseType(0);
	}
		
	public bool IsComplete() => !string.IsNullOrEmpty(Case?.TrustUkPrn) && CaseType > 0;
}

[Serializable]
public class BuildCaseModel
{
	public string TrustUkPrn { get; set; }
	
	public void SetTrustUkPrn(string trustUkPrn)
	{
		//Guard.Against.NullOrEmpty(trustUkPrn);
		
		TrustUkPrn = trustUkPrn;
	}
}
[Serializable]
public class BuildConcernsCaseModel : BuildCaseModel
{
	
	public DateTimeOffset DeEscalation { get;  set; }

	public string Issue { get;  set; }

	public string CurrentStatus { get;  set; }

	public string CaseAim { get;  set; }

	public string NextSteps { get;  set; }
		
	public string DirectionOfTravel { get;  set; }
		
	public int StatusId { get;  set; }
		
	public int RatingId { get;  set; }

	public List<ConcernsCaseRecord> Concerns { get; set; } = new();
	
	public void SetStatusId(int statusId)
	{
		Guard.Against.Negative(statusId);
		
		StatusId = statusId;
	}
	
	public void SetRatingId(int ratingId)
	{
		Guard.Against.Negative(ratingId);
		
		RatingId = ratingId;
	}

	public void SetDirectionOfTravel(string directionOfTravel)
	{
		Guard.Against.NullOrWhiteSpace(directionOfTravel);
		
		DirectionOfTravel = directionOfTravel;
	}

	public void AddConcern(int ragRatingId, string type, string subType, int meansOfReferralId)
	{
		Concerns.Add(new ConcernsCaseRecord()
		{
			RatingId = ragRatingId,
			Type = type,
			SubType = subType,
			MeansOfReferralId = meansOfReferralId
		});
	}
}

public class BuildNonConcernsCaseModel : BuildCaseModel
{
	
}

public class ConcernsCaseRecord
{
	public string Type { get;  set; }
	
	public string SubType { get;  set; }
	
	public int StatusId { get;  set; }
		
	public int RatingId { get;  set; }
	
	public int MeansOfReferralId { get; set; }
	
	public void SetType(string type, string subType)
	{
		Guard.Against.NullOrWhiteSpace(type);
		Guard.Against.NullOrWhiteSpace(subType);
		
		Type = type;
		SubType = subType;
	}
}