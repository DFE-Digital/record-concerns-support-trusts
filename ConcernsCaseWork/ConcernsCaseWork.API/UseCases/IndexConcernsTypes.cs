using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases
{
    public class IndexConcernsTypes : IIndexConcernsTypes
    {
        private IConcernsTypeGateway _concernsTypeGateway;

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