using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public class SearchEstablishments : IUseCase<SearchEstablishmentsRequest, IList<EstablishmentSummaryResponse>>
    {
        private readonly IEstablishmentGateway _establishmentGateway;

        public SearchEstablishments(IEstablishmentGateway establishmentGateway)
        {
            _establishmentGateway = establishmentGateway;
        }

        public IList<EstablishmentSummaryResponse> Execute(SearchEstablishmentsRequest request)
        {
            return _establishmentGateway.SearchEstablishments(request?.Urn, request?.Ukprn, request?.Name)
                .Select(e => EstablishmentSummaryResponseFactory.Create(e))
                .ToList();
        }
    }
}