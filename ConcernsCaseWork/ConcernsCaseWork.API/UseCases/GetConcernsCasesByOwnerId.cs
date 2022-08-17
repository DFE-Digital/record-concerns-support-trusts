using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public class GetConcernsCasesByOwnerId : IGetConcernsCasesByOwnerId
    {
        private readonly IConcernsCaseGateway _concernsCaseGateway;

        public GetConcernsCasesByOwnerId(IConcernsCaseGateway concernsCaseGateway)
        {
            _concernsCaseGateway = concernsCaseGateway;
        }

        public IList<ConcernsCaseResponse> Execute(string ownerId, int? statusUrn, int page, int count)
        {
            var cases = _concernsCaseGateway.GetConcernsCasesByOwnerId(ownerId, statusUrn, page, count);
            return cases.Select(ConcernsCaseResponseFactory.Create).ToList();
        }
    }
}