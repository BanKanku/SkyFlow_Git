using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using SkyFlow.Models;

namespace SkyFlow.Database
{
    public class SqlPassengerRepository
    {
        public List<Passenger> GetAll()
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            return connection.Query<Passenger>(
                @"SELECT
                    PassengerID,
                    PassportNumber,
                    FullName,
                    Email,
                    PhoneNumber
                  FROM Passengers
                  ORDER BY PassengerID"
            ).ToList();
        }

        public Passenger? GetById(int id)
        {
            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            return connection.QueryFirstOrDefault<Passenger>(
                @"SELECT
                    PassengerID,
                    PassportNumber,
                    FullName,
                    Email,
                    PhoneNumber
                  FROM Passengers
                  WHERE PassengerID = @PassengerID",
                new
                {
                    PassengerID = id
                }
            );
        }

        public Passenger? GetByPassport(
            string passportNumber)
        {
            if (string.IsNullOrWhiteSpace(
                    passportNumber))
            {
                return null;
            }

            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            return connection.QueryFirstOrDefault<Passenger>(
                @"SELECT
                    PassengerID,
                    PassportNumber,
                    FullName,
                    Email,
                    PhoneNumber
                  FROM Passengers
                  WHERE UPPER(PassportNumber) =
                        UPPER(@PassportNumber)",
                new
                {
                    PassportNumber =
                        passportNumber.Trim()
                }
            );
        }

        public void Add(Passenger passenger)
        {
            if (passenger == null)
            {
                throw new ArgumentNullException(
                    nameof(passenger)
                );
            }

            ValidatePassenger(passenger);

            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            int duplicatePassport =
                connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*)
                      FROM Passengers
                      WHERE UPPER(PassportNumber) =
                            UPPER(@PassportNumber)",
                    new
                    {
                        PassportNumber =
                            passenger.PassportNumber.Trim()
                    }
                );

            if (duplicatePassport > 0)
            {
                throw new InvalidOperationException(
                    "A passenger with that passport number already exists."
                );
            }

            int newId =
                connection.QuerySingle<int>(
                    @"INSERT INTO Passengers
                        (
                            PassportNumber,
                            FullName,
                            Email,
                            PhoneNumber
                        )
                      OUTPUT INSERTED.PassengerID
                      VALUES
                        (
                            @PassportNumber,
                            @FullName,
                            @Email,
                            @PhoneNumber
                        )",
                    new
                    {
                        PassportNumber =
                            passenger.PassportNumber
                                .Trim()
                                .ToUpper(),

                        FullName =
                            passenger.FullName.Trim(),

                        Email =
                            passenger.Email.Trim(),

                        PhoneNumber =
                            passenger.PhoneNumber.Trim()
                    }
                );

            passenger.PassengerID =
                newId;
        }

        public void Update(Passenger passenger)
        {
            if (passenger == null)
            {
                throw new ArgumentNullException(
                    nameof(passenger)
                );
            }

            ValidatePassenger(passenger);

            using var connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            int duplicatePassport =
                connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*)
                      FROM Passengers
                      WHERE UPPER(PassportNumber) =
                            UPPER(@PassportNumber)
                        AND PassengerID <> @PassengerID",
                    new
                    {
                        PassportNumber =
                            passenger.PassportNumber.Trim(),

                        passenger.PassengerID
                    }
                );

            if (duplicatePassport > 0)
            {
                throw new InvalidOperationException(
                    "A passenger with that passport number already exists."
                );
            }

            int affectedRows =
                connection.Execute(
                    @"UPDATE Passengers
                      SET
                        PassportNumber = @PassportNumber,
                        FullName = @FullName,
                        Email = @Email,
                        PhoneNumber = @PhoneNumber
                      WHERE PassengerID = @PassengerID",
                    new
                    {
                        passenger.PassengerID,

                        PassportNumber =
                            passenger.PassportNumber
                                .Trim()
                                .ToUpper(),

                        FullName =
                            passenger.FullName.Trim(),

                        Email =
                            passenger.Email.Trim(),

                        PhoneNumber =
                            passenger.PhoneNumber.Trim()
                    }
                );

            if (affectedRows == 0)
            {
                throw new InvalidOperationException(
                    "Passenger was not found."
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
                      WHERE PassengerID = @PassengerID",
                    new
                    {
                        PassengerID = id
                    }
                );

            if (bookingCount > 0)
            {
                throw new InvalidOperationException(
                    "This passenger cannot be deleted because they have bookings."
                );
            }

            connection.Execute(
                @"DELETE FROM Passengers
                  WHERE PassengerID = @PassengerID",
                new
                {
                    PassengerID = id
                }
            );
        }

        private static void ValidatePassenger(
            Passenger passenger)
        {
            if (string.IsNullOrWhiteSpace(
                    passenger.PassportNumber))
            {
                throw new ArgumentException(
                    "Passport number is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    passenger.FullName))
            {
                throw new ArgumentException(
                    "Passenger full name is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    passenger.Email))
            {
                throw new ArgumentException(
                    "Email address is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    passenger.PhoneNumber))
            {
                throw new ArgumentException(
                    "Phone number is required."
                );
            }

            if (!passenger.Email.Contains('@'))
            {
                throw new ArgumentException(
                    "A valid email address is required."
                );
            }
        }
    }
}