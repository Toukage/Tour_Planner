using BusinessLayer;
using TourPlanner.Model;
using System.IO;
using BusinessLayer.Interfaces;

namespace Tests
{
    internal class ReportTests
    {
        private IReport _report;

        [SetUp]
        public void Setup()
        {
            _report = new Report();
        }


        [Test]
        public async Task ReportAsync_ShouldCreatePdfReport()//testet ob ein report erstellt wird
        {
            var tour = new Tour
            {
                TourName = "ReportTour",
                TourStart = "A",
                TourEnd = "B",
                TourDescription = "this is a test tour",
                Transport = "Car",
                Distance = 12.5f,
                EstTime = 60f
            };

            var log = new TourLog
            {
                TourID = 1,
                LogDate = DateTime.UtcNow,
                LogComment = "ReportLog",
                LogDifficulty = 2,
                LogDistance = 50,
                LogTime = 30,
                Rating = 4
            };

            string tempPath = Path.GetTempFileName() + ".pdf";
            await _report.ReportAsync(tour, new List<TourLog> { log }, null, tempPath);

            Assert.That(File.Exists(tempPath), Is.True);

            File.Delete(tempPath);
        }

        [Test]
        public void ReportNullTourTest()//testet ob eine exception geworfen wird wenn tour null ist
        {
            string tempPath = Path.GetTempFileName() + ".pdf";
            Assert.ThrowsAsync<ReportException>(async () =>
                await _report.ReportAsync(null, new List<TourLog>(), null, tempPath)
            );
            File.Delete(tempPath);
        }

        [Test]
        public void ReportInvalidPathTest()//testet ob eine exception geworfen wird wenn der pfad ungültig ist
        {
            var tour = new Tour { TourName = "Tour", TourStart = "A", TourEnd = "B" };
            var logs = new List<TourLog>();
            string badPath = @"G:\definitely\not\a\real\path\report.pdf";
            Assert.ThrowsAsync<ReportException>(async () =>
                await _report.ReportAsync(tour, logs, null, badPath)
            );
        }

        [Test]
        public async Task ReportNoLogListTest()//testet ob ein report erstellt wird wenn keine logs vorhanden sind
        {
            var tour = new Tour
            {
                TourName = "EmptyLogTour",
                TourStart = "A",
                TourEnd = "B",
                TourDescription = "no logs test"
            };
            string tempPath = Path.GetTempFileName() + ".pdf";
            await _report.ReportAsync(tour, new List<TourLog>(), null, tempPath);

            Assert.That(File.Exists(tempPath), Is.True);
            File.Delete(tempPath);
        }

        [Test]
        public async Task ReportNoMapTest()//testet ob ein report erstellt wird wenn kein mapPng vorhanden ist
        {
            var tour = new Tour { TourName = "NoMapTour", TourStart = "A", TourEnd = "B" };
            var log = new TourLog
            {
                TourID = 1,
                LogDate = DateTime.UtcNow,
                LogComment = "Log",
                LogDifficulty = 3,
                LogDistance = 10,
                LogTime = 20,
                Rating = 5
            };
            string tempPath = Path.GetTempFileName() + ".pdf";
            await _report.ReportAsync(tour, new List<TourLog> { log }, null, tempPath);

            Assert.That(File.Exists(tempPath), Is.True);
            File.Delete(tempPath);
        }
    }
}
