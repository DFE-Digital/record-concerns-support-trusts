using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.Pages.Base;

public abstract class WizardPageModel : AbstractPageModel
{
	[TempData]
	public int CurrentStep { get; set; }
	
	public abstract int LastStep { get; set; }
	
	protected void NextStep()
	{
		if (IsLastStep())
		{
			ResetSteps();
		}
		else
		{
			CurrentStep++;
		}
	}

	protected void ResetStepsIfLastStepReached()
	{
		if (IsLastStep())
		{
			ResetSteps();
		}
	}

	protected void ResetSteps() => CurrentStep = 0;
	
	private bool IsLastStep() => CurrentStep >= LastStep;
}