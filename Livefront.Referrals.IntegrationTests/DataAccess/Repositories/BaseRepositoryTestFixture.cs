using System.Data.Common;
using Livefront.Referrals.DataAccess;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


namespace Livefront.Referrals.IntegrationTests.DataAccess.Repositories;

public class BaseRepositoryTestFixture
{
    protected CancellationToken cancellationToken = CancellationToken.None;
    protected ReferralsContext dbContext;
    protected DbConnection dbConnection;
    
    protected string GetSqliteConnectionString(string databaseName)
    {
        var builder = new SqliteConnectionStringBuilder()
        {
            DataSource = databaseName,
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Private
        };
        
        return builder.ConnectionString;
    }

    protected async Task ConfigureDbContextAsync()
    {
        dbConnection = new SqliteConnection(GetSqliteConnectionString("referrals"));
        await dbConnection.OpenAsync(cancellationToken);
        var dbContextOptions = new DbContextOptionsBuilder<ReferralsContext>()
            .UseSqlite(dbConnection)
            .Options;
        dbContext = new ReferralsContext(dbContextOptions);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }

    protected void CleanUp()
    {
        dbContext.Dispose();
        dbConnection.Close();
        dbConnection.Dispose();
    }
}