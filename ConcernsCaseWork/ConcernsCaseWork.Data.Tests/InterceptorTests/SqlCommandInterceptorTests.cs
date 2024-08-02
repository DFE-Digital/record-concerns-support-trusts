using ConcernsCaseWork.Data.EFInterceptors;
using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System.Data.Common;

namespace ConcernsCaseWork.Data.Tests.InterceptorTests;

[TestFixture]
public class SqlCommandInterceptorTests
{
	private const string _envVarName = "ENABLE_DETAILED_SQL_LOGGING";
	private DatabaseTestFixture _fixture;
	private ConcernsDbContext _context;
	private Mock<SqlCommandInterceptor> _sqlCommandInterceptorMock;

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		// This ensures that the environment is consistent for the entire test class
		_sqlCommandInterceptorMock = new Mock<SqlCommandInterceptor> { CallBase = true };
		_fixture = new DatabaseTestFixture([_sqlCommandInterceptorMock.Object]);
		Environment.SetEnvironmentVariable(_envVarName, null);

	}

	[SetUp]
	public void SetUp()
	{
		_context = _fixture.CreateContext([_sqlCommandInterceptorMock.Object]);
		Environment.SetEnvironmentVariable(_envVarName, null);
	}

	[TearDown]
	public void TearDown()
	{
		Environment.SetEnvironmentVariable(_envVarName, null);
	}

	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
		// Clean up environment after all tests have run
		Environment.SetEnvironmentVariable(_envVarName, null);
	}

	[Test]
	public void Should_AddSqlCommandInterceptor_When_EnableDetailedSqlLoggingIsTrue()
	{
		// Arrange
		Environment.SetEnvironmentVariable(_envVarName, "true");

		// Act
		_context.Set<ConcernsCase>().AsNoTracking().FirstOrDefault();

		// Assert
		_sqlCommandInterceptorMock.Verify(
			m => m.ReaderExecuting(It.IsAny<DbCommand>(), It.IsAny<CommandEventData>(), It.IsAny<InterceptionResult<DbDataReader>>()),
			Times.AtLeastOnce,
			"The SQL Command Interceptor should be called when ENABLE_DETAILED_SQL_LOGGING is set to true."
		);
	}

	[Test]
	public void Should_AddSqlCommandInterceptor_When_EnableDetailedSqlLoggingIsFalse()
	{
		// Arrange
		Environment.SetEnvironmentVariable(_envVarName, "false");

		// Act
		_context.Set<ConcernsCase>().AsNoTracking().FirstOrDefault();

		// Assert
		_sqlCommandInterceptorMock.Verify(
			m => m.ReaderExecuting(It.IsAny<DbCommand>(), It.IsAny<CommandEventData>(), It.IsAny<InterceptionResult<DbDataReader>>()),
			Times.Never,
			"The SQL Command Interceptor should never be called when ENABLE_DETAILED_SQL_LOGGING is set to false."
		);
	}
}

