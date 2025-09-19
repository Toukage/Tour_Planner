using DataAccessLayer;
using DataAccessLayer.Interfaces;
using TourPlanner.Model;

namespace Tests
{
   
   
    [TestFixture]
    public class TourRepoTests
    {
        private TestDbContextFactory _factory;
        private ITourRepo _tourRepo;

        [SetUp]
        public void Setup()
        {
            var options = TestDBContext.NewDB();
            _factory = new TestDbContextFactory(options);
            _tourRepo = new TourRepo(_factory);
        }

        [Test]
        public async Task InsertTourTest()
        { 
            //Arrange
            var tour = new Tour
            {
                TourName = "InsertTestTour",
                TourDescription = "TestDesc",
                TourStart = "Wien",
                TourEnd = "Linz",
                Transport = "Car"
            };
            //Act
            await _tourRepo.InsertTourAsync(tour);
            var tours = await _tourRepo.GetAllToursAsync();
            TestContext.Out.WriteLine($"Tour count after insert: {tours.Count}");
            TestContext.Out.WriteLine($"Tour names: {string.Join(", ", tours.Select(t => t.TourName))}");

            //Assert
            Assert.That(tours.Exists(t => t.TourName == "InsertTestTour"), Is.True);

        }

        [Test]
        public async Task EditTourTest()//ändert die tourbeschreibung einer tour
        {
            //Arrange
            var tour = new Tour
            {
                TourName = "EditTestTour",
                TourDescription = "OldDesc",
                TourStart = "A",
                TourEnd = "B",
                Transport = "Car"
            };
            await _tourRepo.InsertTourAsync(tour);
            tour.TourDescription = "NewDesc";

            //Act
            await _tourRepo.EditTourAsync(tour);
            var tours = await _tourRepo.GetAllToursAsync();

            //Assert
            Assert.That(tours.Exists(t => t.TourDescription == "NewDesc"), Is.True);
        }

        [Test]
        public async Task DropTourTest()//löscht eine tour aus der db
        {
            //Arrange
            var tour = new Tour
            {
                TourName = "DropTestTour",
                TourDescription = "Desc",
                TourStart = "Start",
                TourEnd = "End",
                Transport = "Bike"
            };
            await _tourRepo.InsertTourAsync(tour);

            //Act
            await _tourRepo.DropTourAsync(tour.TourID);
            var tours = await _tourRepo.GetAllToursAsync();
            TestContext.Out.WriteLine($"Tours after delete: {tours.Count}");

            //Assert
            Assert.That(tours.Exists(t => t.TourName == "DropTestTour"), Is.False);
        }

        [Test]
        public void InsertNullTourTest() //versucht eine null tour hinzuzufügen
        {
            Assert.ThrowsAsync<TourRepoException>(async () =>
                await _tourRepo.InsertTourAsync(null));
        }

        [Test]
        public void DropNullTourTest() //versucht eine tour mit ungültiger id zu löschen
        {
            Assert.ThrowsAsync<TourRepoException>(async () =>
                await _tourRepo.DropTourAsync(-1));
        }

        [Test]
        public void EditNullTourTest() //versucht eine null tour zu editieren
        {
            Assert.ThrowsAsync<TourRepoException>(async () =>
                await _tourRepo.EditTourAsync(null));
        }
    }
}
