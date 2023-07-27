using CommunityToolkit.Mvvm.Messaging;

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

        public DbScopedResolver()
        {
            FilePath = Path.Combine(
                Path.GetTempPath(),
                "RentopolyTests",
                Guid.NewGuid().ToString("N")
                );
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

            using var context = CreateDbContext();
            context.Database.Migrate();
        }

        private string FilePath { get; }

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
            var connectionString = new SqliteConnectionStringBuilder
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                DataSource = FilePath,
                Pooling = false
            };
            var options = new DbContextOptionsBuilder<BoardGameContext>().UseSqlite(connectionString.ToString()).Options;
            return new BoardGameContext(options);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    File.Delete(FilePath);
                }
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
