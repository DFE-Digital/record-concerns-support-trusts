using Concerns.Data.Models;
using Concerns.Data.RequestModels.CaseActions.FinancialPlan;
using Concerns.Data.ResponseModels.CaseActions.FinancialPlan;

namespace Concerns.Data.Factories.CaseActionFactories
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
                DateViablePlanReceived =  createFinancialPlanRequest.DateViablePlanReceived,
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
                DateViablePlanReceived =  patchFinancialPlanRequest.DateViablePlanReceived,
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
                DateViablePlanReceived= model.DateViablePlanReceived,
                Notes = model.Notes,
                Status = model.Status,
                UpdatedAt = model.UpdatedAt,
                StatusId=model.StatusId,
            };
        }


    }
}
