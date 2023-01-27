using AutoFixture;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class FinancialPlanFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static FinancialPlanModel BuildClosedFinancialPlanModel(DateTime closedAt)
		{
			var model = new FinancialPlanModel(
				Fixture.Create<long>(),
				Fixture.Create<long>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<string>(),
				new FinancialPlanStatusModel(Fixture.Create<string>(), Fixture.Create<long>(), true),
				closedAt,
				Fixture.Create<DateTime>()
			);

			return model;
		} 
		
		public static FinancialPlanModel BuildOpenFinancialPlanModel()
		{
			var model = new FinancialPlanModel(
				Fixture.Create<long>(),
				Fixture.Create<long>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<string>(),
				new FinancialPlanStatusModel(Fixture.Create<string>(), Fixture.Create<long>(), false),
				null,
				Fixture.Create<DateTime>()
			);
			return model;
		} 

		public static List<FinancialPlanModel> BuildListFinancialPlanModel(DateTime? closedAt = null)
		{
			return closedAt.HasValue 
				? new List<FinancialPlanModel>() { BuildClosedFinancialPlanModel((DateTime)closedAt) } 
				: new List<FinancialPlanModel>() { BuildOpenFinancialPlanModel() };
		}

		public static List<FinancialPlanModel> BuildListFinancialPlanModel(params FinancialPlanModel[] financialPlanModels)
		{
			return financialPlanModels.ToList();
		}
	}
}
