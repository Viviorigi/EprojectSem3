use master
go
drop database EprojectSem3
go
create database EprojectSem3
go
use EprojectSem3
go

-- Bảng Regions (Không phụ thuộc bảng khác)
CREATE TABLE Regions (
    RegionId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
);

-- Bảng Cities (Phụ thuộc Regions)
CREATE TABLE Cities (
    CityId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    RegionId INT NOT NULL,
    FOREIGN KEY (RegionId) REFERENCES Regions(RegionId)
);

-- Bảng Subscriptions (Không phụ thuộc bảng khác)
CREATE TABLE Subscriptions (
    SubscriptionId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Duration INT NOT NULL,
    MaxAds INT NOT NULL,
    IsAgent BIT NOT NULL
);

-- Bảng Users (Phụ thuộc Subscriptions)
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(15),
    Address VARCHAR(255),
    Role TINYINT NOT NULL,
    Status TINYINT NOT NULL,
    SubscriptionId INT,
	ResetPasswordToken NVARCHAR(255),
	ResetTokenExpiration DATETIME,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (SubscriptionId) REFERENCES Subscriptions(SubscriptionId)
);

-- Bảng Categories (Không phụ thuộc bảng khác)
CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Description TEXT
);

-- Bảng Listings (Phụ thuộc Users, Categories, Cities)
CREATE TABLE Listings (
    ListingId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Description TEXT,
    Price DECIMAL(10,2) NOT NULL,
    CategoryId INT NOT NULL,
    UserId INT NOT NULL,
    CityId INT NOT NULL,
    Status TINYINT NOT NULL,
	image varchar(255) not null,
    Priority TINYINT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (CityId) REFERENCES Cities(CityId)
);

-- Bảng Images (Phụ thuộc Listings)
CREATE TABLE Images (
    ImageId INT PRIMARY KEY IDENTITY(1,1),
    ListingId INT NOT NULL,
    ImagePath NVARCHAR(255) NOT NULL,
    FOREIGN KEY (ListingId) REFERENCES Listings(ListingId)
);

-- Bảng UserSubscriptions (Phụ thuộc Users, Subscriptions)
CREATE TABLE UserSubscriptions (
    UserSubscriptionId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    SubscriptionId INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (SubscriptionId) REFERENCES Subscriptions(SubscriptionId)
);

CREATE TABLE Contacts (
	ContactId INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(255) NOT NULL,
	Email VARCHAR(255) NOT NULL,
	Subject VARCHAR(255) NOT NULL,
	Content TEXT
)