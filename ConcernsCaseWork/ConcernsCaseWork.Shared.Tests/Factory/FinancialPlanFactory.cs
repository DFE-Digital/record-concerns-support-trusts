using AutoFixture;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class FinancialPlanFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static FinancialPlanModel BuildFinancialPlanModel()
		{
			return new FinancialPlanModel(
				Fixture.Create<long>(),
				Fixture.Create<long>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<FinancialPlanStatus>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<string>()
			);
		} 

		public static List<FinancialPlanModel> BuildListFinancialPlanModel()
		{
			return new List<FinancialPlanModel>() { BuildFinancialPlanModel() };
		}
	}
}
