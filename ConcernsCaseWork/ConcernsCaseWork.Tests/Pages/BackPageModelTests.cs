using ConcernsCaseWork.Pages.Shared;
using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class BackPageModelTests
	{
		[TestCase("", false)]
		[TestCase("/", false)]
		[TestCase("login", false)]
		[TestCase("/login", false)]
		[TestCase("logout", false)]
		[TestCase("/logout", false)]
		[TestCase("case", true)]
		[TestCase("/case", true)]
		[TestCase("home", false)]
		[TestCase("/home", false)]
		[TestCase("/case/details", true)]
		[TestCase("/case/concerntype", false)]
		[TestCase("/case/1/management", false)]
		[TestCase("/case/1/management/closure", true)]
		public void WhenCanRender_ReturnsExpected(string requestPath, bool expected)
		{
			// act
			var canRender = BackPageModel.CanRender(requestPath);
			
			// assert
			Assert.That(canRender, Is.EqualTo(expected));
		}
	}
}