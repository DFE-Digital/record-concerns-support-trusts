using ConcernsCaseWork.Data.Enums;
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
            string crmCaseNumber,
            bool? retrospectiveApproval,
            bool? submissionRequired,
            string submissionDocumentLink,
            DateTimeOffset receivedRequestDate,
            DecisionType[] decisionTypes,
            decimal totalAmountRequested,
            string supportingNotes,
            DateTimeOffset now
        )
        {
            ValidateDecisionModel(crmCaseNumber, submissionDocumentLink, totalAmountRequested, supportingNotes);

            return new Decision
            {
                DecisionTypes = decisionTypes?.ToList() ?? new List<DecisionType>(),
                TotalAmountRequested = totalAmountRequested,
                SupportingNotes = supportingNotes,
                ReceivedRequestDate = receivedRequestDate,
                SubmissionDocumentLink = submissionDocumentLink,
                SubmissionRequired = submissionRequired,
                RetrospectiveApproval = retrospectiveApproval,
                CrmCaseNumber = crmCaseNumber,
                Status = Enums.Concerns.DecisionStatus.InProgress,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        /// <summary>
        /// Validates that the properties of a decision are valid, and should be called before creation/update is made.
        /// If invalid properties are found an exception is thrown. In future it might be better to return an exceptions collection.
        /// Some of these validations are good candidates for turning into value types
        /// </summary>
        /// <param name="crmCaseNumber"></param>
        /// <param name="submissionDocumentLink"></param>
        /// <param name="totalAmountRequested"></param>
        /// <param name="supportingNotes"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private static void ValidateDecisionModel(string crmCaseNumber, string submissionDocumentLink,
            decimal totalAmountRequested, string supportingNotes)
        {
            // some of these validations are good candidates for turning into value types
            _ = totalAmountRequested >= 0
                ? totalAmountRequested
                : throw new ArgumentOutOfRangeException(nameof(totalAmountRequested),
                    "The total amount requested cannot be a negative value");

            if (crmCaseNumber?.Length > MaxCaseNumberLength)
            {
                throw new ArgumentException($"{nameof(crmCaseNumber)} can be a maximum of {MaxCaseNumberLength} characters",
                    nameof(crmCaseNumber));
            }

            if (supportingNotes?.Length > MaxSupportingNotesLength)
            {
                throw new ArgumentException(
                    $"{nameof(supportingNotes)} can be a maximum of {MaxSupportingNotesLength} characters",
                    nameof(supportingNotes));
            }

            if (submissionDocumentLink?.Length > MaxUrlLength)
            {
                throw new ArgumentException($"{nameof(submissionDocumentLink)} can be a maximum of {MaxUrlLength} characters",
                    nameof(submissionDocumentLink));
            }
        }

        public const int MaxUrlLength = 2048;
        public const int MaxSupportingNotesLength = 2000;
        public const int MaxCaseNumberLength = 20;

        public int ConcernsCaseId { get; set; }

        public int DecisionId { get; set; }
        public ICollection<DecisionType> DecisionTypes { get; set; }

        // nullable
        public decimal TotalAmountRequested { get; set; }

        [StringLength(MaxSupportingNotesLength)]
        public string SupportingNotes { get; set; }

        public DateTimeOffset ReceivedRequestDate { get; set; }

        [StringLength(MaxUrlLength)]
        public string SubmissionDocumentLink { get; set; }

        public bool? SubmissionRequired { get; set; }

        public bool? RetrospectiveApproval { get; set; }

        [StringLength(MaxCaseNumberLength)]
        public string CrmCaseNumber { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public Enums.Concerns.DecisionStatus Status { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }

        public string GetTitle()
        {
            switch (DecisionTypes?.Count ?? 0)
            {
                case 0:
                    return "No Decision Types";

                case int i when i > 1:
                    return "Multiple Decision Types";

                default:
                    return DecisionTypes.First().DecisionTypeId.GetDescription();
            }
        }

        /// <summary>
        /// Updates the decision, by copying values from another decision. This is in lieu of another suitable class to carry data.
        /// Create an unsaved decision and pass it to this method, all properties that can be copied will be. Existing decision IDs will
        /// be unchanged.
        /// </summary>
        /// <param name="updatedDecision"></param>
        /// <param name="now"></param>
        public void Update(Decision updatedDecision, DateTimeOffset now)
        {
            _ = updatedDecision ?? throw new ArgumentNullException(nameof(updatedDecision));
            ValidateDecisionModel(updatedDecision.CrmCaseNumber, updatedDecision.SubmissionDocumentLink, updatedDecision.TotalAmountRequested, updatedDecision.SupportingNotes);

            DecisionTypes = updatedDecision.DecisionTypes ?? Array.Empty<DecisionType>();
            TotalAmountRequested = updatedDecision.TotalAmountRequested;
            SupportingNotes = updatedDecision.SupportingNotes;
            ReceivedRequestDate = updatedDecision.ReceivedRequestDate;
            SubmissionDocumentLink = updatedDecision.SubmissionDocumentLink;
            SubmissionRequired = updatedDecision.SubmissionRequired;
            RetrospectiveApproval = updatedDecision.RetrospectiveApproval;
            CrmCaseNumber = updatedDecision.CrmCaseNumber;
            UpdatedAt = now;
        }
    }
}
