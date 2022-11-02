using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases
{
    public class GetConcernsCaseByTrustUkprn : IGetConcernsCaseByTrustUkprn
    {
        private readonly IConcernsCaseGateway _concernsCaseGateway;

        public GetConcernsCaseByTrustUkprn(IConcernsCaseGateway concernsCaseGateway)
        {
            _concernsCaseGateway = concernsCaseGateway;
        }
        public IList<ConcernsCaseResponse> Execute(string trustUkprn, int page, int count)
        {
            var concernsCases = _concernsCaseGateway.GetConcernsCaseByTrustUkprn(trustUkprn, page, count);
            return concernsCases.Select(ConcernsCaseResponseFactory.Create).ToList();
        }
    }
}