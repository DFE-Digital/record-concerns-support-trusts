using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Models.CaseActions
{
	public sealed class FinancialPlanStatusModel
	{
		public string Name { get; set; }
		public long Id { get; set; }

		public FinancialPlanStatusModel(string name, long id) =>
			(Name, Id) = (name, id);
	}
}
