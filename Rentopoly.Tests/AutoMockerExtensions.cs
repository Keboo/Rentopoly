using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Moq.AutoMock.Resolvers;

using Rentopoly.Data;

namespace Rentopoly.Tests;

public static class AutoMockerExtensions
{
    public static IDbScope<BoardGameContext> WithDbScope(this AutoMocker mocker)
    {
        var resolver = new DbScopedResolver();
        var existing = mocker.Resolvers.ToList();
        mocker.Resolvers.Clear();
        existing.Insert(0, resolver);
        existing.Add(resolver);
        foreach (var existingResolver in existing)
        {
            mocker.Resolvers.Add(existingResolver);
        }
        return resolver;
    }

    public interface IDbScope<TContext> : IDbContextFactory<TContext>, IDisposable
        where TContext : DbContext
    { }

    private sealed class DbScopedResolver : IMockResolver, IDbScope<BoardGameContext>
    {
        private bool _disposedValue;

        private readonly Lazy<SqliteConnection> _sqliteConnection = new(() =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        });

        public void Resolve(MockResolutionContext context)
        {
            if (context.RequestType == typeof(BoardGameContext))
                context.Value = CreateDbContext();
            else if (context.RequestType == typeof(Func<BoardGameContext>))
            {
                context.Value = new Func<BoardGameContext>(CreateDbContext);
            }
        }

        public BoardGameContext CreateDbContext()
        {
            var builder = new DbContextOptionsBuilder<BoardGameContext>()
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging()
                        .UseSqlite(_sqliteConnection.Value);

            var dbContext = new BoardGameContext(builder.Options);

            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
