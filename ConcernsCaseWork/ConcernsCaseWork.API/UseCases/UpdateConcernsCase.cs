using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases
{
    public class UpdateConcernsCase : IUpdateConcernsCase
    {
        private readonly IConcernsCaseGateway _concernsCaseGateway;
        
        public UpdateConcernsCase(
            IConcernsCaseGateway concernsCaseGateway)
        {
            _concernsCaseGateway = concernsCaseGateway;
        }
        
        public ConcernsCaseResponse Execute(int urn, ConcernCaseRequest request)
        {
            var currentConcernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(urn);
            var concernsCaseToUpdate = ConcernsCaseFactory.Update(currentConcernsCase, request);
            var updatedConcernsCase = _concernsCaseGateway.Update(concernsCaseToUpdate);
            return ConcernsCaseResponseFactory.Create(updatedConcernsCase);
        }
    }
}