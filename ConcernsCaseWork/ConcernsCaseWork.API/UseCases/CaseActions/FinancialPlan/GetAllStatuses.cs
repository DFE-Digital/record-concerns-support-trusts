using Concerns.Data.Gateways;
using Concerns.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.UseCases.CaseActions.FinancialPlan
{
    public class GetAllStatuses : IUseCase<Object, List<FinancialPlanStatus>>
    {
        private readonly IFinancialPlanGateway _gateway;

        public GetAllStatuses(IFinancialPlanGateway gateway)
        {
            _gateway = gateway;
        }

        public List<FinancialPlanStatus> Execute(Object ignore)
        {
            return ExecuteAsync(ignore).Result;
        }

        public async Task<List<FinancialPlanStatus>> ExecuteAsync(Object ignore)
        {
            return await _gateway.GetAllStatuses();
        }
    }
}
