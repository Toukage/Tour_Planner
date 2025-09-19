using DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public static class TestDBContext
    {
        public static DbContextOptions<DatabaseManager> NewDB()
        {
            return new DbContextOptionsBuilder<DatabaseManager>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }
    }
    public class TestDbContextFactory : IDbContextFactory<DatabaseManager>
    {
        private readonly DbContextOptions<DatabaseManager> _options;
        public TestDbContextFactory(DbContextOptions<DatabaseManager> options) => _options = options;
        public DatabaseManager CreateDbContext() => new DatabaseManager(_options);
    }

}
