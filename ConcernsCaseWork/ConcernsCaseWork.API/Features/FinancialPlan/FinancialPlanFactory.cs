using ConcernsCaseWork.API.Contracts.FinancialPlan;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.FinancialPlan
{
	public static class FinancialPlanFactory
	{
		public static FinancialPlanCase CreateDBModel(CreateFinancialPlanRequest createFinancialPlanRequest)
		{
			return new FinancialPlanCase
			{
				CaseUrn = createFinancialPlanRequest.CaseUrn,
				Name = createFinancialPlanRequest.Name,
				ClosedAt = createFinancialPlanRequest.ClosedAt,
				CreatedAt = createFinancialPlanRequest.CreatedAt,
				CreatedBy = createFinancialPlanRequest.CreatedBy,
				DatePlanRequested = createFinancialPlanRequest.DatePlanRequested,
				DateViablePlanReceived = createFinancialPlanRequest.DateViablePlanReceived,
				Notes = createFinancialPlanRequest.Notes,
				StatusId = createFinancialPlanRequest.StatusId,
				UpdatedAt = createFinancialPlanRequest.UpdatedAt,
			};
		}

		public static FinancialPlanCase CreateDBModel(PatchFinancialPlanRequest patchFinancialPlanRequest)
		{
			return new FinancialPlanCase
			{
				Id = patchFinancialPlanRequest.Id,
				CaseUrn = patchFinancialPlanRequest.CaseUrn,
				Name = patchFinancialPlanRequest.Name,
				ClosedAt = patchFinancialPlanRequest.ClosedAt,
				CreatedAt = patchFinancialPlanRequest.CreatedAt,
				CreatedBy = patchFinancialPlanRequest.CreatedBy,
				DatePlanRequested = patchFinancialPlanRequest.DatePlanRequested,
				DateViablePlanReceived = patchFinancialPlanRequest.DateViablePlanReceived,
				Notes = patchFinancialPlanRequest.Notes,
				StatusId = patchFinancialPlanRequest.StatusId,
				UpdatedAt = patchFinancialPlanRequest.UpdatedAt,
			};
		}

		public static FinancialPlanResponse CreateResponse(FinancialPlanCase model)
		{
			return new FinancialPlanResponse
			{
				Id = model.Id,
				CaseUrn = model.CaseUrn,
				Name = model.Name,
				CreatedAt = model.CreatedAt,
				CreatedBy = model.CreatedBy,
				ClosedAt = model.ClosedAt,
				DatePlanRequested = model.DatePlanRequested,
				DateViablePlanReceived = model.DateViablePlanReceived,
				Notes = model.Notes,
				Status = CreateStatusResponse(model.Status),
				UpdatedAt = model.UpdatedAt,
				StatusId = model.StatusId,
				DeletedAt = model.DeletedAt
			};
		}

		public static FinancialPlanStatusResponse CreateStatusResponse(FinancialPlanStatus model)
		{
			if (model == null)
				return null;

			return new FinancialPlanStatusResponse
			{
				Id = model.Id,
				Name = model.Name,
				Description = model.Description,
				CreatedAt = model.CreatedAt,
				UpdatedAt = model.UpdatedAt,
				IsClosedStatus = model.IsClosedStatus
			};
		}
	}
}
