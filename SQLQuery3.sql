-- Markingmyname. (n.d.). CREATE DATABASE (Transact-SQL) - SQL Server. Microsoft Learn. https://learn.microsoft.com/en-us/sql/t-sql/statements/create-database-transact-sql?view=sql-server-ver16&tabs=sqlpool 
-- TABLE CREATION SECTION with validation constraints
CREATE TABLE Venue (
    VenueID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    VenueName VARCHAR(250) NOT NULL,
    VenueLocation VARCHAR(250) NOT NULL,
    VenueCapacity INT NOT NULL CHECK (VenueCapacity > 0),
    VenueImage VARCHAR(MAX) NOT NULL,
    CONSTRAINT UQ_VenueName_Location UNIQUE (VenueName, VenueLocation)
);
GO

CREATE TABLE Event (
    EventID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    EventDate DATE NOT NULL,
    EventDescription VARCHAR(500) NOT NULL,
    CONSTRAINT CHK_EventDate CHECK (EventDate >= GETDATE())
);
GO

CREATE TABLE Booking (
    BookingID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    EventID INT NOT NULL,
    VenueID INT NOT NULL,
    BookingDate DATE NOT NULL,
    FOREIGN KEY (EventID) REFERENCES Event(EventID) ON DELETE CASCADE,
    FOREIGN KEY (VenueID) REFERENCES Venue(VenueID) ON DELETE CASCADE,
    CONSTRAINT CHK_UniqueBooking UNIQUE (EventID, VenueID, BookingDate),
    CONSTRAINT CHK_BookingDate CHECK (BookingDate >= GETDATE())
);
GO

-- Markingmyname. (n.d.-b). CREATE TABLE (Transact-SQL) - SQL Server. Microsoft Learn. https://learn.microsoft.com/en-us/sql/t-sql/statements/create-table-transact-sql?view=sql-server-ver16
-- Insert data into Venue (placeholder URLs for Azure Blob Storage)
INSERT INTO Venue (VenueName, VenueLocation, VenueCapacity, VenueImage)
VALUES
('Sonto', 'Trends Lounge Mbombela', 500, 'https://yourazureaccount.blob.core.windows.net/container/sonto1.jpg'),
('Sonto 2', 'Industrial Works Centurion', 1000, 'https://yourazureaccount.blob.core.windows.net/container/sonto2.jpg'),
('Sonto 3', 'The Playground, Braamfontein', 750, 'https://yourazureaccount.blob.core.windows.net/container/sonto3.jpg');
GO
-- WilliamDAssafMSFT. (n.d.). INSERT (Transact-SQL) - SQL Server. Microsoft Learn. https://learn.microsoft.com/en-us/sql/t-sql/statements/insert-transact-sql?view=sql-server-ver16
-- Insert data into Event
INSERT INTO Event (EventDate, EventDescription)
VALUES
('2025-06-12', 'Fashion Show'),
('2025-07-20', 'Outdoor Music Festival'),
('2025-08-25', 'Listening Party');
GO
-- WilliamDAssafMSFT. (n.d.). INSERT (Transact-SQL) - SQL Server. Microsoft Learn. https://learn.microsoft.com/en-us/sql/t-sql/statements/insert-transact-sql?view=sql-server-ver16
-- Insert data into Booking
INSERT INTO Booking (EventID, VenueID, BookingDate)
VALUES
(1, 1, '2025-06-12'),
(2, 2, '2025-07-20'),
(3, 3, '2025-08-25');
GO
-- Markingmyname. (n.d.-c). CREATE VIEW (Transact-SQL) - SQL Server. Microsoft Learn. https://learn.microsoft.com/en-us/sql/t-sql/statements/create-view-transact-sql?view=sql-server-ver16
-- VIEW for enhanced booking display
CREATE VIEW BookingView AS
SELECT
    b.BookingID,
    v.VenueName,
    v.VenueLocation,
    v.VenueCapacity,
    e.EventDate,
    e.EventDescription,
    b.BookingDate
FROM Booking b
JOIN Venue v ON b.VenueID = v.VenueID
JOIN Event e ON b.EventID = e.EventID;
GO
-- Markingmyname. (n.d.-b). CREATE PROCEDURE (Transact-SQL) - SQL Server. Microsoft Learn. https://learn.microsoft.com/en-us/sql/t-sql/statements/create-procedure-transact-sql?view=sql-server-ver16
-- Stored procedure for search functionality
CREATE PROCEDURE SearchBookings
    @SearchTerm VARCHAR(250)
AS
BEGIN
    SELECT * FROM BookingView
    WHERE BookingID LIKE '%' + @SearchTerm + '%'
       OR EventDescription LIKE '%' + @SearchTerm + '%'
    ORDER BY BookingDate;
END;
GO

-- TABLE MANIPULATION SECTION
SELECT * FROM Venue;
SELECT * FROM Event;
SELECT * FROM Booking;
SELECT * FROM BookingView;
EXEC SearchBookings @SearchTerm = 'Fashion';
GO