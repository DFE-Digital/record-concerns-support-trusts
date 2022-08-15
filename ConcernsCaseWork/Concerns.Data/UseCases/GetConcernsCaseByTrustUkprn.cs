using Concerns.Data.Factories;
using Concerns.Data.Gateways;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
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