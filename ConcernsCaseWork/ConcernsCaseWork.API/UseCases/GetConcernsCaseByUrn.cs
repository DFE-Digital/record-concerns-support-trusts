using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;

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
           var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(urn);
           return concernsCase == null ? null : ConcernsCaseResponseFactory.Create(concernsCase);
        }
    }
}