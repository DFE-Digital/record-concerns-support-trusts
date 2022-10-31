using ConcernsCaseWork.Data;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConcernsCaseWork.API.Tests
{
    public class DbFixture : IDisposable
    {
        private readonly ConcernsDbContext _concernsDbContext;
        private readonly IDbContextTransaction _concernsTransaction;
        public readonly string ConnString;
        
        
        public DbFixture()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "integration_settings.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile(configPath)
                .AddUserSecrets(typeof(DbFixture).Assembly)
                .AddEnvironmentVariables()
                .Build();

            ConnString = config.GetConnectionString("DefaultConnection");

            var tramsContextBuilder = new DbContextOptionsBuilder<ConcernsDbContext>();

            tramsContextBuilder.UseSqlServer(ConnString);
            _concernsDbContext = new ConcernsDbContext(tramsContextBuilder.Options);
            
            _concernsDbContext.Database.EnsureCreated();
            _concernsDbContext.Database.Migrate();
            
            _concernsTransaction = _concernsDbContext.Database.BeginTransaction();
        }

        public void Dispose()
        {
            _concernsTransaction.Rollback();
            _concernsTransaction.Dispose();
            GC.SuppressFinalize(this);
        }
    }
    
    [CollectionDefinition("Database", DisableParallelization = true)]
    public class DatabaseCollection : ICollectionFixture<DbFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
