using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases
{
    public class GetConcernsCaseByUrn : IGetConcernsCaseByUrn
    {
        private readonly IConcernsCaseGateway _concernsCaseGateway;

        public GetConcernsCaseByUrn(IConcernsCaseGateway concernsCaseGateway)
        {
            _concernsCaseGateway = concernsCaseGateway;
        }

        public ConcernsCaseResponse Execute(int urn)
        {
           var concernsCase = _concernsCaseGateway.GetConcernsCaseById(urn);
           return concernsCase == null ? null : ConcernsCaseResponseFactory.Create(concernsCase);
        }
    }
}