using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NTIWarningLetter
{
	public class GetAllConditionTypes : IUseCase<object, List<NTIWarningLetterConditionType>>
	{
		private readonly INTIWarningLetterGateway _gateway;

		public GetAllConditionTypes(INTIWarningLetterGateway gateway)
		{
			_gateway = gateway;
		}

		public List<NTIWarningLetterConditionType> Execute(object ignore)
		{
			return ExecuteAsync(ignore).Result;
		}

		public async Task<List<NTIWarningLetterConditionType>> ExecuteAsync(object ignore)
		{
			return await _gateway.GetAllConditionTypes();
		}
	}
}
