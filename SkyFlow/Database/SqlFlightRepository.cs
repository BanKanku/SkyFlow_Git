using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using SkyFlow.Models;

namespace SkyFlow.Database
{
    public class SqlFlightRepository
    {
        public List<Flight> GetAll()
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            return connection.Query<Flight>(
                @"SELECT
                    FlightID,
                    FlightNumber,
                    Origin,
                    Destination,
                    DepartureTime,
                    AircraftCapacity,
                    AvailableSeats,
                    Status
                  FROM Flights
                  ORDER BY DepartureTime"
            ).ToList();
        }

        public Flight? GetById(int id)
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            return connection.QueryFirstOrDefault<Flight>(
                @"SELECT
                    FlightID,
                    FlightNumber,
                    Origin,
                    Destination,
                    DepartureTime,
                    AircraftCapacity,
                    AvailableSeats,
                    Status
                  FROM Flights
                  WHERE FlightID = @FlightID",
                new
                {
                    FlightID = id
                }
            );
        }

        public Flight? GetByFlightNumber(
            string flightNumber)
        {
            if (string.IsNullOrWhiteSpace(flightNumber))
                return null;

            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            return connection.QueryFirstOrDefault<Flight>(
                @"SELECT
                    FlightID,
                    FlightNumber,
                    Origin,
                    Destination,
                    DepartureTime,
                    AircraftCapacity,
                    AvailableSeats,
                    Status
                  FROM Flights
                  WHERE UPPER(FlightNumber) =
                        UPPER(@FlightNumber)",
                new
                {
                    FlightNumber = flightNumber.Trim()
                }
            );
        }

        public void Add(Flight flight)
        {
            if (flight == null)
                throw new ArgumentNullException(
                    nameof(flight)
                );

            ValidateFlight(flight);

            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            int duplicateCount =
                connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*)
                      FROM Flights
                      WHERE UPPER(FlightNumber) =
                            UPPER(@FlightNumber)",
                    new
                    {
                        FlightNumber =
                            flight.FlightNumber.Trim()
                    }
                );

            if (duplicateCount > 0)
            {
                throw new InvalidOperationException(
                    "A flight with that number already exists."
                );
            }

            int newId =
                connection.QuerySingle<int>(
                    @"INSERT INTO Flights
                        (
                            FlightNumber,
                            Origin,
                            Destination,
                            DepartureTime,
                            AircraftCapacity,
                            AvailableSeats,
                            Status
                        )
                      OUTPUT INSERTED.FlightID
                      VALUES
                        (
                            @FlightNumber,
                            @Origin,
                            @Destination,
                            @DepartureTime,
                            @AircraftCapacity,
                            @AvailableSeats,
                            @Status
                        )",
                    new
                    {
                        FlightNumber =
                            flight.FlightNumber
                                .Trim()
                                .ToUpper(),

                        Origin =
                            flight.Origin
                                .Trim()
                                .ToUpper(),

                        Destination =
                            flight.Destination
                                .Trim()
                                .ToUpper(),

                        flight.DepartureTime,
                        flight.AircraftCapacity,
                        flight.AvailableSeats,
                        flight.Status
                    }
                );

            flight.FlightID = newId;
        }

        public void Update(Flight flight)
        {
            if (flight == null)
                throw new ArgumentNullException(
                    nameof(flight)
                );

            ValidateFlight(flight);

            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            int affectedRows =
                connection.Execute(
                    @"UPDATE Flights
                      SET
                        FlightNumber = @FlightNumber,
                        Origin = @Origin,
                        Destination = @Destination,
                        DepartureTime = @DepartureTime,
                        AircraftCapacity = @AircraftCapacity,
                        AvailableSeats = @AvailableSeats,
                        Status = @Status
                      WHERE FlightID = @FlightID",
                    new
                    {
                        flight.FlightID,

                        FlightNumber =
                            flight.FlightNumber
                                .Trim()
                                .ToUpper(),

                        Origin =
                            flight.Origin
                                .Trim()
                                .ToUpper(),

                        Destination =
                            flight.Destination
                                .Trim()
                                .ToUpper(),

                        flight.DepartureTime,
                        flight.AircraftCapacity,
                        flight.AvailableSeats,
                        flight.Status
                    }
                );

            if (affectedRows == 0)
            {
                throw new InvalidOperationException(
                    "Flight was not found."
                );
            }
        }

        public void Delete(int id)
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            int bookingCount =
                connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*)
                      FROM Bookings
                      WHERE FlightID = @FlightID",
                    new
                    {
                        FlightID = id
                    }
                );

            if (bookingCount > 0)
            {
                throw new InvalidOperationException(
                    "This flight cannot be deleted because it has bookings."
                );
            }

            connection.Execute(
                @"DELETE FROM Flights
                  WHERE FlightID = @FlightID",
                new
                {
                    FlightID = id
                }
            );
        }

        private static void ValidateFlight(
            Flight flight)
        {
            if (string.IsNullOrWhiteSpace(
                    flight.FlightNumber))
            {
                throw new ArgumentException(
                    "Flight number is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    flight.Origin))
            {
                throw new ArgumentException(
                    "Origin is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    flight.Destination))
            {
                throw new ArgumentException(
                    "Destination is required."
                );
            }

            if (flight.Origin.Equals(
                    flight.Destination,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Origin and destination cannot be the same."
                );
            }

            if (flight.AircraftCapacity <= 0)
            {
                throw new ArgumentException(
                    "Aircraft capacity must be greater than zero."
                );
            }

            if (flight.AvailableSeats < 0 ||
                flight.AvailableSeats >
                flight.AircraftCapacity)
            {
                throw new ArgumentException(
                    "Available seats must be between zero and aircraft capacity."
                );
            }

            string[] allowedStatuses =
            {
                "Scheduled",
                "Boarding",
                "Departed"
            };

            bool validStatus =
                allowedStatuses.Any(
                    status =>
                        status.Equals(
                            flight.Status,
                            StringComparison.OrdinalIgnoreCase
                        )
                );

            if (!validStatus)
            {
                throw new ArgumentException(
                    "Invalid flight status."
                );
            }
        }
    }
}