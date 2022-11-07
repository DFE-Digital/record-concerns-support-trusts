using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases
{
    public class CreateConcernsRecord : ICreateConcernsRecord
    {
        private readonly IConcernsRecordGateway _concernsRecordGateway;
        private readonly IConcernsCaseGateway _concernsCaseGateway;
        private readonly IConcernsTypeGateway _concernsTypeGateway;
        private readonly IConcernsRatingGateway _concernsRatingGateway;
        private readonly IConcernsMeansOfReferralGateway _concernsMeansOfReferralGateway;

        public CreateConcernsRecord(
            IConcernsRecordGateway concernsRecordGateway, 
            IConcernsCaseGateway concernsCaseGateway,
            IConcernsTypeGateway concernsTypeGateway,
            IConcernsRatingGateway concernsRatingGateway,
            IConcernsMeansOfReferralGateway concernsMeansOfReferralGateway)
        {
            _concernsRecordGateway = concernsRecordGateway;
            _concernsCaseGateway = concernsCaseGateway;
            _concernsTypeGateway = concernsTypeGateway;
            _concernsRatingGateway = concernsRatingGateway;
            _concernsMeansOfReferralGateway = concernsMeansOfReferralGateway;
        }
        public ConcernsRecordResponse Execute(ConcernsRecordRequest request)
        {
            var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(request.CaseUrn);
            var concernsType = _concernsTypeGateway.GetConcernsTypeById(request.TypeId);
            var concernsRatings = _concernsRatingGateway.GetRatingById(request.RatingId);
            var concernsMeansOfReferral = request.MeansOfReferralId != null ? _concernsMeansOfReferralGateway.GetMeansOfReferralById((int)request.MeansOfReferralId) : null;
            var concernsRecordToCreate = ConcernsRecordFactory.Create(request, concernsCase, concernsType, concernsRatings, concernsMeansOfReferral);
            var savedConcernsRecord = _concernsRecordGateway.SaveConcernsCase(concernsRecordToCreate);
            return ConcernsRecordResponseFactory.Create(savedConcernsRecord);
        }
    }
}