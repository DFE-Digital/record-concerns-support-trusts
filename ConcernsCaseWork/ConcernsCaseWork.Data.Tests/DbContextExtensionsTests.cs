using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests
{
	[TestFixture]
    public class DbContextOptionsBuilderExtensionsTests
    {
        private DatabaseTestFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new DatabaseTestFixture();
        }

        [Test]
        public void UseConcernsSqlServer_ShouldConfigureSqlServerWithoutRetry()
        {
            // Arrange  
            var builder = new DbContextOptionsBuilder();
            var connectionString = _fixture.CreateContext().Database.GetDbConnection().ConnectionString;

            // Act  
            builder.UseConcernsSqlServer(connectionString, enableRetryOnFailure: false);

            // Assert  
            var options = builder.Options;
			AssertExtensions(options, false);
		}

        [Test]
        public void UseConcernsSqlServer_ShouldConfigureSqlServerWithRetry()
        {
			// Arrange  
			var builder = new DbContextOptionsBuilder();
			var connectionString = _fixture.CreateContext().Database.GetDbConnection().ConnectionString;

			// Act  
			builder.UseConcernsSqlServer(connectionString, enableRetryOnFailure: true);

			// Assert  
			var options = builder.Options;
			AssertExtensions(options, true);
		}

		private static void AssertExtensions(DbContextOptions options, bool expectedEnableRetryOnFailure)
		{
			options.Should().NotBeNull();
			var sqlServerOptionExtension = options.Extensions.SingleOrDefault(e => e.GetType().Name == "SqlServerOptionsExtension");
			sqlServerOptionExtension.Should().NotBeNull();
			sqlServerOptionExtension!.GetType().GetProperty("MigrationsHistoryTable")?.GetValue(sqlServerOptionExtension).Should().Be("__EfMigrationsHistory");
			sqlServerOptionExtension.GetType().GetProperty("EnableRetryOnFailure")?.GetValue(sqlServerOptionExtension).Should().Be(expectedEnableRetryOnFailure);
		} 
	}
}
