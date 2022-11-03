using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases
{
    public class GetConcernsCasesByOwnerId : IGetConcernsCasesByOwnerId
    {
        private readonly IConcernsCaseGateway _concernsCaseGateway;

        public GetConcernsCasesByOwnerId(IConcernsCaseGateway concernsCaseGateway)
        {
            _concernsCaseGateway = concernsCaseGateway;
        }

        public IList<ConcernsCaseResponse> Execute(string ownerId, int? statusId, int page, int count)
        {
            var cases = _concernsCaseGateway.GetConcernsCasesByOwnerId(ownerId, statusId, page, count);
            return cases.Select(ConcernsCaseResponseFactory.Create).ToList();
        }
    }
}