# ✈️ SkyFlow - Airport Management System

SkyFlow is a C# console-based airport management application designed to simulate core airport operations such as flight management, passenger check-in, boarding, staff management, and role-based access control.

The project demonstrates practical use of Object-Oriented Programming, repository-based architecture, input validation, secure password handling, and relational database design.

## 🚀 Key Features

### 🔐 Authentication & Security

- Role-based authentication for Administrators and Gate Agents
- Passwords are hashed using PBKDF2 with SHA-256 and unique random salts
- Constant-time password hash comparison
- Password masking during login and staff creation
- Case-insensitive username authentication
- Input validation for login and user creation

### 👨‍💼 Administrator

Administrators can:

- View all scheduled flights
- Add new flights
- Update flight status
- Delete flights
- View system statistics
- Monitor overall flight occupancy
- View the staff directory
- Create new Administrator or Gate Agent accounts

### 🧑‍✈️ Gate Agent

Gate Agents can:

- View flight manifests
- Search passengers by ID or passport number
- View passenger booking information
- Check passengers in
- View assigned seat numbers
- Start the boarding process
- Board checked-in passengers
- Update flights to departed status

## 🛫 Flight Management

SkyFlow maintains information including:

- Flight number
- Origin
- Destination
- Departure date and time
- Aircraft capacity
- Available seats
- Flight status

The application validates flight information before a new flight is created, including duplicate flight numbers, aircraft capacity, and route information.

## 👤 Passenger Management

Passenger records contain:

- Passenger ID
- Passport number
- Full name
- Email address
- Phone number

Passengers can be searched using either their unique ID or passport number.

All passenger information included with the project is synthetic demonstration data.

## 🎫 Booking & Check-In

Bookings connect passengers to flights and include:

- Booking ID
- Flight
- Passenger
- Seat number
- Booking status
- Booking date

Supported booking states include:

- Confirmed
- CheckedIn
- Boarded

The check-in workflow updates booking status and flight seat availability.

## 🗄️ Database Design

SkyFlow includes a SQL Server database setup script for `SkyFlowDB`.

The database contains four main tables:

```text
Users
  |
  | authentication and staff information
  |
Flights -------- Bookings -------- Passengers
```

### Tables

**Users**
- UserID
- Username
- PasswordHash
- FullName
- Role
- CreatedDate

**Flights**
- FlightID
- FlightNumber
- Origin
- Destination
- DepartureTime
- AircraftCapacity
- AvailableSeats
- Status

**Passengers**
- PassengerID
- PassportNumber
- FullName
- Email
- PhoneNumber

**Bookings**
- BookingID
- FlightID
- PassengerID
- SeatNumber
- Status
- BookingDate

The SQL schema includes:

- Primary keys
- Foreign-key relationships
- Unique constraints
- Role validation
- Flight-status validation
- Booking-status validation
- Aircraft-capacity validation
- Duplicate-seat prevention

> **Current implementation note:** The application currently uses in-memory repositories for its runtime demonstration data. The included SQL Server schema represents the persistence layer being prepared for full database-backed repository integration.

## 🧠 Object-Oriented Programming

SkyFlow demonstrates several important OOP concepts.

### Inheritance

`Admin` and `GateAgent` inherit common functionality from the base `User` class.

```text
User
├── Admin
└── GateAgent
```

### Polymorphism

The abstract `DisplayDashboard()` method allows each user type to display a different dashboard at runtime.

### Encapsulation

Application data and repository operations are organized into dedicated models and repository classes.

### Abstraction

Interfaces and repository classes help separate application logic from data-access responsibilities.

## 🏗️ Project Structure

```text
SkyFlow_Git/
│
├── README.md
├── .gitignore
│
└── SkyFlow/
    │
    ├── Database/
    │   ├── BookingRepository.cs
    │   ├── DatabaseConnection.cs
    │   ├── FlightRepository.cs
    │   ├── PassengerRepository.cs
    │   └── UserRepository.cs
    │
    ├── Interfaces/
    │   ├── IDataRepository.cs
    │   └── IDisplayable.cs
    │
    ├── Models/
    │   ├── Admin.cs
    │   ├── Booking.cs
    │   ├── Flight.cs
    │   ├── GateAgent.cs
    │   ├── Passenger.cs
    │   └── User.cs
    │
    ├── Services/
    │   └── TableRenderer.cs
    │
    ├── SQL/
    │   └── SETUPDATABASE.SQL
    │
    ├── Program.cs
    └── SkyFlow.csproj
```

## 🛠️ Technologies

- C#
- .NET
- SQL Server
- Microsoft.Data.SqlClient
- Dapper
- LINQ
- Git
- GitHub

## 🔒 Security Improvements

The project includes several security-focused improvements:

- PBKDF2 password hashing
- SHA-256 password derivation
- Random password salts
- Constant-time password verification
- Masked password entry
- Environment-variable support for custom database connection strings
- No production credentials stored in the repository

For demonstration purposes, development login credentials are shown by the application when it starts.

## ⚙️ Getting Started

### Requirements

Install:

- .NET SDK
- Git
- SQL Server LocalDB or SQL Server if using the included database schema

### Clone the Repository

```bash
git clone https://github.com/BanKanku/SkyFlow_Git.git
cd SkyFlow_Git
```

### Build

```bash
dotnet build SkyFlow/SkyFlow.csproj
```

### Run

```bash
dotnet run --project SkyFlow/SkyFlow.csproj
```

## 🗃️ Optional SQL Server Setup

The SQL database schema is located at:

```text
SkyFlow/SQL/SETUPDATABASE.SQL
```

The default development connection targets:

```text
(localdb)\MSSQLLocalDB
```

and the database:

```text
SkyFlowDB
```

A custom connection string can be supplied through the environment variable:

```text
SKYFLOW_CONNECTION_STRING
```

The current runtime repositories use in-memory data, so SQL Server is not required simply to run the console demonstration.

## 📚 Skills Demonstrated

This project demonstrates experience with:

- C# application development
- Object-Oriented Programming
- Repository design
- LINQ
- Authentication logic
- Password security
- Input validation
- Exception handling
- SQL database design
- Relational data modelling
- Git version control
- Console UI development

## 🔮 Planned Improvements

Future development can include:

- Full SQL-backed repository implementation
- Passenger and booking creation
- Flight search and filtering
- Seat assignment validation
- CSV flight-manifest export
- Audit logging
- Automated unit tests
- Additional reporting and analytics
- Migration to a desktop or web-based interface

## 👨‍💻 Developer

**Ban Kanku**

IT Student & Software Developer

Technologies: C# • Java • Python • SQL

GitHub: **BanKanku**