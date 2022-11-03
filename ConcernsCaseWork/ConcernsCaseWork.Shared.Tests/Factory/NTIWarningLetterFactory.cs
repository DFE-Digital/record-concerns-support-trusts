using AutoFixture;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class NTIWarningLetterFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static List<NtiWarningLetterModel> BuildListNTIWarningLetterModels(int count, DateTime? closedAt = null)
		{
			return Enumerable.Range(0, count).Select(_ => BuildNTIWarningLetterModel(closedAt)).ToList();
		}

		public static NtiWarningLetterModel BuildNTIWarningLetterModel(DateTime? closedAt = null)
		{
			var ntiWarningLetter = new NtiWarningLetterModel();
			ntiWarningLetter.Id = Fixture.Create<long>();
			ntiWarningLetter.CaseUrn = Fixture.Create<long>();
			ntiWarningLetter.Notes = Fixture.Create<string>();
			ntiWarningLetter.ClosedAt = closedAt;

			return ntiWarningLetter;
		}
	}
}