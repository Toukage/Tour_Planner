using DataAccessLayer;
using TourPlanner.Model;

namespace Tests
{
    internal class LogRepoTests
    {
        private TestDbContextFactory _factory;
        private ILogRepo _logRepo;

        [SetUp]
        public void Setup()
        {
            var options = TestDBContext.NewDB();
            _factory = new TestDbContextFactory(options);
            _logRepo = new LogRepo(_factory);
        }

        [Test]
        public async Task InsertLogTest()//erstellt neues log objekt und fügt es der db hinzu
        {
            //Arrange
            var logEntity = new TourLog
            {
                TourID = 1,
                LogDate = System.DateTime.UtcNow,
                LogDifficulty = 3,
                LogDistance = 10.0f,
                LogTime = 60.0f,
                Rating = 4
            };

            //Act
            var saved = await _logRepo.InsertLogAsync(logEntity, CancellationToken.None);
            TestContext.Out.WriteLine($"Inserted log with ID: {saved.LogID} for TourID: {saved.TourID}");

            //Assert
            Assert.That(saved.LogID, Is.GreaterThan(0));
            Assert.That(saved.TourID, Is.EqualTo(1));
        }


        [Test]
        public async Task EditLogTest()//updated ein log in der db
        {
            //Arrange
            var logEntity = new TourLog
            {
                TourID = 1,
                LogDate = System.DateTime.UtcNow,
                LogDifficulty = 3,
                LogDistance = 10.0f,
                LogTime = 60.0f,
                Rating = 4
            };
            await _logRepo.InsertLogAsync(logEntity);
            logEntity.LogDifficulty = 2;

            //Act
            await _logRepo.EditLogAsync(logEntity);
            var logs = await _logRepo.GetLogsAsync(1, CancellationToken.None);

            //Assert
            Assert.That(logs.Exists(l => l.LogDifficulty == 2), Is.True);
        }

        [Test]
        public async Task DropLogTest()
        {
            //Arrange
            var logEntity = new TourLog
            {
                TourID = 2,
                LogDate = System.DateTime.UtcNow,
                LogDifficulty = 2,
                LogDistance = 5.0f,
                LogTime = 30.0f,
                Rating = 3
            };
            var saved = await _logRepo.InsertLogAsync(logEntity, CancellationToken.None);

            //Act
            await _logRepo.DropLogAsync(saved, CancellationToken.None);
            var logs = await _logRepo.GetLogsAsync(2, CancellationToken.None);
            TestContext.Out.WriteLine($"Logs after delete: {logs.Count}");

            //Assert
            Assert.That(logs, Is.Empty);
        }

        [Test]
        public void InsertNullLogTest()
        {
            Assert.ThrowsAsync<LogRepoException>(async () =>
                await _logRepo.InsertLogAsync(null, CancellationToken.None));
        }

        [Test]
        public void EditNullLogTest()
        {
            Assert.ThrowsAsync<LogRepoException>(async () =>
                await _logRepo.EditLogAsync(null, CancellationToken.None));
        }

        [Test]
        public void DropNullLogTest()
        {
            Assert.ThrowsAsync<LogRepoException>(async () =>
                await _logRepo.DropLogAsync(null, CancellationToken.None));
        }
    }
}
