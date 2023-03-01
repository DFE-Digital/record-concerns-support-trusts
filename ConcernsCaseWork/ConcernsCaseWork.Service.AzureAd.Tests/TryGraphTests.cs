namespace ConcernsCaseWork.Service.AzureAd.Tests;

public class TryGraphTests
{
	[Fact]
	public async Task TryGraph()
	{
		var sut = new ConcernsCaseWork.Service.AzureAd.GraphManager();
		var members = await sut.MyGetMembersInGroup("");
	}
}