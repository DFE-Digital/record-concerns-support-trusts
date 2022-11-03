namespace ConcernsCaseWork.API.RequestModels.Concerns.Decisions
{
    public class GetDecisionsRequest
    {
        public int ConcernsCaseUrn { get; set; }
        public GetDecisionsRequest(int concernsCaseUrn)
        {
            ConcernsCaseUrn = concernsCaseUrn <= 0 ? throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn)) : concernsCaseUrn;
        }
    }
}
