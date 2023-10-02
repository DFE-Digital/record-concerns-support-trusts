using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NTIWarningLetter
{
	public class GetAllConditions : IUseCase<object, List<NTIWarningLetterCondition>>
	{
		private readonly INTIWarningLetterGateway _gateway;

		public GetAllConditions(INTIWarningLetterGateway gateway)
		{
			_gateway = gateway;
		}

		public List<NTIWarningLetterCondition> Execute(object ignore)
		{
			return ExecuteAsync(ignore).Result;
		}

		public async Task<List<NTIWarningLetterCondition>> ExecuteAsync(object ignore)
		{
			return await _gateway.GetAllConditions();
		}
	}
}
