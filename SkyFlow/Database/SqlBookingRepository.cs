using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using SkyFlow.Models;

namespace SkyFlow.Database
{
    public class SqlBookingRepository
    {
        public List<Booking> GetAll()
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            return connection.Query<Booking>(
                @"SELECT
                    b.BookingID,
                    b.FlightID,
                    b.PassengerID,
                    b.SeatNumber,
                    b.Status,
                    b.BookingDate,
                    p.FullName AS PassengerName,
                    f.FlightNumber
                  FROM Bookings b
                  INNER JOIN Passengers p
                    ON b.PassengerID = p.PassengerID
                  INNER JOIN Flights f
                    ON b.FlightID = f.FlightID
                  ORDER BY b.BookingID"
            ).ToList();
        }

        public Booking? GetById(int id)
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            return connection.QueryFirstOrDefault<Booking>(
                @"SELECT
                    b.BookingID,
                    b.FlightID,
                    b.PassengerID,
                    b.SeatNumber,
                    b.Status,
                    b.BookingDate,
                    p.FullName AS PassengerName,
                    f.FlightNumber
                  FROM Bookings b
                  INNER JOIN Passengers p
                    ON b.PassengerID = p.PassengerID
                  INNER JOIN Flights f
                    ON b.FlightID = f.FlightID
                  WHERE b.BookingID = @BookingID",
                new
                {
                    BookingID = id
                }
            );
        }

        public List<Booking> GetBookingsByFlight(
            int flightId)
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            return connection.Query<Booking>(
                @"SELECT
                    b.BookingID,
                    b.FlightID,
                    b.PassengerID,
                    b.SeatNumber,
                    b.Status,
                    b.BookingDate,
                    p.FullName AS PassengerName,
                    f.FlightNumber
                  FROM Bookings b
                  INNER JOIN Passengers p
                    ON b.PassengerID = p.PassengerID
                  INNER JOIN Flights f
                    ON b.FlightID = f.FlightID
                  WHERE b.FlightID = @FlightID
                  ORDER BY b.SeatNumber",
                new
                {
                    FlightID = flightId
                }
            ).ToList();
        }

        public List<Booking> GetBookingsByPassenger(
            int passengerId)
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            return connection.Query<Booking>(
                @"SELECT
                    b.BookingID,
                    b.FlightID,
                    b.PassengerID,
                    b.SeatNumber,
                    b.Status,
                    b.BookingDate,
                    p.FullName AS PassengerName,
                    f.FlightNumber
                  FROM Bookings b
                  INNER JOIN Passengers p
                    ON b.PassengerID = p.PassengerID
                  INNER JOIN Flights f
                    ON b.FlightID = f.FlightID
                  WHERE b.PassengerID = @PassengerID
                  ORDER BY b.BookingDate DESC",
                new
                {
                    PassengerID = passengerId
                }
            ).ToList();
        }

        public void Add(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(
                    nameof(booking)
                );
            }

            ValidateBooking(booking);

            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            int flightExists =
                connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*)
                      FROM Flights
                      WHERE FlightID = @FlightID",
                    new
                    {
                        booking.FlightID
                    }
                );

            if (flightExists == 0)
            {
                throw new InvalidOperationException(
                    "The selected flight does not exist."
                );
            }

            int passengerExists =
                connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*)
                      FROM Passengers
                      WHERE PassengerID = @PassengerID",
                    new
                    {
                        booking.PassengerID
                    }
                );

            if (passengerExists == 0)
            {
                throw new InvalidOperationException(
                    "The selected passenger does not exist."
                );
            }

            int seatExists =
                connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*)
                      FROM Bookings
                      WHERE FlightID = @FlightID
                        AND UPPER(SeatNumber) =
                            UPPER(@SeatNumber)",
                    new
                    {
                        booking.FlightID,
                        SeatNumber =
                            booking.SeatNumber.Trim()
                    }
                );

            if (seatExists > 0)
            {
                throw new InvalidOperationException(
                    "That seat is already assigned on this flight."
                );
            }

            int newId =
                connection.QuerySingle<int>(
                    @"INSERT INTO Bookings
                        (
                            FlightID,
                            PassengerID,
                            SeatNumber,
                            Status,
                            BookingDate
                        )
                      OUTPUT INSERTED.BookingID
                      VALUES
                        (
                            @FlightID,
                            @PassengerID,
                            @SeatNumber,
                            @Status,
                            @BookingDate
                        )",
                    new
                    {
                        booking.FlightID,
                        booking.PassengerID,

                        SeatNumber =
                            booking.SeatNumber
                                .Trim()
                                .ToUpper(),

                        booking.Status,
                        booking.BookingDate
                    }
                );

            booking.BookingID = newId;
        }

        public void Update(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(
                    nameof(booking)
                );
            }

            ValidateBooking(booking);

            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            int duplicateSeat =
                connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*)
                      FROM Bookings
                      WHERE FlightID = @FlightID
                        AND UPPER(SeatNumber) =
                            UPPER(@SeatNumber)
                        AND BookingID <> @BookingID",
                    new
                    {
                        booking.FlightID,

                        SeatNumber =
                            booking.SeatNumber.Trim(),

                        booking.BookingID
                    }
                );

            if (duplicateSeat > 0)
            {
                throw new InvalidOperationException(
                    "That seat is already assigned on this flight."
                );
            }

            int affectedRows =
                connection.Execute(
                    @"UPDATE Bookings
                      SET
                        FlightID = @FlightID,
                        PassengerID = @PassengerID,
                        SeatNumber = @SeatNumber,
                        Status = @Status,
                        BookingDate = @BookingDate
                      WHERE BookingID = @BookingID",
                    new
                    {
                        booking.BookingID,
                        booking.FlightID,
                        booking.PassengerID,

                        SeatNumber =
                            booking.SeatNumber
                                .Trim()
                                .ToUpper(),

                        booking.Status,
                        booking.BookingDate
                    }
                );

            if (affectedRows == 0)
            {
                throw new InvalidOperationException(
                    "Booking was not found."
                );
            }
        }

        public void UpdateBookingStatus(
            int bookingId,
            string status)
        {
            if (!IsValidStatus(status))
            {
                throw new ArgumentException(
                    "Invalid booking status."
                );
            }

            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            int affectedRows =
                connection.Execute(
                    @"UPDATE Bookings
                      SET Status = @Status
                      WHERE BookingID = @BookingID",
                    new
                    {
                        BookingID = bookingId,
                        Status = NormalizeStatus(status)
                    }
                );

            if (affectedRows == 0)
            {
                throw new InvalidOperationException(
                    "Booking was not found."
                );
            }
        }

        public void Delete(int id)
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            connection.Execute(
                @"DELETE FROM Bookings
                  WHERE BookingID = @BookingID",
                new
                {
                    BookingID = id
                }
            );
        }

        private static void ValidateBooking(
            Booking booking)
        {
            if (booking.FlightID <= 0)
            {
                throw new ArgumentException(
                    "A valid flight is required."
                );
            }

            if (booking.PassengerID <= 0)
            {
                throw new ArgumentException(
                    "A valid passenger is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    booking.SeatNumber))
            {
                throw new ArgumentException(
                    "Seat number is required."
                );
            }

            if (!IsValidStatus(booking.Status))
            {
                throw new ArgumentException(
                    "Invalid booking status."
                );
            }

            if (booking.BookingDate == default)
            {
                booking.BookingDate =
                    DateTime.Now;
            }

            booking.Status =
                NormalizeStatus(
                    booking.Status
                );
        }

        private static bool IsValidStatus(
            string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;

            return
                status.Equals(
                    "Confirmed",
                    StringComparison.OrdinalIgnoreCase) ||

                status.Equals(
                    "CheckedIn",
                    StringComparison.OrdinalIgnoreCase) ||

                status.Equals(
                    "Boarded",
                    StringComparison.OrdinalIgnoreCase) ||

                status.Equals(
                    "Cancelled",
                    StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeStatus(
            string status)
        {
            if (status.Equals(
                    "Confirmed",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Confirmed";
            }

            if (status.Equals(
                    "CheckedIn",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "CheckedIn";
            }

            if (status.Equals(
                    "Boarded",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Boarded";
            }

            return "Cancelled";
        }
    }
}