using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases
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