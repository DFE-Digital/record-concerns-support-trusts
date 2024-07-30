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
	public class CaseValidatorService : ICaseValidatorService
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

		public CaseValidatorService(
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

		public async Task<List<CaseValidationErrorModel>> ValidateClose(long caseUrn)
		{
			var result = new List<CaseValidationErrorModel>();
			var (recordModels, caseActionModels) = await GetAllModels(caseUrn);

			var numberOfOpenConcerns = recordModels.Count(r => r.StatusId == (int)CaseStatus.Live);
			if (numberOfOpenConcerns > 0)
				result.Add(new CaseValidationErrorModel() { Type = CaseValidationError.Concern, Error = "Close concerns" });

			var caseActionErrorMessages = _caseActionValidator.Validate(caseActionModels);

			var caseActionErrors = caseActionErrorMessages.Select(error => new CaseValidationErrorModel() { Type = CaseValidationError.CaseAction, Error = error });

			result.AddRange(caseActionErrors);

			return result;
		}

		public async Task<List<CaseValidationErrorModel>> ValidateDelete(long caseUrn)
		{
			var result = new List<CaseValidationErrorModel>();
			var (recordModels, caseActionModels) = await GetAllModels(caseUrn);

			var numberOfConcerns = recordModels.Count;
			if (numberOfConcerns > 0)
				result.Add(new CaseValidationErrorModel() { Type = CaseValidationError.Concern, Error = "Delete concerns" });

			var caseActionErrorMessages = _caseActionValidator.ValidateDelete(caseActionModels);

			var caseActionErrors = caseActionErrorMessages.Select(error => new CaseValidationErrorModel() { Type = CaseValidationError.CaseAction, Error = error });

			result.AddRange(caseActionErrors);

			return result;
		}

		private async Task<List<DecisionSummaryModel>> GetDecisions(long caseUrn)
		{
			var apiDecisions = await _decisionService.GetDecisionsByCaseUrn(caseUrn);

			var result = apiDecisions.Select(d => DecisionMapping.ToDecisionSummaryModel(d)).ToList();

			return result;
		}

		private async Task<(IList<RecordModel>, IList<CaseActionModel>)> GetAllModels(long caseUrn)
		{
			var result = new List<CaseValidationErrorModel>();
			List<CaseActionModel> caseActionModels = new();

			var recordsModels = await _recordModelService.GetRecordsModelByCaseUrn(caseUrn);

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

			return (recordsModels, caseActionModels);
		} 
	}


	public interface ICaseValidatorService
	{
		public Task<List<CaseValidationErrorModel>> ValidateClose(long caseUrn);
		public Task<List<CaseValidationErrorModel>> ValidateDelete(long caseUrn);
	}
}
