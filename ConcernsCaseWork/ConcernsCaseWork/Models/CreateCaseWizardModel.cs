using System;

namespace ConcernsCaseWork.Models;

public class CreateCaseWizardModel
{
	public string TrustUkPrn { get; private set; }
	public int CaseType { get; private set; }

	private bool HasTrustUkPrn() => !string.IsNullOrEmpty(TrustUkPrn);
	private bool HasCaseType() => CaseType > 0;

	public void SetTrustUkPrn(string trustUkPrn)
	{
		TrustUkPrn = trustUkPrn;
	}
	
	public void SetCaseType(int caseType)
	{
		if (caseType == 2)
		{
			throw new NotImplementedException();
		}
		
		CaseType = caseType;
	}

	public static CreateCaseWizardModel CreateInstance()
	{
		var model = new CreateCaseWizardModel();
		model.Initialise();
		return model;
	}

	public void Initialise()
	{
		SetTrustUkPrn(null);
		SetCaseType(0);
	}
		
	public bool IsInitialised() => !HasTrustUkPrn() && !HasCaseType();
		
	public bool IsComplete() => HasTrustUkPrn() && HasCaseType();
		
	public bool IsInProgress() => (HasTrustUkPrn() || HasCaseType()) && !IsComplete();
}