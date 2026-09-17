using SkyFlow.Database;
using SkyFlow.Models;
using Xunit;

namespace SkyFlow.Tests
{
    public class BookingRepositoryTests
    {
        [Fact]
        public void GetAll_ReturnsBookings()
        {
            BookingRepository repository = new BookingRepository();

            var bookings = repository.GetAll();

            Assert.NotNull(bookings);
            Assert.NotEmpty(bookings);
        }

        [Fact]
        public void GetById_WithValidId_ReturnsBooking()
        {
            BookingRepository repository = new BookingRepository();

            var booking = repository.GetById(1);

            Assert.NotNull(booking);
            Assert.Equal(1, booking.BookingID);
            Assert.Equal("12A", booking.SeatNumber);
        }

        [Fact]
        public void GetById_WithInvalidId_ReturnsNull()
        {
            BookingRepository repository = new BookingRepository();

            var booking = repository.GetById(99999);

            Assert.Null(booking);
        }

        [Fact]
        public void Add_NewBooking_AssignsIdAndStoresBooking()
        {
            BookingRepository repository = new BookingRepository();

            Booking booking = new Booking
            {
                FlightID = 1,
                PassengerID = 3,
                SeatNumber = "20C",
                Status = "Confirmed",
                BookingDate = DateTime.Now
            };

            repository.Add(booking);

            Assert.True(booking.BookingID > 0);

            var storedBooking = repository.GetById(booking.BookingID);

            Assert.NotNull(storedBooking);
            Assert.Equal("20C", storedBooking.SeatNumber);
            Assert.Equal("Confirmed", storedBooking.Status);
        }
    }
}