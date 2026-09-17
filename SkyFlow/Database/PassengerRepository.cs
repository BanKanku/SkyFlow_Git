using System;
using System.Collections.Generic;
using System.Linq;
using SkyFlow.Models;

namespace SkyFlow.Database
{
    public class PassengerRepository
    {
        private static readonly List<Passenger> passengers = new List<Passenger>();

        public PassengerRepository()
        {
            if (passengers.Count == 0)
            {
                passengers.Add(new Passenger
                {
                    PassengerID = 1,
                    PassportNumber = "DEMO10001",
                    FullName = "Alex Morgan",
                    Email = "alex.morgan@example.com",
                    PhoneNumber = "0000000001"
                });

                passengers.Add(new Passenger
                {
                    PassengerID = 2,
                    PassportNumber = "DEMO10002",
                    FullName = "Jordan Lee",
                    Email = "jordan.lee@example.com",
                    PhoneNumber = "0000000002"
                });

                passengers.Add(new Passenger
                {
                    PassengerID = 3,
                    PassportNumber = "DEMO10003",
                    FullName = "Taylor Reed",
                    Email = "taylor.reed@example.com",
                    PhoneNumber = "0000000003"
                });
            }
        }

        public List<Passenger> GetAll()
        {
            return passengers;
        }

        public Passenger? GetById(int id)
        {
            return passengers.FirstOrDefault(p => p.PassengerID == id);
        }

        public Passenger? GetByPassport(string passportNumber)
        {
            return passengers.FirstOrDefault(p =>
                p.PassportNumber.Equals(
                    passportNumber,
                    StringComparison.OrdinalIgnoreCase));
        }

        public void Add(Passenger passenger)
        {
            passenger.PassengerID = passengers.Count == 0
                ? 1
                : passengers.Max(p => p.PassengerID) + 1;

            passengers.Add(passenger);
        }

        public void Update(Passenger passenger)
        {
            Passenger? existing =
                passengers.FirstOrDefault(
                    p => p.PassengerID == passenger.PassengerID);

            if (existing != null)
            {
                existing.PassportNumber = passenger.PassportNumber;
                existing.FullName = passenger.FullName;
                existing.Email = passenger.Email;
                existing.PhoneNumber = passenger.PhoneNumber;
            }
        }

        public void Delete(int id)
        {
            Passenger? passenger =
                passengers.FirstOrDefault(p => p.PassengerID == id);

            if (passenger != null)
            {
                passengers.Remove(passenger);
            }
        }
    }
}