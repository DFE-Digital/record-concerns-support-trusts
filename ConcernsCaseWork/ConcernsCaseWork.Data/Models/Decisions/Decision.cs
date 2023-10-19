using ConcernsCaseWork.Data.Enums;
using ConcernsCaseWork.Data.Models.Decisions.Outcome;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Data.Models.Decisions
{
	public record DecisionParameters
	{
		public string CrmCaseNumber { get; set; }

		public bool? HasCrmCase { get; set; }

		public bool? RetrospectiveApproval { get; set; }

		public bool? SubmissionRequired { get; set; }

		public string SubmissionDocumentLink { get; set; }

		public DateTimeOffset ReceivedRequestDate { get; set; }

		public DecisionType[] DecisionTypes { get; set; }

		public decimal TotalAmountRequested { get; set; }

		public string SupportingNotes { get; set; }

		public DateTimeOffset Now { get; set; }
	}

	public class Decision : IAuditable
	{
		public Decision()
		{
			DecisionTypes = new List<DecisionType>();
		}

		public static Decision CreateNew(
			DecisionParameters parameters)
		{
			ValidateDecisionModel(parameters.CrmCaseNumber, parameters.SubmissionDocumentLink, parameters.TotalAmountRequested, parameters.SupportingNotes);

			return new Decision
			{
				DecisionTypes = parameters.DecisionTypes?.ToList() ?? new List<DecisionType>(),
				TotalAmountRequested = parameters.TotalAmountRequested,
				SupportingNotes = parameters.SupportingNotes,
				ReceivedRequestDate = parameters.ReceivedRequestDate,
				SubmissionDocumentLink = parameters.SubmissionDocumentLink,
				SubmissionRequired = parameters.SubmissionRequired,
				RetrospectiveApproval = parameters.RetrospectiveApproval,
				CrmCaseNumber = parameters.CrmCaseNumber,
				HasCrmCase = parameters.HasCrmCase,
				Status = Enums.Concerns.DecisionStatus.InProgress,
				CreatedAt = parameters.Now,
				UpdatedAt = parameters.Now
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
				throw new ArgumentException($"{nameof(crmCaseNumber)} can be a maximum of {MaxCaseNumberLength} characters",
					nameof(crmCaseNumber));

			if (supportingNotes?.Length > MaxSupportingNotesLength)
				throw new ArgumentException(
					$"{nameof(supportingNotes)} can be a maximum of {MaxSupportingNotesLength} characters",
					nameof(supportingNotes));

			if (submissionDocumentLink?.Length > MaxUrlLength)
				throw new ArgumentException($"{nameof(submissionDocumentLink)} can be a maximum of {MaxUrlLength} characters",
					nameof(submissionDocumentLink));
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

		public bool? HasCrmCase { get; set; }

		[StringLength(MaxCaseNumberLength)]
		public string CrmCaseNumber { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }

		public Enums.Concerns.DecisionStatus Status { get; set; }
		public DateTimeOffset? ClosedAt { get; set; }

		public DecisionOutcome Outcome { get; set; }

		public DateTime? DeletedAt { get; set; }

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
			HasCrmCase = updatedDecision.HasCrmCase;
			UpdatedAt = now;
		}

		/// <summary>
		/// Closes the decision, by copying values from another decision. This is in lieu of another suitable class to carry data.
		/// Create an unsaved decision and pass it to this method, all properties that can be copied will be. Existing decision IDs will
		/// be unchanged.
		/// </summary>
		/// <param name="notes"></param>
		/// <param name="updatedAt"></param>
		public void Close(string notes, DateTimeOffset updatedAt)
		{
			ClosedAt = updatedAt;
			SupportingNotes = notes;
			UpdatedAt = updatedAt;
		}

		public void Delete(DateTime now)
		{
			DeletedAt = now;
		}
	}
}
