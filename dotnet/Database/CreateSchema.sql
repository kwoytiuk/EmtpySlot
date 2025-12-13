-- EmptySlot Database Schema for SQL Server
-- Run this script to create the database schema

-- Create database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EmptySlotDb')
BEGIN
    CREATE DATABASE EmptySlotDb;
END
GO

USE EmptySlotDb;
GO

-- Profiles table
CREATE TABLE Profiles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserType NVARCHAR(20) NOT NULL DEFAULT 'Customer',
    FullName NVARCHAR(255) NULL,
    Phone NVARCHAR(50) NULL,
    AvatarUrl NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT CHK_UserType CHECK (UserType IN ('Customer', 'Provider', 'Admin'))
);

-- Service Categories table
CREATE TABLE ServiceCategories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Slug NVARCHAR(100) NOT NULL UNIQUE,
    Icon NVARCHAR(100) NULL,
    Description NVARCHAR(MAX) NULL,
    ParentId UNIQUEIDENTIFIER NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_ServiceCategories_Parent FOREIGN KEY (ParentId)
        REFERENCES ServiceCategories(Id) ON DELETE NO ACTION
);

-- Providers table
CREATE TABLE Providers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    BusinessName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Phone NVARCHAR(50) NULL,
    Email NVARCHAR(255) NULL,
    Website NVARCHAR(500) NULL,
    LogoUrl NVARCHAR(500) NULL,
    CoverImageUrl NVARCHAR(500) NULL,
    Verified BIT NOT NULL DEFAULT 0,
    RatingAverage DECIMAL(3,2) NOT NULL DEFAULT 0,
    RatingCount INT NOT NULL DEFAULT 0,
    CancellationPolicy NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Providers_User FOREIGN KEY (UserId)
        REFERENCES Profiles(Id) ON DELETE CASCADE
);

-- Provider Locations table
CREATE TABLE ProviderLocations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProviderId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    AddressLine1 NVARCHAR(255) NOT NULL,
    AddressLine2 NVARCHAR(255) NULL,
    City NVARCHAR(100) NOT NULL,
    StateProvince NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(100) NOT NULL DEFAULT 'USA',
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL,
    Phone NVARCHAR(50) NULL,
    IsPrimary BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_ProviderLocations_Provider FOREIGN KEY (ProviderId)
        REFERENCES Providers(Id) ON DELETE CASCADE
);

-- Services table
CREATE TABLE Services (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProviderId UNIQUEIDENTIFIER NOT NULL,
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DurationMinutes INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    DepositRequired DECIMAL(10,2) NULL,
    ImageUrl NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Services_Provider FOREIGN KEY (ProviderId)
        REFERENCES Providers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Services_Category FOREIGN KEY (CategoryId)
        REFERENCES ServiceCategories(Id) ON DELETE NO ACTION
);

-- Staff Members table
CREATE TABLE StaffMembers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProviderId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NULL,
    Phone NVARCHAR(50) NULL,
    AvatarUrl NVARCHAR(500) NULL,
    Bio NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_StaffMembers_Provider FOREIGN KEY (ProviderId)
        REFERENCES Providers(Id) ON DELETE CASCADE
);

-- Appointments table
CREATE TABLE Appointments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BookingReference NVARCHAR(50) NOT NULL UNIQUE,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    ProviderId UNIQUEIDENTIFIER NOT NULL,
    LocationId UNIQUEIDENTIFIER NOT NULL,
    ServiceId UNIQUEIDENTIFIER NOT NULL,
    StaffId UNIQUEIDENTIFIER NULL,
    AppointmentDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    Price DECIMAL(10,2) NOT NULL,
    DepositPaid DECIMAL(10,2) NULL,
    PaymentStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    PaymentIntentId NVARCHAR(255) NULL,
    CustomerNotes NVARCHAR(MAX) NULL,
    ProviderNotes NVARCHAR(MAX) NULL,
    CancelledBy UNIQUEIDENTIFIER NULL,
    CancelledAt DATETIME2 NULL,
    CancellationReason NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Appointments_Customer FOREIGN KEY (CustomerId)
        REFERENCES Profiles(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Appointments_Provider FOREIGN KEY (ProviderId)
        REFERENCES Providers(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Appointments_Location FOREIGN KEY (LocationId)
        REFERENCES ProviderLocations(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Appointments_Service FOREIGN KEY (ServiceId)
        REFERENCES Services(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Appointments_Staff FOREIGN KEY (StaffId)
        REFERENCES StaffMembers(Id) ON DELETE SET NULL,
    CONSTRAINT CHK_Status CHECK (Status IN ('Pending', 'Confirmed', 'Completed', 'Cancelled', 'NoShow')),
    CONSTRAINT CHK_PaymentStatus CHECK (PaymentStatus IN ('Pending', 'Paid', 'Refunded'))
);

-- Reviews table
CREATE TABLE Reviews (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    ProviderId UNIQUEIDENTIFIER NOT NULL,
    AppointmentId UNIQUEIDENTIFIER NULL,
    Rating INT NOT NULL,
    Comment NVARCHAR(MAX) NULL,
    Response NVARCHAR(MAX) NULL,
    RespondedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Reviews_Customer FOREIGN KEY (CustomerId)
        REFERENCES Profiles(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Reviews_Provider FOREIGN KEY (ProviderId)
        REFERENCES Providers(Id) ON DELETE CASCADE,
    CONSTRAINT CHK_Rating CHECK (Rating >= 1 AND Rating <= 5)
);

-- Favorites table
CREATE TABLE Favorites (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    ProviderId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Favorites_Customer FOREIGN KEY (CustomerId)
        REFERENCES Profiles(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Favorites_CustomerProvider UNIQUE (CustomerId, ProviderId)
);

-- Create indexes for better performance
CREATE INDEX IX_Providers_UserId ON Providers(UserId);
CREATE INDEX IX_Providers_Verified ON Providers(Verified);
CREATE INDEX IX_ProviderLocations_ProviderId ON ProviderLocations(ProviderId);
CREATE INDEX IX_Services_ProviderId ON Services(ProviderId);
CREATE INDEX IX_Services_CategoryId ON Services(CategoryId);
CREATE INDEX IX_Appointments_CustomerId ON Appointments(CustomerId);
CREATE INDEX IX_Appointments_ProviderId ON Appointments(ProviderId);
CREATE INDEX IX_Appointments_Date ON Appointments(AppointmentDate);
CREATE INDEX IX_Reviews_ProviderId ON Reviews(ProviderId);
CREATE INDEX IX_Favorites_CustomerId ON Favorites(CustomerId);

PRINT 'Database schema created successfully!';
GO
