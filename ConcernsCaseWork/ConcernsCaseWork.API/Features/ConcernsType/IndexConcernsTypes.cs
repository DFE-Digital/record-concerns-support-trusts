using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.ConcernsType
{
	public interface IIndexConcernsTypes
	{
		public IList<ConcernsTypeResponse> Execute();
	}

	public class IndexConcernsTypes : IIndexConcernsTypes
	{
		private readonly IConcernsTypeGateway _concernsTypeGateway;

		public IndexConcernsTypes(IConcernsTypeGateway concernsTypeGateway)
		{
			_concernsTypeGateway = concernsTypeGateway;
		}
		public IList<ConcernsTypeResponse> Execute()
		{
			var types = _concernsTypeGateway.GetTypes();
			return types.Select(ConcernsTypeResponseFactory.Create).ToList();
		}
	}
}