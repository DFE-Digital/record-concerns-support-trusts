using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Models
{
	[Parallelizable(ParallelScope.All)]
	public class TrustSearchTests
	{
		[Test]
		public void WhenTrustSearchInterlockIncrement_ReturnsPageIncremented()
		{
			// act
			var trustSearch = TrustFactory.CreateTrustSearch("groupname", "ukprn", "companieshousenumber");
			var nextPage = trustSearch.PageIncrement();

			// assert
			Assert.That(nextPage, Is.EqualTo(2));
			Assert.That(trustSearch.GroupName, Is.EqualTo("groupname"));
			Assert.That(trustSearch.Ukprn, Is.EqualTo("ukprn"));
			Assert.That(trustSearch.CompaniesHouseNumber, Is.EqualTo("companieshousenumber"));
		}
		
		[Test]
		public void WhenTrustSearchInterlockMultipleIncrement_ReturnsPageIncremented()
		{
			// act
			var trustSearch = TrustFactory.CreateTrustSearch("groupname", "ukprn", "companieshousenumber");
			trustSearch.PageIncrement();
			trustSearch.PageIncrement();
			trustSearch.PageIncrement();
			var nextPage = trustSearch.PageIncrement();

			// assert
			Assert.That(nextPage, Is.EqualTo(5));
			Assert.That(trustSearch.GroupName, Is.EqualTo("groupname"));
			Assert.That(trustSearch.Ukprn, Is.EqualTo("ukprn"));
			Assert.That(trustSearch.CompaniesHouseNumber, Is.EqualTo("companieshousenumber"));
		}
		
		[Test]
		public void WhenTrustSearchInterlockParallelIncrement_ReturnsPageIncremented()
		{
			// act
			var trustSearch = TrustFactory.CreateTrustSearch("groupname", "ukprn", "companieshousenumber");

			var tasks = new List<Task<int>>();
			for (var i = 0; i < 10; ++i)
			{
				tasks.Add(Task<int>.Factory.StartNew( () => trustSearch.PageIncrement()));
			}

			Task.WaitAll(tasks.Cast<Task>().ToArray());
			
			var pageValue = trustSearch.Page;

			// assert
			Assert.That(pageValue, Is.EqualTo(11));
			Assert.That(trustSearch.GroupName, Is.EqualTo("groupname"));
			Assert.That(trustSearch.Ukprn, Is.EqualTo("ukprn"));
			Assert.That(trustSearch.CompaniesHouseNumber, Is.EqualTo("companieshousenumber"));
		}
	}
}