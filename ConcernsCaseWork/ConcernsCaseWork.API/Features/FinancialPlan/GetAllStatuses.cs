using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.FinancialPlan
{
	public class GetAllStatuses : IUseCase<object, List<FinancialPlanStatus>>
	{
		private readonly IFinancialPlanGateway _gateway;

		public GetAllStatuses(IFinancialPlanGateway gateway)
		{
			_gateway = gateway;
		}

		public List<FinancialPlanStatus> Execute(object ignore)
		{
			return ExecuteAsync(ignore).Result;
		}

		public async Task<List<FinancialPlanStatus>> ExecuteAsync(object ignore)
		{
			return await _gateway.GetAllStatuses();
		}
	}
}
