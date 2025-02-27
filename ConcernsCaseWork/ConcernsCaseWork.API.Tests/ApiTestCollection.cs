using ConcernsCaseWork.API.Tests.Fixtures;
using Xunit;

namespace ConcernsCaseWork.API.Tests
{
	[CollectionDefinition("ApiTestCollection", DisableParallelization = true)]
	public class ApiTestCollection : ICollectionFixture<ApiTestFixture>
	{
		public const string ApiTestCollectionName = "ApiTestCollection";

		// No code needed here. It's just the xUnit pattern for shared fixture.
	}
}