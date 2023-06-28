using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Service.Base;
using FluentAssertions;
using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Mappers
{
	public class PaginationMappingTests
	{
		[Test]
		public void ToModel_When_HasNextAndPrevious_Returns_Model()
		{
			var paginationResponse = new Pagination();
			paginationResponse.Page = 2;
			paginationResponse.HasPrevious = true;
			paginationResponse.HasNext = true;
			paginationResponse.TotalPages = 5;
			paginationResponse.RecordCount = 50;

			var result = PaginationMapping.ToModel(paginationResponse);

			result.Url.Should().BeEmpty();
			result.PageNumber.Should().Be(2);
			result.TotalPages.Should().Be(5);
			result.Next.Should().Be(3);
			result.Previous.Should().Be(1);
			result.RecordCount.Should().Be(50);
		}

		[Test]
		public void ToModel_When_HasOnlyOnePage_Returns_Model()
		{
			var paginationResponse = new Pagination();
			paginationResponse.Page = 1;
			paginationResponse.HasPrevious = false;
			paginationResponse.HasNext = false;
			paginationResponse.TotalPages = 1;

			var result = PaginationMapping.ToModel(paginationResponse);

			result.PageNumber.Should().Be(1);
			result.TotalPages.Should().Be(1);
			result.Next.Should().BeNull();
			result.Previous.Should().BeNull();
		}
	}
}
