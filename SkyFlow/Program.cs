using System;
using System.Collections.Generic;
using System.Linq;
using SkyFlow.Models;
using SkyFlow.Database;

namespace SkyFlow
{
    class Program
    {
        static readonly UserRepository userRepo = new UserRepository();
        static readonly FlightRepository flightRepo = new FlightRepository();
        static readonly BookingRepository bookingRepo = new BookingRepository();
        static readonly PassengerRepository passengerRepo = new PassengerRepository();

        static User? currentUser;

        static void Main(string[] args)
        {
            Console.Title = "SkyFlow Airport Management System";
            Console.ForegroundColor = ConsoleColor.Cyan;

            while (true)
            {
                ShowLoginScreen();

                if (currentUser == null)
                {
                    continue;
                }

                currentUser.ShowWelcomeMessage();

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();

                bool running = true;

                while (running && currentUser != null)
                {
                    currentUser.DisplayDashboard();

                    string input =
                        Console.ReadLine()?.Trim() ?? string.Empty;

                    if (currentUser is Admin)
                    {
                        running = HandleAdminMenu(input);
                    }
                    else if (currentUser is GateAgent)
                    {
                        running = HandleGateAgentMenu(input);
                    }
                    else
                    {
                        currentUser = null;
                        running = false;
                    }
                }
            }
        }

        static void ShowLoginScreen()
        {
            Console.Clear();

            Console.WriteLine("========================================");
            Console.WriteLine("       SKYFLOW AIRPORT SYSTEM           ");
            Console.WriteLine("            LOGIN PORTAL                ");
            Console.WriteLine("========================================");
            Console.WriteLine("  Demo Credentials:                     ");
            Console.WriteLine("  Admin:   admin / admin123             ");
            Console.WriteLine("  Agent:   agent1 / agent123            ");
            Console.WriteLine("========================================\n");

            Console.Write("  Username: ");

            string username =
                Console.ReadLine()?.Trim() ?? string.Empty;

            Console.Write("  Password: ");

            string password = ReadPassword();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine(
                    "\n Username and password are required.");

                Console.WriteLine(
                    "\nPress any key to try again...");

                Console.ReadKey();

                currentUser = null;

                return;
            }

            currentUser =
                userRepo.Authenticate(username, password);

            if (currentUser == null)
            {
                Console.WriteLine(
                    "\n Invalid username or password!");

                Console.WriteLine(
                    "\nPress any key to try again...");

                Console.ReadKey();
            }
        }

        static string ReadPassword()
        {
            string password = string.Empty;

            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace &&
                    key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;

                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace &&
                         password.Length > 0)
                {
                    password =
                        password.Substring(
                            0,
                            password.Length - 1);

                    Console.Write("\b \b");
                }

            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();

            return password;
        }

        static bool HandleAdminMenu(string option)
        {
            switch (option)
            {
                case "1":
                    ManageFlights();
                    break;

                case "2":
                    ViewSystemOverview();
                    break;

                case "3":
                    ManageStaff();
                    break;

                case "4":
                    Console.WriteLine("\nLogging out...");

                    currentUser = null;

                    return false;

                default:
                    Console.WriteLine(
                        "Invalid option. Press any key to continue...");

                    Console.ReadKey();

                    break;
            }

            return true;
        }

        static void ManageFlights()
        {
            while (true)
            {
                Console.Clear();

                List<Flight> flights =
                    flightRepo.GetAll().ToList();

                RenderFlightTable(flights);

                Console.WriteLine(
                    "\n========================================");

                Console.WriteLine(
                    "  Flight Management                     ");

                Console.WriteLine(
                    "========================================");

                Console.WriteLine(
                    "  1. Add New Flight                     ");

                Console.WriteLine(
                    "  2. Update Flight Status               ");

                Console.WriteLine(
                    "  3. Delete Flight                      ");

                Console.WriteLine(
                    "  4. Back to Main Menu                  ");

                Console.WriteLine(
                    "========================================");

                Console.Write("\nSelect option: ");

                string choice =
                    Console.ReadLine()?.Trim()
                    ?? string.Empty;

                if (choice == "4")
                {
                    break;
                }

                switch (choice)
                {
                    case "1":
                        AddNewFlight();
                        break;

                    case "2":
                        UpdateFlightStatus();
                        break;

                    case "3":
                        DeleteFlight();
                        break;

                    default:
                        Console.WriteLine(
                            "Invalid option!");

                        Console.ReadKey();

                        break;
                }
            }
        }

        static void RenderFlightTable(
            List<Flight> flights)
        {
            Console.Clear();

            Console.WriteLine(
                "\n========================================");

            Console.WriteLine(
                "         SKYFLOW FLIGHT SCHEDULE          ");

            Console.WriteLine(
                "========================================\n");

            Console.WriteLine(
                $"{"Flight #",-10} " +
                $"{"Origin",-12} " +
                $"{"Destination",-12} " +
                $"{"Departure Time",-20} " +
                $"{"Capacity",-10} " +
                $"{"Available",-10} " +
                $"{"Status",-15}");

            Console.WriteLine(
                new string('-', 90));

            foreach (Flight flight in flights)
            {
                string departureTime =
                    flight.DepartureTime.ToString(
                        "yyyy-MM-dd HH:mm");

                Console.WriteLine(
                    $"{flight.FlightNumber,-10} " +
                    $"{flight.Origin,-12} " +
                    $"{flight.Destination,-12} " +
                    $"{departureTime,-20} " +
                    $"{flight.AircraftCapacity,-10} " +
                    $"{flight.AvailableSeats,-10} " +
                    $"{flight.Status,-15}");
            }

            Console.WriteLine(
                "\n========================================");
        }

        static void AddNewFlight()
        {
            Console.Clear();

            Console.WriteLine(
                "========================================");

            Console.WriteLine(
                "         ADD NEW FLIGHT                 ");

            Console.WriteLine(
                "========================================\n");

            Console.Write(
                "Flight Number (e.g., SF999): ");

            string flightNumber =
                Console.ReadLine()?.Trim().ToUpper()
                ?? string.Empty;

            Console.Write(
                "Origin (e.g., JHB, CPT, DBN): ");

            string origin =
                Console.ReadLine()?.Trim().ToUpper()
                ?? string.Empty;

            Console.Write("Destination: ");

            string destination =
                Console.ReadLine()?.Trim().ToUpper()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(flightNumber) ||
                string.IsNullOrWhiteSpace(origin) ||
                string.IsNullOrWhiteSpace(destination))
            {
                Console.WriteLine(
                    "\n Flight number, origin and destination are required.");

                Console.WriteLine(
                    "\nPress any key to continue...");

                Console.ReadKey();

                return;
            }

            if (origin == destination)
            {
                Console.WriteLine(
                    "\n Origin and destination cannot be the same.");

                Console.WriteLine(
                    "\nPress any key to continue...");

                Console.ReadKey();

                return;
            }

            bool duplicateFlight =
                flightRepo.GetAll().Any(
                    f => f.FlightNumber.Equals(
                        flightNumber,
                        StringComparison.OrdinalIgnoreCase));

            if (duplicateFlight)
            {
                Console.WriteLine(
                    "\n A flight with that number already exists.");

                Console.WriteLine(
                    "\nPress any key to continue...");

                Console.ReadKey();

                return;
            }

            Console.Write(
                "Departure Date (yyyy-mm-dd): ");

            string date =
                Console.ReadLine()?.Trim()
                ?? string.Empty;

            Console.Write(
                "Departure Time (HH:MM): ");

            string time =
                Console.ReadLine()?.Trim()
                ?? string.Empty;

            if (!DateTime.TryParse(
                    $"{date} {time}",
                    out DateTime departureTime))
            {
                Console.WriteLine(
                    "\n Invalid departure date or time.");

                Console.WriteLine(
                    "\nPress any key to continue...");

                Console.ReadKey();

                return;
            }

            Console.Write(
                "Aircraft Capacity: ");

            string capacityInput =
                Console.ReadLine()?.Trim()
                ?? string.Empty;

            if (!int.TryParse(
                    capacityInput,
                    out int capacity) ||
                capacity <= 0)
            {
                Console.WriteLine(
                    "\n Aircraft capacity must be a positive number.");

                Console.WriteLine(
                    "\nPress any key to continue...");

                Console.ReadKey();

                return;
            }

            Flight newFlight =
                new Flight
                {
                    FlightNumber = flightNumber,
                    Origin = origin,
                    Destination = destination,
                    DepartureTime = departureTime,
                    AircraftCapacity = capacity,
                    AvailableSeats = capacity,
                    Status = "Scheduled"
                };

            flightRepo.Add(newFlight);

            Console.WriteLine(
                "\n Flight added successfully!");

            Console.WriteLine(
                "\nPress any key to continue...");

            Console.ReadKey();
        }

        static void UpdateFlightStatus()
        {
            List<Flight> flights =
                flightRepo.GetAll().ToList();

            RenderFlightTable(flights);

            Console.Write(
                "\nEnter Flight Number to update: ");

            string flightNum =
                Console.ReadLine()?.Trim().ToUpper()
                ?? string.Empty;

            Flight? flight =
                flights.FirstOrDefault(
                    f => f.FlightNumber.Equals(
                        flightNum,
                        StringComparison.OrdinalIgnoreCase));

            if (flight == null)
            {
                Console.WriteLine(
                    " Flight not found!");

                Console.ReadKey();

                return;
            }

            Console.WriteLine(
                "\nSelect new status:");

            Console.WriteLine("1. Scheduled");
            Console.WriteLine("2. Boarding");
            Console.WriteLine("3. Departed");

            Console.Write("Choice: ");

            string choice =
                Console.ReadLine()?.Trim()
                ?? string.Empty;

            string? status =
                choice switch
                {
                    "1" => "Scheduled",
                    "2" => "Boarding",
                    "3" => "Departed",
                    _ => null
                };

            if (status == null)
            {
                Console.WriteLine(
                    "\n Invalid status selection.");

                Console.ReadKey();

                return;
            }

            flight.Status = status;

            flightRepo.Update(flight);

            Console.WriteLine(
                $"\n Flight {flight.FlightNumber} " +
                $"status updated to {status}");

            Console.ReadKey();
        }

        static void DeleteFlight()
        {
            List<Flight> flights =
                flightRepo.GetAll().ToList();

            RenderFlightTable(flights);

            Console.Write(
                "\nEnter Flight Number to delete: ");

            string flightNum =
                Console.ReadLine()?.Trim().ToUpper()
                ?? string.Empty;

            Flight? flight =
                flights.FirstOrDefault(
                    f => f.FlightNumber.Equals(
                        flightNum,
                        StringComparison.OrdinalIgnoreCase));

            if (flight == null)
            {
                Console.WriteLine(
                    " Flight not found!");

                Console.ReadKey();

                return;
            }

            Console.Write(
                $"Are you sure you want to delete " +
                $"flight {flightNum}? (Y/N): ");

            string confirmation =
                Console.ReadLine()?.Trim().ToUpper()
                ?? string.Empty;

            if (confirmation == "Y")
            {
                flightRepo.Delete(
                    flight.FlightID);

                Console.WriteLine(
                    " Flight deleted!");
            }
            else
            {
                Console.WriteLine(
                    " Flight deletion cancelled.");
            }

            Console.ReadKey();
        }

        static void ViewSystemOverview()
        {
            Console.Clear();

            List<Flight> flights =
                flightRepo.GetAll().ToList();

            List<Booking> bookings =
                bookingRepo.GetAll().ToList();

            Console.WriteLine(
                "========================================");

            Console.WriteLine(
                "              SYSTEM OVERVIEW           ");

            Console.WriteLine(
                "========================================\n");

            Console.WriteLine(
                $"Total Flights: {flights.Count}");

            Console.WriteLine(
                $"Total Bookings: {bookings.Count}");

            Console.WriteLine(
                "\nFlights by Status:");

            Console.WriteLine(
                $"  - Scheduled: " +
                $"{flights.Count(f => f.Status == "Scheduled")}");

            Console.WriteLine(
                $"  - Boarding:  " +
                $"{flights.Count(f => f.Status == "Boarding")}");

            Console.WriteLine(
                $"  - Departed:  " +
                $"{flights.Count(f => f.Status == "Departed")}");

            int totalCapacity =
                flights.Sum(
                    f => f.AircraftCapacity);

            int totalBooked =
                flights.Sum(
                    f => f.AircraftCapacity -
                         f.AvailableSeats);

            double occupancyRate =
                totalCapacity > 0
                    ? (double)totalBooked /
                      totalCapacity * 100
                    : 0;

            Console.WriteLine(
                $"\nOverall Occupancy Rate: " +
                $"{occupancyRate:F1}%");

            Console.WriteLine(
                "\nPress any key to continue...");

            Console.ReadKey();
        }

        static void ManageStaff()
        {
            while (true)
            {
                Console.Clear();

                List<User> users =
                    userRepo.GetAll().ToList();

                Console.WriteLine(
                    "\n========================================");

                Console.WriteLine(
                    "              STAFF DIRECTORY           ");

                Console.WriteLine(
                    "========================================\n");

                Console.WriteLine(
                    $"{"ID",-5} " +
                    $"{"Username",-15} " +
                    $"{"Full Name",-25} " +
                    $"{"Role",-12}");

                Console.WriteLine(
                    new string('-', 60));

                foreach (User user in users)
                {
                    Console.WriteLine(
                        $"{user.UserID,-5} " +
                        $"{user.Username,-15} " +
                        $"{user.FullName,-25} " +
                        $"{user.Role,-12}");
                }

                Console.WriteLine(
                    "\n========================================");

                Console.WriteLine(
                    "  Staff Management                      ");

                Console.WriteLine(
                    "========================================");

                Console.WriteLine(
                    "  1. Add New Staff Member               ");

                Console.WriteLine(
                    "  2. Back to Main Menu                  ");

                Console.WriteLine(
                    "========================================");

                Console.Write(
                    "\nSelect option: ");

                string choice =
                    Console.ReadLine()?.Trim()
                    ?? string.Empty;

                if (choice == "2")
                {
                    break;
                }

                if (choice != "1")
                {
                    Console.WriteLine(
                        "\nInvalid option.");

                    Console.ReadKey();

                    continue;
                }

                Console.Clear();

                Console.WriteLine(
                    "========================================");

                Console.WriteLine(
                    "         ADD NEW STAFF MEMBER           ");

                Console.WriteLine(
                    "========================================\n");

                Console.Write("Username: ");

                string username =
                    Console.ReadLine()?.Trim()
                    ?? string.Empty;

                Console.Write("Full Name: ");

                string fullName =
                    Console.ReadLine()?.Trim()
                    ?? string.Empty;

                Console.Write("Password: ");

                string password =
                    ReadPassword();

                Console.Write(
                    "Role (Admin/GateAgent): ");

                string role =
                    Console.ReadLine()?.Trim()
                    ?? string.Empty;

                if (string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(fullName) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine(
                        "\n All fields are required.");

                    Console.ReadKey();

                    continue;
                }

                if (userRepo.GetAll().Any(
                    u => u.Username.Equals(
                        username,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine(
                        "\n Username already exists.");

                    Console.ReadKey();

                    continue;
                }

                User newUser;

                if (role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
                {
                    newUser = new Admin();

                    role = "Admin";
                }
                else if (role.Equals(
                    "GateAgent",
                    StringComparison.OrdinalIgnoreCase))
                {
                    newUser = new GateAgent();

                    role = "GateAgent";
                }
                else
                {
                    Console.WriteLine(
                        "\n Role must be Admin or GateAgent.");

                    Console.ReadKey();

                    continue;
                }

                newUser.Username = username;
                newUser.FullName = fullName;

                // UserRepository hashes the password
                // before storing it.
                newUser.PasswordHash = password;

                newUser.Role = role;

                userRepo.Add(newUser);

                Console.WriteLine(
                    "\n Staff member added!");

                Console.ReadKey();
            }
        }

        static bool HandleGateAgentMenu(
            string option)
        {
            switch (option)
            {
                case "1":
                    ViewFlightManifest();
                    break;

                case "2":
                    PassengerCheckIn();
                    break;

                case "3":
                    BoardingGate();
                    break;

                case "4":
                    Console.WriteLine(
                        "\nLogging out...");

                    currentUser = null;

                    return false;

                default:
                    Console.WriteLine(
                        "Invalid option.");

                    Console.ReadKey();

                    break;
            }

            return true;
        }

        static void ViewFlightManifest()
        {
            Console.Clear();

            List<Flight> flights =
                flightRepo.GetAll().ToList();

            RenderFlightTable(flights);

            Console.Write(
                "\nEnter Flight Number to view manifest: ");

            string flightNum =
                Console.ReadLine()?.Trim().ToUpper()
                ?? string.Empty;

            Flight? flight =
                flights.FirstOrDefault(
                    f => f.FlightNumber.Equals(
                        flightNum,
                        StringComparison.OrdinalIgnoreCase));

            if (flight == null)
            {
                Console.WriteLine(
                    " Flight not found!");

                Console.ReadKey();

                return;
            }

            List<Booking> bookings =
                bookingRepo
                    .GetBookingsByFlight(
                        flight.FlightID)
                    .ToList();

            if (!bookings.Any())
            {
                Console.WriteLine(
                    $"\nNo passengers booked on " +
                    $"flight {flightNum}");
            }
            else
            {
                Console.WriteLine(
                    $"\n========== FLIGHT MANIFEST - " +
                    $"{flightNum} " +
                    $"({flight.Origin} to " +
                    $"{flight.Destination}) ==========\n");

                Console.WriteLine(
                    $"{"Booking ID",-12} " +
                    $"{"Passenger Name",-25} " +
                    $"{"Seat",-8} " +
                    $"{"Status",-12}");

                Console.WriteLine(
                    new string('-', 60));

                foreach (Booking booking in bookings)
                {
                    Console.WriteLine(
                        $"{booking.BookingID,-12} " +
                        $"{booking.PassengerName,-25} " +
                        $"{booking.SeatNumber,-8} " +
                        $"{booking.Status,-12}");
                }
            }

            Console.WriteLine(
                "\nPress any key to continue...");

            Console.ReadKey();
        }

        static void PassengerCheckIn()
        {
            Console.Clear();

            Console.WriteLine(
                "========================================");

            Console.WriteLine(
                "         PASSENGER CHECK-IN             ");

            Console.WriteLine(
                "========================================\n");

            Console.Write(
                "Enter Passenger ID or Passport Number: ");

            string search =
                Console.ReadLine()?.Trim()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(search))
            {
                Console.WriteLine(
                    "\n Passenger ID or passport number is required.");

                Console.ReadKey();

                return;
            }

            Passenger? passenger;

            if (int.TryParse(
                    search,
                    out int passengerId))
            {
                passenger =
                    passengerRepo.GetById(
                        passengerId);
            }
            else
            {
                passenger =
                    passengerRepo.GetByPassport(
                        search);
            }

            if (passenger == null)
            {
                Console.WriteLine(
                    " Passenger not found!");

                Console.ReadKey();

                return;
            }

            Console.WriteLine(
                $"\n Passenger found: " +
                $"{passenger.FullName}");

            Console.WriteLine(
                $"  Passport: " +
                $"{passenger.PassportNumber}");

            Console.WriteLine(
                $"  Email: {passenger.Email}");

            Console.WriteLine(
                $"  Phone: {passenger.PhoneNumber}");

            List<Booking> allBookings =
                bookingRepo.GetAll()
                    .Where(
                        b => b.PassengerID ==
                             passenger.PassengerID)
                    .ToList();

            if (!allBookings.Any())
            {
                Console.WriteLine(
                    "\nNo bookings found for this passenger.");

                Console.ReadKey();

                return;
            }

            Console.WriteLine(
                "\n========== PASSENGER BOOKINGS ==========\n");

            Console.WriteLine(
                $"{"Booking ID",-12} " +
                $"{"Flight",-10} " +
                $"{"Seat",-8} " +
                $"{"Status",-12}");

            Console.WriteLine(
                new string('-', 45));

            foreach (Booking booking in allBookings)
            {
                Console.WriteLine(
                    $"{booking.BookingID,-12} " +
                    $"{booking.FlightNumber,-10} " +
                    $"{booking.SeatNumber,-8} " +
                    $"{booking.Status,-12}");
            }

            Console.Write(
                "\nEnter Booking ID to check in: ");

            string bookingInput =
                Console.ReadLine()?.Trim()
                ?? string.Empty;

            if (!int.TryParse(
                    bookingInput,
                    out int bookingId))
            {
                Console.WriteLine(
                    "\n Invalid booking ID.");

                Console.ReadKey();

                return;
            }

            Booking? selectedBooking =
                allBookings.FirstOrDefault(
                    b => b.BookingID == bookingId);

            if (selectedBooking == null)
            {
                Console.WriteLine(
                    "\n Invalid booking!");

                Console.ReadKey();

                return;
            }

            if (selectedBooking.Status == "CheckedIn" ||
                selectedBooking.Status == "Boarded")
            {
                Console.WriteLine(
                    $"\n Passenger already " +
                    $"{selectedBooking.Status}!");

                Console.ReadKey();

                return;
            }

            bookingRepo.UpdateBookingStatus(
                bookingId,
                "CheckedIn");

            Console.WriteLine(
                $"\n Passenger {passenger.FullName} " +
                "checked in successfully!");

            Console.WriteLine(
                $"  Seat: {selectedBooking.SeatNumber}");

            Flight? flight =
                flightRepo.GetById(
                    selectedBooking.FlightID);

            if (flight != null &&
                flight.AvailableSeats > 0)
            {
                flight.AvailableSeats--;

                flightRepo.Update(flight);
            }

            Console.ReadKey();
        }

        static void BoardingGate()
        {
            Console.Clear();

            List<Flight> flights =
                flightRepo.GetAll()
                    .Where(
                        f => f.Status == "Boarding" ||
                             f.Status == "Scheduled")
                    .ToList();

            if (!flights.Any())
            {
                Console.WriteLine(
                    "No flights available for boarding.");

                Console.ReadKey();

                return;
            }

            RenderFlightTable(flights);

            Console.Write(
                "\nEnter Flight Number for boarding: ");

            string flightNum =
                Console.ReadLine()?.Trim().ToUpper()
                ?? string.Empty;

            Flight? flight =
                flights.FirstOrDefault(
                    f => f.FlightNumber.Equals(
                        flightNum,
                        StringComparison.OrdinalIgnoreCase));

            if (flight == null)
            {
                Console.WriteLine(
                    " Flight not found!");

                Console.ReadKey();

                return;
            }

            if (flight.Status != "Boarding")
            {
                Console.Write(
                    $"\nFlight {flightNum} is not in " +
                    "boarding status. Start boarding? (Y/N): ");

                string startBoarding =
                    Console.ReadLine()?.Trim().ToUpper()
                    ?? string.Empty;

                if (startBoarding == "Y")
                {
                    flight.Status = "Boarding";

                    flightRepo.Update(flight);

                    Console.WriteLine(
                        $"\n Boarding started for " +
                        $"flight {flightNum}");
                }
                else
                {
                    Console.WriteLine(
                        "\n Boarding cancelled.");

                    Console.ReadKey();

                    return;
                }
            }

            List<Booking> checkedInPassengers =
                bookingRepo
                    .GetBookingsByFlight(
                        flight.FlightID)
                    .Where(
                        b => b.Status == "CheckedIn")
                    .ToList();

            if (!checkedInPassengers.Any())
            {
                Console.WriteLine(
                    "\nNo checked-in passengers " +
                    "for this flight.");

                Console.ReadKey();

                return;
            }

            Console.WriteLine(
                $"\nFound {checkedInPassengers.Count} " +
                "checked-in passenger(s).\n");

            Console.WriteLine(
                "========== READY FOR BOARDING ==========\n");

            Console.WriteLine(
                $"{"Passenger Name",-25} " +
                $"{"Seat",-8} " +
                $"{"Status",-12}");

            Console.WriteLine(
                new string('-', 50));

            foreach (Booking passenger
                     in checkedInPassengers)
            {
                Console.WriteLine(
                    $"{passenger.PassengerName,-25} " +
                    $"{passenger.SeatNumber,-8} " +
                    $"{passenger.Status,-12}");
            }

            Console.Write(
                "\nBoard all checked-in passengers? (Y/N): ");

            string confirmation =
                Console.ReadLine()?.Trim().ToUpper()
                ?? string.Empty;

            if (confirmation == "Y")
            {
                foreach (Booking passenger
                         in checkedInPassengers)
                {
                    bookingRepo.UpdateBookingStatus(
                        passenger.BookingID,
                        "Boarded");

                    Console.WriteLine(
                        $"  Boarded: " +
                        $"{passenger.PassengerName} " +
                        $"(Seat {passenger.SeatNumber})");
                }

                flight.Status = "Departed";

                flightRepo.Update(flight);

                Console.WriteLine(
                    $"\n All passengers boarded! " +
                    $"Flight {flightNum} has departed.");
            }
            else
            {
                Console.WriteLine(
                    "\n Boarding cancelled.");
            }

            Console.ReadKey();
        }
    }
}