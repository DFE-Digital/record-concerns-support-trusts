using AutoFixture;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class FinancialPlanFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static FinancialPlanModel BuildFinancialPlanModel(DateTime? closedAt = null)
		{
			return new FinancialPlanModel(
				Fixture.Create<long>(),
				Fixture.Create<long>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<string>(),
				Fixture.Create<FinancialPlanStatusModel>(),
				closedAt
			);
		} 

		public static List<FinancialPlanModel> BuildListFinancialPlanModel(DateTime? closedAt = null)
		{
			return new List<FinancialPlanModel>() { BuildFinancialPlanModel(closedAt) };
		}

		public static List<FinancialPlanModel> BuildListFinancialPlanModel(params FinancialPlanModel[] financialPlanModels)
		{
			return financialPlanModels.ToList();
		}
	}
}
