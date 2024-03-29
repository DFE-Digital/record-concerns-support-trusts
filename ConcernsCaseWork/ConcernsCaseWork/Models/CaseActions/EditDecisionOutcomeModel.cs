﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.Models.Validatable;
using System.ComponentModel;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class EditDecisionOutcomeModel
	{
		public EditDecisionOutcomeModel()
		{
			BusinessAreasConsulted = new List<DecisionOutcomeBusinessArea>();
			DecisionMadeDate = new OptionalDateModel();
			DecisionEffectiveFromDate = new OptionalDateModel();
		}

		public DecisionOutcomeStatus Status { get; set; }

		[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "The total amount requested must be zero or greater")]
		public decimal? TotalAmount { get; set; }

		[DisplayName("Date decision made")]
		public OptionalDateModel DecisionMadeDate { get; set; }

		[DisplayName("Date decision effective")]
		public OptionalDateModel DecisionEffectiveFromDate { get; set; }

		[EnumDataType(typeof(DecisionOutcomeAuthorizer))]
		public DecisionOutcomeAuthorizer? Authorizer { get; set; }

		public List<DecisionOutcomeBusinessArea> BusinessAreasConsulted { get; set; }
	}
}
