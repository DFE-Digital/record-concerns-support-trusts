using ConcernsCaseWork.Data.Auditing;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests.Tests
{
	[TestFixture]
	public class EntityStateToAuditChangeTypeMapTests
	{
		[Test]
		public void Map_When_EntityState_Added_Should_Return_INSERT()
		{
			var result = EntityStateToAuditChangeTypeMap.Map(EntityState.Added);
			result.Should().Be(Models.AuditChangeType.INSERT);
		}

		[Test]
		public void Map_When_EntityState_Modified_Should_Return_UPDATE()
		{
			var result = EntityStateToAuditChangeTypeMap.Map(EntityState.Modified);
			result.Should().Be(Models.AuditChangeType.UPDATE);
		}

		[Test]
		public void Map_When_EntityState_Deleted_Should_Return_DELETE()
		{
			var result = EntityStateToAuditChangeTypeMap.Map(EntityState.Deleted);
			result.Should().Be(Models.AuditChangeType.DELETE);
		}

		[TestCase(EntityState.Detached)]
		[TestCase(EntityState.Unchanged)]
		public void Map_When_EntityState_Not_Supported_Should_Throw_NotSupportedException(EntityState entityState)
		{
			Assert.Throws<NotSupportedException>(() => EntityStateToAuditChangeTypeMap.Map(entityState));
		}

		[TestCase(EntityState.Added)]
		[TestCase(EntityState.Modified)]
		[TestCase(EntityState.Deleted)]

		public void IsSupported_When_Supported_Entity_State_Returns_True(EntityState entityState)
		{
			var result = EntityStateToAuditChangeTypeMap.IsSupported(entityState);
			result.Should().BeTrue();
		}

		[TestCase(EntityState.Detached)]
		[TestCase(EntityState.Unchanged)]

		public void IsSupported_When_Unsupported_Entity_State_Returns_False(EntityState entityState)
		{
			var result = EntityStateToAuditChangeTypeMap.IsSupported(entityState);
			result.Should().BeFalse();
		}
	}
}
