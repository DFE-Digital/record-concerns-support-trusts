using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;

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
            var concernsType = _concernsTypeGateway.GetConcernsTypeByUrn(request.TypeUrn);
            var concernsRatings = _concernsRatingGateway.GetRatingByUrn(request.RatingUrn);
            var concernsMeansOfReferral = request.MeansOfReferralUrn != null ? _concernsMeansOfReferralGateway.GetMeansOfReferralByUrn((int)request.MeansOfReferralUrn) : null;
            var concernsRecordToCreate = ConcernsRecordFactory.Create(request, concernsCase, concernsType, concernsRatings, concernsMeansOfReferral);
            var savedConcernsRecord = _concernsRecordGateway.SaveConcernsCase(concernsRecordToCreate);
            return ConcernsRecordResponseFactory.Create(savedConcernsRecord);
        }
    }
}