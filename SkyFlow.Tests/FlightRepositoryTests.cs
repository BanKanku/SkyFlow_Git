using System;
using SkyFlow.Database;
using SkyFlow.Models;
using Xunit;

namespace SkyFlow.Tests
{
    public class FlightRepositoryTests
    {
        [Fact]
        public void GetAll_ReturnsFlights()
        {
            FlightRepository repository = new FlightRepository();

            var flights = repository.GetAll();

            Assert.NotNull(flights);
            Assert.NotEmpty(flights);
        }

        [Fact]
        public void GetById_WithValidId_ReturnsFlight()
        {
            FlightRepository repository = new FlightRepository();

            var flight = repository.GetById(1);

            Assert.NotNull(flight);
            Assert.Equal(1, flight.FlightID);
            Assert.Equal("SF102", flight.FlightNumber);
        }

        [Fact]
        public void GetById_WithInvalidId_ReturnsNull()
        {
            FlightRepository repository = new FlightRepository();

            var flight = repository.GetById(99999);

            Assert.Null(flight);
        }

        [Fact]
        public void Add_NewFlight_AssignsIdAndStoresFlight()
        {
            FlightRepository repository = new FlightRepository();

            Flight flight = new Flight
            {
                FlightNumber = "TEST900",
                Origin = "JHB",
                Destination = "CPT",
                DepartureTime = DateTime.Now.AddDays(1),
                AircraftCapacity = 100,
                AvailableSeats = 100,
                Status = "Scheduled"
            };

            repository.Add(flight);

            Assert.True(flight.FlightID > 0);

            var storedFlight = repository.GetById(flight.FlightID);

            Assert.NotNull(storedFlight);
            Assert.Equal("TEST900", storedFlight.FlightNumber);
            Assert.Equal("JHB", storedFlight.Origin);
            Assert.Equal("CPT", storedFlight.Destination);
        }
    }
}