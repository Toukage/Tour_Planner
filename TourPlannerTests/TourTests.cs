using NUnit.Framework;
using TourPlanner.ViewModel;
using TourPlanner.Model;

namespace Tests
{
    public class TourTests
    {
        private TourViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new TourViewModel();
        }

        [Test]
        public void CreateTourCommand_ShouldNotBeNull()
        {
            var viewModel = new TourViewModel();
            Assert.That(viewModel.CreateTourCommand, Is.Not.Null);
        }


        [Test]
        public void CreateTourCommand_ShouldExecuteWithoutException()
        {
            _viewModel.Tour = new Tour
            {
                TourName = "Test Tour",
                Description = "Just a test",
                TourStart = "Start Point",
                TourEnd = "End Point",
                Transport = "Car"
            };

            Assert.DoesNotThrow(() => _viewModel.CreateTourCommand.Execute(null));
        }

        [Test]
        public void CreateTourCommand_CanExecute_ReturnsTrue()
        {
            var viewModel = new TourViewModel();
            Assert.That(viewModel.CreateTourCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void DeleteTourCommand_ShouldExecuteWithoutException()
        {
            _viewModel.Tour = new Tour
            {
                TourName = "Test Tour To Delete",
                Description = "To delete",
                TourStart = "Start",
                TourEnd = "End",
                Transport = "Bike"
            };

            Assert.DoesNotThrow(() => _viewModel.DeleteTourCommand.Execute(null));
        }
    }

}
