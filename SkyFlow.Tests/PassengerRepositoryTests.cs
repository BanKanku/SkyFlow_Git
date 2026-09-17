using SkyFlow.Database;
using Xunit;

namespace SkyFlow.Tests
{
    public class PassengerRepositoryTests
    {
        [Fact]
        public void GetAll_ReturnsPassengers()
        {
            PassengerRepository repository = new PassengerRepository();

            var passengers = repository.GetAll();

            Assert.NotNull(passengers);
            Assert.NotEmpty(passengers);
        }

        [Fact]
        public void GetById_WithValidId_ReturnsPassenger()
        {
            PassengerRepository repository = new PassengerRepository();

            var passenger = repository.GetById(1);

            Assert.NotNull(passenger);
            Assert.Equal(1, passenger.PassengerID);
            Assert.Equal("Alex Morgan", passenger.FullName);
        }

        [Fact]
        public void GetById_WithInvalidId_ReturnsNull()
        {
            PassengerRepository repository = new PassengerRepository();

            var passenger = repository.GetById(99999);

            Assert.Null(passenger);
        }

        [Fact]
        public void GetByPassport_WithValidPassport_ReturnsPassenger()
        {
            PassengerRepository repository = new PassengerRepository();

            var passenger = repository.GetByPassport("DEMO10001");

            Assert.NotNull(passenger);
            Assert.Equal("Alex Morgan", passenger.FullName);
        }

        [Fact]
        public void GetByPassport_IsCaseInsensitive()
        {
            PassengerRepository repository = new PassengerRepository();

            var passenger = repository.GetByPassport("demo10001");

            Assert.NotNull(passenger);
            Assert.Equal("DEMO10001", passenger.PassportNumber);
        }
    }
}