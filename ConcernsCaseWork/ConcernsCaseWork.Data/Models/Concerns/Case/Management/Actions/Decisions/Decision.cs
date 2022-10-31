using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions
{
    public class Decision
    {
        private Decision()
        {
            DecisionTypes = new List<DecisionType>();
        }

        public static Decision CreateNew(
            int concernsCaseId,
            string crmCaseNumber,
            bool? retrospectiveApproval,
            bool? submissionRequired,
            string submissionDocumentLink,
            DateTimeOffset receivedRequestDate,
            DecisionType[] decisionTypes,
            decimal totalAmountRequested,
            string supportingNotes,
            DateTimeOffset createdAt
        )
        {
            // some of these validations are good candidates for turning into value types
            _ = concernsCaseId > 0 ? concernsCaseId : throw new ArgumentOutOfRangeException(nameof(concernsCaseId), "value must be greater than zero");
            _ = totalAmountRequested >= 0 ? totalAmountRequested : throw new ArgumentOutOfRangeException(nameof(totalAmountRequested), "The total amount requested cannot be a negative value");

            if (crmCaseNumber?.Length > MaxCaseNumberLength)
            {
                throw new ArgumentException($"{nameof(crmCaseNumber)} can be a maximum of {MaxCaseNumberLength} characters", nameof(crmCaseNumber));
            }

            if (supportingNotes?.Length > MaxSupportingNotesLength)
            {
                throw new ArgumentException($"{nameof(supportingNotes)} can be a maximum of {MaxSupportingNotesLength} characters", nameof(supportingNotes));
            }

            if (submissionDocumentLink?.Length > MaxUrlLength)
            {
                throw new ArgumentException($"{nameof(submissionDocumentLink)} can be a maximum of {MaxUrlLength} characters", nameof(submissionDocumentLink));
            }

            return new Decision()
            {
                ConcernsCaseId = concernsCaseId,
                DecisionTypes = decisionTypes ?? Array.Empty<DecisionType>(),
                TotalAmountRequested = totalAmountRequested,
                SupportingNotes = supportingNotes,
                ReceivedRequestDate = receivedRequestDate,
                SubmissionDocumentLink = submissionDocumentLink,
                SubmissionRequired = submissionRequired,
                RetrospectiveApproval = retrospectiveApproval,
                CrmCaseNumber = crmCaseNumber,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                Status = Enums.Concerns.DecisionStatus.InProgress
            };

        }

        public const int MaxUrlLength = 2048;
        public const int MaxSupportingNotesLength = 2000;
        public const int MaxCaseNumberLength = 20;

        public int ConcernsCaseId { get; set;}

        public int DecisionId { get; set; }
        public IList<DecisionType> DecisionTypes { get; set;}

        // nullable
        public decimal TotalAmountRequested { get; set;}
        
        [StringLength(MaxSupportingNotesLength)]
        public string SupportingNotes { get; set;}
        
        public DateTimeOffset ReceivedRequestDate { get; set;}
        
        [StringLength(MaxUrlLength)]
        public string SubmissionDocumentLink { get; set;}
        
        public bool? SubmissionRequired { get; set;}
        
        public bool? RetrospectiveApproval { get; set;}

        [StringLength(MaxCaseNumberLength)]
        public string CrmCaseNumber { get; set;}
        public DateTimeOffset CreatedAt { get; set;}
        public DateTimeOffset UpdatedAt { get; set;}

        public Enums.Concerns.DecisionStatus Status { get; set;}
        public DateTimeOffset? ClosedAt { get; set; }

    }
}
