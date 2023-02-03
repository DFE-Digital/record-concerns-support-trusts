using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Data.Exceptions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using System.Text.Json;

namespace ConcernsCaseWork.Data.Models
{
    public class ConcernsCase : IAuditable
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ReviewAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public string CrmEnquiry { get; set; }
        public string TrustUkprn { get; set; }
        public string ReasonAtReview { get; set; }
        public DateTime? DeEscalation { get; set; }
        public string Issue { get; set; }
        public string CurrentStatus { get; set; }
        public string CaseAim { get; set; }
        public string DeEscalationPoint { get; set; }
        public string NextSteps { get; set; }
        public string DirectionOfTravel { get; set; }
        public string CaseHistory { get; set; }
        public int Urn { get; set; }
        public int StatusId { get; set; }
        public int RatingId { get; set; }
        
        public Territory? Territory { get; set; }

        public virtual ConcernsStatus Status { get; set; }
        public virtual ConcernsRating Rating { get; set; }
        public virtual ICollection<ConcernsRecord> ConcernsRecords { get; set; }

        public virtual ICollection<Decision> Decisions { get; private set; } = new List<Decision>();

        public void AddDecision(Decision decision, DateTimeOffset now)
        {
	        _ = decision ?? throw new ArgumentNullException(nameof(decision));

	        decision.ConcernsCaseId = Id;
	        decision.UpdatedAt = now;

	        Decisions.Add(decision);
        }

        public void UpdateDecision(int decisionId, Decision updatedDecision, DateTimeOffset now)
        {
	        _ = decisionId > 0 ? decisionId : throw new ArgumentOutOfRangeException(nameof(decisionId));
	        _ = updatedDecision ?? throw new ArgumentNullException(nameof(updatedDecision));

	        var currentDecision = Decisions.FirstOrDefault(x => x.DecisionId == decisionId);
	        if (currentDecision == null)
	        {
		        throw new ArgumentOutOfRangeException(nameof(decisionId),
			        $"Decision id {decisionId} not found in this concerns case. Concerns case urn {Urn}");
	        }

	        currentDecision.UpdatedAt = now;
	        currentDecision.Update(updatedDecision, now);
        }
        
        public void CloseDecision(int decisionId, string notes, DateTimeOffset now)
        {
	        _ = decisionId > 0 ? decisionId : throw new ArgumentOutOfRangeException(nameof(decisionId));

	        var currentDecision = Decisions.FirstOrDefault(x => x.DecisionId == decisionId);
	        if (currentDecision == null)
	        {
		        throw new EntityNotFoundException(decisionId, nameof(Decision));
	        }

	        if (currentDecision.ClosedAt != null)
	        {
		        throw new StateChangeNotAllowedException($"Decision with id {decisionId} cannot be closed as it is already closed.");
	        }
	        
	        if (currentDecision.Outcome == null)
	        {
		        throw new StateChangeNotAllowedException($"Decision with id {decisionId} cannot be closed as it does not have an Outcome.");
	        }
	        
	        currentDecision.Close(notes, now);
        }

        public string Serialise() => JsonSerializer.Serialize(this);
    }
}