-- =========================================================
-- SkyFlow Airport Management System
-- Database Setup Script
-- =========================================================

IF DB_ID('SkyFlowDB') IS NULL
BEGIN
    CREATE DATABASE SkyFlowDB;
END;
GO

USE SkyFlowDB;
GO

-- =========================================================
-- Users
-- =========================================================

CREATE TABLE Users
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT CK_Users_Role
        CHECK (Role IN ('Admin', 'GateAgent'))
);
GO

-- =========================================================
-- Flights
-- =========================================================

CREATE TABLE Flights
(
    FlightID INT IDENTITY(1,1) PRIMARY KEY,
    FlightNumber NVARCHAR(10) NOT NULL UNIQUE,
    Origin NVARCHAR(50) NOT NULL,
    Destination NVARCHAR(50) NOT NULL,
    DepartureTime DATETIME2 NOT NULL,
    AircraftCapacity INT NOT NULL,
    AvailableSeats INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Scheduled',

    CONSTRAINT CK_Flights_Capacity
        CHECK (AircraftCapacity > 0),

    CONSTRAINT CK_Flights_AvailableSeats
        CHECK (
            AvailableSeats >= 0
            AND AvailableSeats <= AircraftCapacity
        ),

    CONSTRAINT CK_Flights_Route
        CHECK (Origin <> Destination),

    CONSTRAINT CK_Flights_Status
        CHECK (
            Status IN (
                'Scheduled',
                'Boarding',
                'Departed'
            )
        )
);
GO

-- =========================================================
-- Passengers
-- =========================================================

CREATE TABLE Passengers
(
    PassengerID INT IDENTITY(1,1) PRIMARY KEY,
    PassportNumber NVARCHAR(20) NOT NULL UNIQUE,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL,
    PhoneNumber NVARCHAR(20) NULL
);
GO

-- =========================================================
-- Bookings
-- =========================================================

CREATE TABLE Bookings
(
    BookingID INT IDENTITY(1,1) PRIMARY KEY,
    FlightID INT NOT NULL,
    PassengerID INT NOT NULL,
    SeatNumber NVARCHAR(5) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Confirmed',
    BookingDate DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Bookings_Flights
        FOREIGN KEY (FlightID)
        REFERENCES Flights(FlightID),

    CONSTRAINT FK_Bookings_Passengers
        FOREIGN KEY (PassengerID)
        REFERENCES Passengers(PassengerID),

    CONSTRAINT UQ_Bookings_FlightSeat
        UNIQUE (FlightID, SeatNumber),

    CONSTRAINT CK_Bookings_Status
        CHECK (
            Status IN (
                'Confirmed',
                'CheckedIn',
                'Boarded'
            )
        )
);
GO

-- =========================================================
-- Demo Flights
-- =========================================================

INSERT INTO Flights
(
    FlightNumber,
    Origin,
    Destination,
    DepartureTime,
    AircraftCapacity,
    AvailableSeats,
    Status
)
VALUES
(
    'SF102',
    'CPT',
    'JHB',
    DATEADD(HOUR, 2, GETDATE()),
    150,
    148,
    'Scheduled'
),
(
    'SF221',
    'DBN',
    'CPT',
    DATEADD(HOUR, 4, GETDATE()),
    120,
    115,
    'Scheduled'
),
(
    'SF305',
    'JHB',
    'PLZ',
    DATEADD(HOUR, 6, GETDATE()),
    100,
    95,
    'Boarding'
);
GO

-- =========================================================
-- Synthetic Demo Passengers
-- =========================================================

INSERT INTO Passengers
(
    PassportNumber,
    FullName,
    Email,
    PhoneNumber
)
VALUES
(
    'DEMO10001',
    'Alex Morgan',
    'alex.morgan@example.com',
    '0000000001'
),
(
    'DEMO10002',
    'Jordan Lee',
    'jordan.lee@example.com',
    '0000000002'
),
(
    'DEMO10003',
    'Taylor Reed',
    'taylor.reed@example.com',
    '0000000003'
);
GO

-- =========================================================
-- Demo Bookings
-- =========================================================

INSERT INTO Bookings
(
    FlightID,
    PassengerID,
    SeatNumber,
    Status
)
VALUES
(1, 1, '12A', 'CheckedIn'),
(1, 2, '14B', 'Confirmed'),
(2, 3, '05C', 'Confirmed');
GO

-- =========================================================
-- User accounts are created by the SkyFlow application.
-- Passwords must be hashed by the application before storage.
-- Do not store plaintext passwords in this script.
-- =========================================================