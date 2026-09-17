using System;
using System.Collections.Generic;
using System.Linq;
using SkyFlow.Models;

namespace SkyFlow.Database
{
    public class FlightRepository
    {
        private static readonly List<Flight> flights = new List<Flight>();

        public FlightRepository()
        {
            if (flights.Count == 0)
            {
                flights.Add(new Flight
                {
                    FlightID = 1,
                    FlightNumber = "SF102",
                    Origin = "CPT",
                    Destination = "JHB",
                    DepartureTime = DateTime.Now.AddHours(2),
                    AircraftCapacity = 150,
                    AvailableSeats = 148,
                    Status = "Scheduled"
                });

                flights.Add(new Flight
                {
                    FlightID = 2,
                    FlightNumber = "SF221",
                    Origin = "DBN",
                    Destination = "CPT",
                    DepartureTime = DateTime.Now.AddHours(4),
                    AircraftCapacity = 120,
                    AvailableSeats = 115,
                    Status = "Scheduled"
                });

                flights.Add(new Flight
                {
                    FlightID = 3,
                    FlightNumber = "SF305",
                    Origin = "JHB",
                    Destination = "PLZ",
                    DepartureTime = DateTime.Now.AddHours(6),
                    AircraftCapacity = 100,
                    AvailableSeats = 95,
                    Status = "Boarding"
                });
            }
        }

        public List<Flight> GetAll()
        {
            return flights;
        }

        public Flight? GetById(int id)
        {
            return flights.FirstOrDefault(f => f.FlightID == id);
        }

        public void Add(Flight flight)
        {
            flight.FlightID = flights.Count == 0
                ? 1
                : flights.Max(f => f.FlightID) + 1;

            flights.Add(flight);
        }

        public void Update(Flight flight)
        {
            Flight? existing =
                flights.FirstOrDefault(f => f.FlightID == flight.FlightID);

            if (existing != null)
            {
                existing.FlightNumber = flight.FlightNumber;
                existing.Origin = flight.Origin;
                existing.Destination = flight.Destination;
                existing.DepartureTime = flight.DepartureTime;
                existing.AircraftCapacity = flight.AircraftCapacity;
                existing.AvailableSeats = flight.AvailableSeats;
                existing.Status = flight.Status;
            }
        }

        public void Delete(int id)
        {
            Flight? flight =
                flights.FirstOrDefault(f => f.FlightID == id);

            if (flight != null)
            {
                flights.Remove(flight);
            }
        }
    }
}