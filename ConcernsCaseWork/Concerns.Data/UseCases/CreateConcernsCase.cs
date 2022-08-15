using Concerns.Data.Factories;
using Concerns.Data.Gateways;
using Concerns.Data.RequestModels;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public class CreateConcernsCase : ICreateConcernsCase
    {
        private readonly IConcernsCaseGateway _concernsCaseGateway;

        public CreateConcernsCase(IConcernsCaseGateway concernsCaseGateway)
        {
            _concernsCaseGateway = concernsCaseGateway;
        }

        public ConcernsCaseResponse Execute(ConcernCaseRequest request)
        {
            var concernsCaseToCreate = ConcernsCaseFactory.Create(request);
            var createdConcernsCase = _concernsCaseGateway.SaveConcernsCase(concernsCaseToCreate);
            return ConcernsCaseResponseFactory.Create(createdConcernsCase);
        }
    }
}