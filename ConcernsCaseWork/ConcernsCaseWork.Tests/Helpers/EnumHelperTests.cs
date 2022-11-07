using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Helpers
{
	[Parallelizable(ParallelScope.All)]
	public class EnumHelperTests
	{
		[TestCase(SRMAStatus.TrustConsidering, "Trust considering")]
		[TestCase(SRMAStatus.PreparingForDeployment, "Preparing for deployment")]
		[TestCase(SRMAStatus.Deployed, "Deployed")]
		public void WhenGetEnumDescription_ReturnsExpected(SRMAStatus value, string expectedResult)
		{
			// act
			var result = EnumHelper.GetEnumDescription(value);

			//assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}


	}
}
