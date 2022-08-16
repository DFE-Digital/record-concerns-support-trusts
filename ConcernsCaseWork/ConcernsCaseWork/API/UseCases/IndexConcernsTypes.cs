using Concerns.Data.Factories;
using Concerns.Data.Gateways;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
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