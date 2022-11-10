﻿using ConcernsCaseWork.API.Contracts.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ConcernsCaseWork.Service.Decision
{
	public class DecisionDto
	{
		private const int _maxUrlLength = 2048;
		private const int _maxSupportingNotesLength = 2000;
		private const int _maxCaseNumberLength = 20;

		[Range(1, int.MaxValue, ErrorMessage = "The ConcernsCaseUrn must be greater than zero")]
		public long ConcernsCaseUrn { get; set; } // TODO: Remove this and pass urn separately to the decision request.

		[Range(1, int.MaxValue, ErrorMessage = "The DecisionId must be greater than zero")]
		public long DecisionId { get; set; } // TODO: Remove this and pass urn separately to the decision request.

		public DecisionType[] DecisionTypes { get; set; }

		[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "The total amount requested must be zero or greater")]
		public decimal TotalAmountRequested { get; set; }

		[StringLength(_maxSupportingNotesLength, ErrorMessage  = "Notes must be 2000 characters or less", MinimumLength = 0)]
		public string SupportingNotes { get; set; }

		// TODO: This should potentially be optional. Waiting for Ben to clarify.
		public DateTimeOffset? ReceivedRequestDate { get; set; }

		[StringLength(_maxUrlLength, ErrorMessage = "Submission document link must be 2048 or less", MinimumLength = 0)]
		public string SubmissionDocumentLink { get; set; }

		public bool? SubmissionRequired { get; set; }

		public bool? RetrospectiveApproval { get; set; }

		[StringLength(_maxCaseNumberLength, MinimumLength = 0)]
		public string CrmCaseNumber { get; set; }

		public bool IsValid()
		{
			return DecisionTypes.All(x => Enum.IsDefined(typeof(DecisionType), x));
		}
	}
}