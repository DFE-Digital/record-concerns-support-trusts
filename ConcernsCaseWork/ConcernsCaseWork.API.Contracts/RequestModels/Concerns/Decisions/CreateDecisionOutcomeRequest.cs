using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions
{
	public class CreateDecisionOutcomeRequest
	{
		private const int _maxUrlLength = 2048;
		private const int _maxCaseNumberLength = 20;

		public DecisionOutcome DecisionOutcome { get; set; }

		[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "The total amount approved must be zero or greater")]
		public decimal TotalAmountApproved { get; set; }

		public DateTimeOffset? DecisionMadeDate { get; set; }

		public DateTimeOffset? DecisionTakeEffectDate { get; set; }

		public Authoriser Authoriser { get; set; }

		public BusinessArea[] BusinessAreasConsulted { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "The DecisionId must be greater than zero")]
		public int DecisionId { get; set; }

		public CreateDecisionOutcomeRequest()
		{
			BusinessAreasConsulted = new BusinessArea[] { };
		}

		public bool IsValid()
		{
			return BusinessAreasConsulted.All(x => Enum.IsDefined(typeof(BusinessArea), x));
		}
	}
}