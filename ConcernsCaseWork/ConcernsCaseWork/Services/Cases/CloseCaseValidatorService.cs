using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using ConcernsCaseWork.Services.Decisions;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Services.Records;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public class CloseCaseValidatorService : ICloseCaseValidatorService
	{
		private readonly IRecordModelService _recordModelService;
		private readonly ISRMAService _srmaModelService;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly INtiUnderConsiderationModelService _ntiUnderConsiderationModelService;
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiModelService _ntiModelService;
		private readonly IDecisionService _decisionService;
		private readonly ITrustFinancialForecastService _trustFinancialForecastService;
		private readonly ICaseActionValidator _caseActionValidator;

		public CloseCaseValidatorService(
			IRecordModelService recordModelService,
			ISRMAService srmaModelService,
			IFinancialPlanModelService financialPlanModelService,
			INtiUnderConsiderationModelService ntiUnderConsiderationModelService,
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiModelService ntiModelService,
			IDecisionService decisionService,
			ITrustFinancialForecastService trustFinancialForecastService,
			ICaseActionValidator caseActionValidator)
		{
			_recordModelService = recordModelService;
			_srmaModelService = srmaModelService;
			_financialPlanModelService = financialPlanModelService;
			_ntiModelService = ntiModelService;
			_ntiUnderConsiderationModelService = ntiUnderConsiderationModelService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiModelService = ntiModelService;
			_decisionService = decisionService;
			_trustFinancialForecastService = trustFinancialForecastService;
			_caseActionValidator = caseActionValidator;
		}

		public async Task<List<CloseCaseErrorModel>> Validate(long caseUrn)
		{
			var result = new List<CloseCaseErrorModel>();
			List<CaseActionModel> caseActionModels = new List<CaseActionModel>();

			var recordsModels = await _recordModelService.GetRecordsModelByCaseUrn(caseUrn);
			var numberOfOpenConcerns = recordsModels.Count(r => r.StatusId == (int)CaseStatus.Live);

			if (numberOfOpenConcerns > 0)
				result.Add(new CloseCaseErrorModel() { Type = CloseCaseError.Concern, Error = "Resolve Concerns" });

			var srmaModelsTask = _srmaModelService.GetSRMAsForCase(caseUrn);
			var financialPlanModelsTask = _financialPlanModelService.GetFinancialPlansModelByCaseUrn(caseUrn);
			var ntiUnderConsiderationModelsTask = _ntiUnderConsiderationModelService.GetNtiUnderConsiderationsForCase(caseUrn);
			var ntiWarningLetterModelsTask = _ntiWarningLetterModelService.GetNtiWarningLettersForCase(caseUrn);
			var ntiModelModelsTask = _ntiModelService.GetNtisForCaseAsync(caseUrn);
			var decisionsTask = GetDecisions(caseUrn);
			var trustFinancialForecastTask = _trustFinancialForecastService.GetAllForCase((int)caseUrn);

			caseActionModels.AddRange(await srmaModelsTask);
			caseActionModels.AddRange(await financialPlanModelsTask);
			caseActionModels.AddRange(await ntiUnderConsiderationModelsTask);
			caseActionModels.AddRange(await ntiWarningLetterModelsTask);
			caseActionModels.AddRange(await ntiModelModelsTask);
			caseActionModels.AddRange(await decisionsTask);
			caseActionModels.AddRange((await trustFinancialForecastTask).Select(x => x.ToTrustFinancialForecastSummaryModel()));

			var caseActionErrorMessages = _caseActionValidator.Validate(caseActionModels);

			var caseActionErrors = caseActionErrorMessages.Select(error => new CloseCaseErrorModel() { Type = CloseCaseError.CaseAction, Error = error });

			result.AddRange(caseActionErrors);

			return result;
		}

		private async Task<List<DecisionSummaryModel>> GetDecisions(long caseUrn)
		{
			var apiDecisions = await _decisionService.GetDecisionsByCaseUrn(caseUrn);

			var result = apiDecisions.Select(d => DecisionMapping.ToDecisionSummaryModel(d)).ToList();

			return result;
		}
	}

	public interface ICloseCaseValidatorService
	{
		public Task<List<CloseCaseErrorModel>> Validate(long caseUrn);
	}
}
