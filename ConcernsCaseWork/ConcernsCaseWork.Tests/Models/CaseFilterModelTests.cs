#nullable enable
using AutoFixture;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Models;
using FluentAssertions;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Models
{

	[TestFixture]
	public class CaseFiltersTests
	{
		[Test]
		public void PersistUsing_EmptyFilterStore_AllSelectedProperties_ShouldBeEmpty()
		{
			var filters = new CaseFilters();
			var filterStore = new Dictionary<string, object?>();

			var result = filters.PersistUsing(filterStore);

			filters.SelectedRegions.Should().BeEmpty();
			filters.SelectedCaseOwners.Should().BeEmpty();
			filters.SelectedTeamLeaders.Should().BeEmpty();
			filters.SelectedStatuses.Should().BeEmpty();
			result.Should().BeSameAs(filters);
		}

		[Test]
		public void PersistUsing_OnlyRegionsKey_OnlySelectedRegionsSet()
		{
			var filters = new CaseFilters();
			var expectedRegions = new[] { nameof(Region.EastMidlands), nameof(Region.London) };

			var filterStore = new Dictionary<string, object?> { { CaseFilters.FilterRegions, expectedRegions } };

			filters.PersistUsing(filterStore);

			filters.SelectedRegions.Should().Equal(expectedRegions);
			filters.SelectedCaseOwners.Should().BeEmpty();
			filters.SelectedTeamLeaders.Should().BeEmpty();
			filters.SelectedStatuses.Should().BeEmpty();
		}

		[Test]
		public void PersistUsing_AllKeysWithValidArrays_AllSelectedProperties_ShouldBeSet()
		{
			var filters = new CaseFilters();
			var expectedRegions = new[] { nameof(Region.EastMidlands) };
			var expectedOwners = new[] { "Alice", "Bob" };
			var expectedLeaders = new[] { "Carol" };
			var expectedStatuses = new[] { nameof(CaseStatus.Close), nameof(CaseStatus.Live) };
			var filterStore = new Dictionary<string, object?>
			{
				{ CaseFilters.FilterRegions, expectedRegions },
				{ CaseFilters.FilterCaseOwners, expectedOwners },
				{ CaseFilters.FilterTeamLeaders, expectedLeaders },
				{ CaseFilters.FilterStatuses, expectedStatuses }
			};

			filters.PersistUsing(filterStore);

			filters.SelectedRegions.Should().Equal(expectedRegions);
			filters.SelectedCaseOwners.Should().Equal(expectedOwners);
			filters.SelectedTeamLeaders.Should().Equal(expectedLeaders);
			filters.SelectedStatuses.Should().Equal(expectedStatuses);
		}

		[TestCase(CaseFilters.FilterRegions)]
		[TestCase(CaseFilters.FilterCaseOwners)]
		[TestCase(CaseFilters.FilterTeamLeaders)]
		[TestCase(CaseFilters.FilterStatuses)]
		public void PersistUsing_KeyWithNullValue_SelectedProperty_ShouldBeEmpty(string key)
		{
			var filters = new CaseFilters();
			var filterStore = new Dictionary<string, object?> { { key, null } };

			filters.PersistUsing(filterStore);

			GetSelectedProperty(filters, key).Should().BeEmpty();
		}

		[TestCase(CaseFilters.FilterRegions)]
		[TestCase(CaseFilters.FilterCaseOwners)]
		[TestCase(CaseFilters.FilterTeamLeaders)]
		[TestCase(CaseFilters.FilterStatuses)]
		public void PersistUsing_KeyWithWrongTypeValue_SelectedProperty_ShouldBeEmpty(string key)
		{
			var filters = new CaseFilters();
			var filterStore = new Dictionary<string, object?> { { key, 123 } };

			filters.PersistUsing(filterStore);

			GetSelectedProperty(filters, key).Should().BeEmpty();
		}

		[Test]
		public void PersistUsing_ExtraKeys_OnlyExpectedPropertiesSet()
		{
			var filters = new CaseFilters();
			var expectedOwners = new[] { "Owner1" };
			var filterStore = new Dictionary<string, object?> { { "UnexpectedKey", new[] { "Value" } }, { CaseFilters.FilterCaseOwners, expectedOwners } };

			filters.PersistUsing(filterStore);

			filters.SelectedCaseOwners.Should().Equal(expectedOwners);
			filters.SelectedRegions.Should().BeEmpty();
			filters.SelectedTeamLeaders.Should().BeEmpty();
			filters.SelectedStatuses.Should().BeEmpty();
		}

		private static string[] GetSelectedProperty(CaseFilters filters, string key)
		{
			return key switch
			{
				CaseFilters.FilterRegions => filters.SelectedRegions,
				CaseFilters.FilterCaseOwners => filters.SelectedCaseOwners,
				CaseFilters.FilterTeamLeaders => filters.SelectedTeamLeaders,
				CaseFilters.FilterStatuses => filters.SelectedStatuses,
				_ => throw new ArgumentException("Invalid key", nameof(key))
			};
		}

		[Test]
		public void SelectedRegionEnums_EmptySelectedRegions_ShouldReturnEmptyArray()
		{
			var filters = new CaseFilters();

			var result = filters.SelectedRegionEnums;

			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}


		[Test]
		public void SelectedRegionEnums_DuplicateInputs_ShouldReturnDistinctRegions()
		{
			var filters = new CaseFilters();
			SetSelectedRegions(filters, ["London", "3", "london", "London"]);

			var result = filters.SelectedRegionEnums;

			result.Should().BeEquivalentTo([Region.London]);
		}

		[Test]
		public void SelectedRegionEnums_MixedValidAndInvalidInputs_ShouldReturnOnlyValidRegions()
		{
			var filters = new CaseFilters();
			SetSelectedRegions(filters, ["London", "foo", "2", "", "SouthEast", "999", " "]);

			var result = filters.SelectedRegionEnums;

			result.Should().BeEquivalentTo([Region.London, Region.EastOfEngland, Region.SouthEast]);
		}

		[Test]
		public void SelectedRegionEnums_AllValidRegionNames_ShouldReturnAllRegions()
		{
			var allNames = Enum.GetNames(typeof(Region));
			var filters = new CaseFilters();
			SetSelectedRegions(filters, allNames);

			var result = filters.SelectedRegionEnums;

			result.Should().BeEquivalentTo(Enum.GetValues(typeof(Region)).Cast<Region>());
		}

		[Test]
		public void SelectedRegionEnums_AllValidRegionNumbers_ShouldReturnAllRegions()
		{
			var allNumbers = Enum.GetValues(typeof(Region)).Cast<int>().Select(i => i.ToString()).ToArray();
			var filters = new CaseFilters();
			SetSelectedRegions(filters, allNumbers);

			var result = filters.SelectedRegionEnums;

			result.Should().BeEquivalentTo(Enum.GetValues(typeof(Region)).Cast<Region>());
		}

		private static void SetSelectedRegions(CaseFilters filters, string[]? values)
		{
			var prop = typeof(CaseFilters).GetProperty("SelectedRegions");
			prop!.SetValue(filters, values ?? []);
		}

		[Test]
		public void IsVisible_AllFiltersEmptyOrInvalid_ShouldReturnFalse()
		{
			var filters = new CaseFilters();
			var query = new List<KeyValuePair<string, StringValues>> { new(nameof(CaseFilters.SelectedRegions), new StringValues("invalid-region")) };

			filters.PopulateFrom(query);

			var result = filters.IsVisible;

			result.Should().BeFalse();
		}

		[Test]
		public void IsVisible_ValidRegionOnly_ShouldReturnTrue()
		{
			var filters = new CaseFilters();
			var query = new List<KeyValuePair<string, StringValues>> { new(nameof(CaseFilters.SelectedRegions), new StringValues(nameof(Region.London))) };

			filters.PopulateFrom(query);

			var result = filters.IsVisible;

			result.Should().BeTrue();
		}

		[Test]
		public void IsVisible_CaseOwnersOnly_ShouldReturnTrue()
		{
			var filters = new CaseFilters();
			var query = new List<KeyValuePair<string, StringValues>> { new(nameof(CaseFilters.SelectedCaseOwners), new StringValues("owner1")) };

			filters.PopulateFrom(query);

			var result = filters.IsVisible;

			result.Should().BeTrue();
		}

		[Test]
		public void IsVisible_TeamLeadersOnly_ShouldReturnTrue()
		{
			var filters = new CaseFilters();
			var query = new List<KeyValuePair<string, StringValues>> { new(nameof(CaseFilters.SelectedTeamLeaders), new StringValues("leader1")) };

			filters.PopulateFrom(query);

			var result = filters.IsVisible;

			result.Should().BeTrue();
		}

		[Test]
		public void IsVisible_StatusesOnly_ShouldReturnTrue()
		{
			var filters = new CaseFilters();
			var query = new List<KeyValuePair<string, StringValues>> { new(nameof(CaseFilters.SelectedStatuses), new StringValues(nameof(CaseStatus.Live))) };

			filters.PopulateFrom(query);

			var result = filters.IsVisible;

			result.Should().BeTrue();
		}

		[Test]
		public void IsVisible_MultipleFiltersSet_ShouldReturnTrue()
		{
			var filters = new CaseFilters();
			var query = new List<KeyValuePair<string, StringValues>>
			{
				new(nameof(CaseFilters.SelectedRegions), new StringValues(nameof(Region.London))),
				new(nameof(CaseFilters.SelectedCaseOwners), new StringValues("owner1")),
				new(nameof(CaseFilters.SelectedTeamLeaders), new StringValues("leader1")),
				new(nameof(CaseFilters.SelectedStatuses), new StringValues(nameof(CaseStatus.Live)))
			};

			filters.PopulateFrom(query);

			var result = filters.IsVisible;

			result.Should().BeTrue();
		}

		[TestCase(new string[0], new string[0], new string[0], new string[0], false, TestName = "IsVisible_AllEmpty_ShouldReturnFalse")]
		[TestCase(new[] { "1" }, new string[0], new string[0], new string[0], true, TestName = "IsVisible_ValidRegion_ShouldReturnTrue")]
		[TestCase(new[] { "invalid" }, new string[0], new string[0], new string[0], false, TestName = "IsVisible_InvalidRegion_ShouldReturnFalse")]
		[TestCase(new string[0], new[] { "owner1" }, new string[0], new string[0], true, TestName = "IsVisible_CaseOwner_ShouldReturnTrue")]
		[TestCase(new string[0], new string[0], new[] { "leader1" }, new string[0], true, TestName = "IsVisible_TeamLeader_ShouldReturnTrue")]
		[TestCase(new string[0], new string[0], new string[0], new[] { "0" }, true, TestName = "IsVisible_Status_ShouldReturnTrue")]
		[TestCase(new[] { "1" }, new[] { "owner1" }, new[] { "leader1" }, new[] { "0" }, true, TestName = "IsVisible_AllFilters_ShouldReturnTrue")]
		public void IsVisible_CombinationTests(string[] regions, string[] caseOwners, string[] teamLeaders, string[] statuses, bool expected)
		{
			var filters = new CaseFilters();
			var query = new List<KeyValuePair<string, StringValues>>();

			if (regions.Length > 0)
				query.Add(new KeyValuePair<string, StringValues>(nameof(CaseFilters.SelectedRegions), new StringValues(regions)));
			if (caseOwners.Length > 0)
				query.Add(new KeyValuePair<string, StringValues>(nameof(CaseFilters.SelectedCaseOwners), new StringValues(caseOwners)));
			if (teamLeaders.Length > 0)
				query.Add(new KeyValuePair<string, StringValues>(nameof(CaseFilters.SelectedTeamLeaders), new StringValues(teamLeaders)));
			if (statuses.Length > 0)
				query.Add(new KeyValuePair<string, StringValues>(nameof(CaseFilters.SelectedStatuses), new StringValues(statuses)));

			filters.PopulateFrom(query);

			var result = filters.IsVisible;

			result.Should().Be(expected);
		}

		[Test]
		[TestCase(null, null, null, TestName = "SetCaseTeamLeaders_NullInput_ShouldAssignNull")]
		[TestCase(null, new string[0], new string[0], TestName = "SetCaseTeamLeaders_EmptyList_ShouldAssignEmptyList")]
		[TestCase(null, new[] { "A" }, new[] { "A" }, TestName = "SetCaseTeamLeaders_SingleItem_ShouldAssignCorrectly")]
		[TestCase(null, new[] { "A", "B", "C" }, new[] { "A", "B", "C" }, TestName = "SetCaseTeamLeaders_MultipleItems_ShouldAssignCorrectly")]
		public void SetCaseTeamLeaders_VariousInputs_ShouldAssignCorrectly(string[]? initialList, string[]? inputList, string[]? expectedList)
		{
			var model = new CaseFilters();
			if (initialList != null)
			{
				model.SetCaseTeamLeaders([..initialList]);
			}

			List<string> input = inputList == null ? [] : [..inputList];

			model.SetCaseTeamLeaders(input);

			if (expectedList == null)
			{
				model.TeamLeaders.Should().BeEmpty();
			}
			else
			{
				model.TeamLeaders.Should().NotBeNull();
				model.TeamLeaders.Should().BeEquivalentTo(expectedList);
			}
		}

		[Test]
		public void SetCaseTeamLeaders_DuplicateValues_AssignsDuplicates()
		{
			var model = new CaseFilters();
			var duplicates = new List<string> { "A", "A", "B" };

			model.SetCaseTeamLeaders(duplicates);

			model.TeamLeaders.Should().BeEquivalentTo(duplicates);
		}

		[Test]
		public void PopulateFrom_EmptyQuery_SelectedArrays_ShouldBeEmpty()
		{
			var filters = new CaseFilters();
			var query = Enumerable.Empty<KeyValuePair<string, StringValues>>();

			filters.PopulateFrom(query);

			filters.SelectedRegions.Should().BeEmpty();
			filters.SelectedCaseOwners.Should().BeEmpty();
			filters.SelectedTeamLeaders.Should().BeEmpty();
			filters.SelectedStatuses.Should().BeEmpty();
		}

		[Test]
		public void PopulateFrom_ClearKey_AllSelectedArrays_ShouldBeEmpty()
		{
			var filters = new CaseFilters();
			filters.PopulateFrom([
				new KeyValuePair<string, StringValues>("SelectedRegions", new StringValues(nameof(Region.EastMidlands))),
				new KeyValuePair<string, StringValues>("SelectedCaseOwners", new StringValues("Owner1")),
				new KeyValuePair<string, StringValues>("SelectedTeamLeaders", new StringValues("Leader1")),
				new KeyValuePair<string, StringValues>("SelectedStatuses", new StringValues(nameof(CaseStatus.Live)))
			]);

			var query = new[] { new KeyValuePair<string, StringValues>("clear", new StringValues("true")) };

			filters.PopulateFrom(query);

			filters.SelectedRegions.Should().BeEmpty();
			filters.SelectedCaseOwners.Should().BeEmpty();
			filters.SelectedTeamLeaders.Should().BeEmpty();
			filters.SelectedStatuses.Should().BeEmpty();
		}

		[Test]
		public void PopulateFrom_RemoveKey_RemovesValuesFromEachFilter()
		{
			var filters = new CaseFilters();
			filters.PopulateFrom([
				new(nameof(CaseFilters.SelectedRegions), new StringValues([nameof(Region.EastMidlands), nameof(Region.WestMidlands)])),
				new(nameof(CaseFilters.SelectedCaseOwners), new StringValues(["Owner1", "Owner2"])),
				new(nameof(CaseFilters.SelectedTeamLeaders), new StringValues(["Leader1", "Leader2"])),
				new(nameof(CaseFilters.SelectedStatuses), new StringValues([nameof(CaseStatus.Live), nameof(CaseStatus.Close)]))
			]);

			var query = new[]
			{
				new KeyValuePair<string, StringValues>("remove", new StringValues("true")),
				new KeyValuePair<string, StringValues>("SelectedRegions", new StringValues(nameof(Region.EastMidlands))),
				new KeyValuePair<string, StringValues>("SelectedCaseOwners", new StringValues("Owner2")),
				new KeyValuePair<string, StringValues>("SelectedTeamLeaders", new StringValues("Leader1")),
				new KeyValuePair<string, StringValues>("SelectedStatuses", new StringValues(nameof(CaseStatus.Close)))
			};

			filters.PopulateFrom(query);

			filters.SelectedRegions.Should().BeEquivalentTo(nameof(Region.WestMidlands));
			filters.SelectedCaseOwners.Should().BeEquivalentTo("Owner1");
			filters.SelectedTeamLeaders.Should().BeEquivalentTo("Leader2");
			filters.SelectedStatuses.Should().BeEquivalentTo(nameof(CaseStatus.Live));
		}

		[TestCase("SelectedRegions", new[] { nameof(Region.EastMidlands), nameof(Region.WestMidlands) }, TestName = "PopulateFrom_OnlySelectedRegions_UpdatesRegions")]
		[TestCase("SelectedCaseOwners", new[] { "Alice", "Bob" }, TestName = "PopulateFrom_OnlySelectedCaseOwners_UpdatesCaseOwners")]
		[TestCase("SelectedTeamLeaders", new[] { "TL1", "TL2" }, TestName = "PopulateFrom_OnlySelectedTeamLeaders_UpdatesTeamLeaders")]
		[TestCase("SelectedStatuses", new[] { nameof(CaseStatus.Live), nameof(CaseStatus.Close) }, TestName = "PopulateFrom_OnlySelectedStatuses_UpdatesStatuses")]
		public void PopulateFrom_SingleFilterKey_OnlyThatArrayUpdated(string key, string[] values)
		{
			var filters = new CaseFilters();
			var query = new[] { new KeyValuePair<string, StringValues>(key, new StringValues(values)) };

			filters.PopulateFrom(query);

			filters.SelectedRegions.Should().BeEquivalentTo(key == "SelectedRegions" ? values : []);
			filters.SelectedCaseOwners.Should().BeEquivalentTo(key == "SelectedCaseOwners" ? values : []);
			filters.SelectedTeamLeaders.Should().BeEquivalentTo(key == "SelectedTeamLeaders" ? values : []);
			filters.SelectedStatuses.Should().BeEquivalentTo(key == "SelectedStatuses" ? values : []);
		}

		[Test]
		public void PopulateFrom_MultipleFilterKeys_AllArraysUpdated()
		{
			var filters = new CaseFilters();
			var query = new[]
			{
				new KeyValuePair<string, StringValues>("SelectedRegions", new StringValues([nameof(Region.EastMidlands), nameof(Region.WestMidlands)])),
				new KeyValuePair<string, StringValues>("SelectedCaseOwners", new StringValues(["OwnerA"])),
				new KeyValuePair<string, StringValues>("SelectedTeamLeaders", new StringValues(["LeaderB"])),
				new KeyValuePair<string, StringValues>("SelectedStatuses", new StringValues([nameof(CaseStatus.Live), nameof(CaseStatus.Close)]))
			};

			filters.PopulateFrom(query);

			filters.SelectedRegions.Should().BeEquivalentTo(nameof(Region.EastMidlands), nameof(Region.WestMidlands));
			filters.SelectedCaseOwners.Should().BeEquivalentTo("OwnerA");
			filters.SelectedTeamLeaders.Should().BeEquivalentTo("LeaderB");
			filters.SelectedStatuses.Should().BeEquivalentTo(nameof(CaseStatus.Live), nameof(CaseStatus.Close));
		}


		[Test]
		public void SetCaseOwners_EmptyList_CaseOwners_ShouldBeEmpty()
		{
			var model = new CaseFilters();
			var emptyList = new List<string>();

			model.SetCaseOwners(emptyList);

			model.CaseOwners.Should().BeEmpty();
			ReferenceEquals(model.CaseOwners, emptyList).Should().BeTrue();
		}

		[Test]
		public void SetCaseOwners_SingleItemList_CaseOwners_ShouldContainItem()
		{
			var model = new CaseFilters();
			var singleItemList = new List<string> { "owner1" };

			model.SetCaseOwners(singleItemList);

			model.CaseOwners.Should().BeEquivalentTo(singleItemList);
			ReferenceEquals(model.CaseOwners, singleItemList).Should().BeTrue();
		}

		[Test]
		public void SetCaseOwners_MultipleAndSpecialItems_CaseOwners_ShouldContainAllItems()
		{
			var model = new CaseFilters();
			var items = new List<string>
			{
				"owner1",
				"owner2",
				"owner1",
				"",
				" ",
				"\t",
				"特殊字符",
				"owner3"
			};

			model.SetCaseOwners(items);

			model.CaseOwners.Should().BeEquivalentTo(items);
			ReferenceEquals(model.CaseOwners, items).Should().BeTrue();
		}

		[Test]
		public void SetCaseOwners_VeryLargeList_CaseOwners_ShouldContainAllItems()
		{
			var model = new CaseFilters();
			var fixture = new Fixture();
			var largeList = fixture.CreateMany<string>(10000).ToList();

			model.SetCaseOwners(largeList);

			model.CaseOwners.Should().BeEquivalentTo(largeList);
			ReferenceEquals(model.CaseOwners, largeList).Should().BeTrue();
		}
	}
}