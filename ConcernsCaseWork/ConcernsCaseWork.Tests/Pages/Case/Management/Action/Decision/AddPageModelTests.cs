using AutoFixture;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Pages.Case.Management.Action.Decision;
using ConcernsCaseWork.Pages.Base;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.Metrics;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Decision
{
	[Parallelizable(ParallelScope.All)]
	public class AddPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnPage()
		{
			var fixture = new Fixture();


			var mockLogger = new Mock<ILogger<AddPageModel>>();
			fixture.Customize<ILogger<AddPageModel>>(x => x.FromFactory(() => mockLogger.Object));
			var sut = fixture.Create<AddPageModel>();

			Assert.NotNull(sut as AbstractPageModel);
			//Assert.That(sut, Is.EqualTo(typeof))
		}
	}
}

//AutoFixture.ObjectCreationExceptionWithPath : AutoFixture was unable to create an instance from System.Reflection.MethodInfo because it's an abstract class. There's no single, most appropriate way to create an object deriving from that class, but you can help AutoFixture figure it out.