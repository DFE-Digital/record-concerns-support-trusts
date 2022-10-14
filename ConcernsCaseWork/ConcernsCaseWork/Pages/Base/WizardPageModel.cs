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

	private bool IsLastStep() => CurrentStep >= LastStep;
	private void ResetSteps() => CurrentStep = 0;
}