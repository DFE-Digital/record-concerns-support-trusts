using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class ViewDecisionOutcomeModel
	{
		public ViewDecisionOutcomeModel()
		{
			BusinessAreasConsulted = new List<string>();
		}

		public string Status { get; set; }

		public string? TotalAmount { get; set; }

		public string? DecisionMadeDate { get; set; }

		public string? DecisionEffectiveFromDate { get; set; }

		public string? Authorizer { get; set; }

		public List<string> BusinessAreasConsulted { get; set; }

		public string EditLink { get; set; }
	}
}
