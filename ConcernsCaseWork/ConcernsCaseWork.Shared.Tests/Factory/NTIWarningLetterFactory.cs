using AutoFixture;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class NTIWarningLetterFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static List<NtiWarningLetterModel> BuildListNTIWarningLetterModels(int count)
		{
			return Enumerable.Range(0, count).Select(_ => BuildListNTIWarningLetterModel()).ToList();
		}

		public static NtiWarningLetterModel BuildListNTIWarningLetterModel()
		{
			var ntiWarningLetter = new NtiWarningLetterModel();
			ntiWarningLetter.Id = Fixture.Create<long>();
			ntiWarningLetter.CaseUrn = Fixture.Create<long>();
			ntiWarningLetter.Notes = Fixture.Create<string>();

			return ntiWarningLetter;
		}
	}
}