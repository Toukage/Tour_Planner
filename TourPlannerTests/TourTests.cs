using NUnit.Framework;
using TourPlanner.ViewModel;
using TourPlanner.Model;

namespace Tests
{
    public class TourTests
    {
        private CreateTourViewModel _createViewModel;
        private DeleteTourViewModel _deleteViewModel;
        private ModifyTourViewModel _modifyViewModel;

        [SetUp]
        public void Setup()
        {
            _createViewModel = new CreateTourViewModel();
            _deleteViewModel = new DeleteTourViewModel();
            _modifyViewModel = new ModifyTourViewModel();
        }

        [Test]
        public void CreateTourCommand_ShouldNotBeNull()
        {
            var viewModel = new CreateTourViewModel();
            Assert.That(viewModel.CreateTourCommand, Is.Not.Null);
        }


        [Test]
        public void CreateTourCommand_ShouldExecuteWithoutException()
        {
            _createViewModel.Tour = new Tour
            {
                TourName = "Test Tour",
                Description = "Just a test",
                TourStart = "Start Point",
                TourEnd = "End Point",
                Transport = "Car"
            };

            Assert.DoesNotThrow(() => _createViewModel.CreateTourCommand.Execute(null));
        }

        [Test]
        public void CreateTourCommand_CanExecute_ReturnsTrue()
        {
            var createViewModel = new CreateTourViewModel();
            Assert.That(createViewModel.CreateTourCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void DeleteTourCommand_ShouldExecuteWithoutException()
        {
            _deleteViewModel.Tour = new Tour
            {
                TourName = "Test Tour To Delete",
                Description = "To delete",
                TourStart = "Start",
                TourEnd = "End",
                Transport = "Bike"
            };

            Assert.DoesNotThrow(() => _deleteViewModel.DeleteTourCommand.Execute(null));
        }
    }

}
