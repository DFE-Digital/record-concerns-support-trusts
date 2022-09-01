using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Service.Redis.Base;
using Service.Redis.Teams;
using Service.TRAMS.Teams;
using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Teams
{
	[Parallelizable(ParallelScope.All)]
	public class TeamsCachedServiceTests
	{
		[Test]
		public void TeamsCachedService_Decorates_ITeamsService()
		{
			var sut = new TeamsCachedService(Mock.Of<ILogger<TeamsCachedService>>(), Mock.Of<ITeamsService>(), Mock.Of<ICacheProvider>());
			Assert.That(sut, Is.AssignableTo<ITeamsService>());
			Assert.That(sut, Is.AssignableTo<ITeamsCachedService>());
		}

		[Test]
		public void Methods_GuardAgainstNullArgs()
		{
			var fixture = new AutoFixture.Fixture();
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamsCachedService).GetMethods());
		}
		
		[Test]
		public void Constructors_GuardAgainstNullArgs()
		{
			var fixture = new AutoFixture.Fixture();
			fixture.Customize(new AutoMoqCustomization());
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamsCachedService).GetConstructors());
		}
	}
}
