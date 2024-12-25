use master
go
drop database EprojectSem3
go
create database EprojectSem3
go
use EprojectSem3
go

CREATE TABLE Regions (
    RegionId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
);

CREATE TABLE Cities (
    CityId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    RegionId INT NOT NULL,
    FOREIGN KEY (RegionId) REFERENCES Regions(RegionId)
);

CREATE TABLE Subscriptions (
    SubscriptionId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Duration INT NOT NULL,
    MaxAds INT NOT NULL,
    IsAgent BIT NOT NULL
);

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(15),
    Address NVARCHAR(255),
    Role TINYINT NOT NULL,
    Status TINYINT NOT NULL,
    SubscriptionId INT,
	Image varchar(255) NULL,
	ResetPasswordToken NVARCHAR(255),
	ResetTokenExpiration DATETIME,
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    FOREIGN KEY (SubscriptionId) REFERENCES Subscriptions(SubscriptionId)
);

CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
	Status TINYINT NOT NULL,
    Description TEXT
);

CREATE TABLE Listings (
    ListingId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Description TEXT,
    Price float NOT NULL,
    CategoryId INT NOT NULL,
    UserId INT NOT NULL,
    CityId INT NOT NULL,
    Status TINYINT NOT NULL,
	image varchar(255) NOT NULL,
    Priority TINYINT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (CityId) REFERENCES Cities(CityId)
);
ALTER TABLE Listings
ADD ContactViaForm TINYINT DEFAULT 0;

CREATE TABLE Images (
    ImageId INT PRIMARY KEY IDENTITY(1,1),
    ListingId INT NOT NULL,
    ImagePath NVARCHAR(255) NOT NULL,
    FOREIGN KEY (ListingId) REFERENCES Listings(ListingId)
);

CREATE TABLE UserSubscriptions (
    UserSubscriptionId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    SubscriptionId INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (SubscriptionId) REFERENCES Subscriptions(SubscriptionId)
);

CREATE TABLE Transactions
(
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,  -- Auto-increment primary key
    UserId INT NOT NULL,  -- Foreign key to User table
    SubscriptionId INT NOT NULL,  -- Foreign key to Subscription table
    Amount DECIMAL(18,2) NOT NULL,  -- Payment amount with 2 decimal places
    TransactionDate DATETIME ,  -- Date of transaction creation
    PaymentDate DATETIME ,  -- Date of payment confirmation
    IsPaid BIT NOT NULL,  -- Indicates if the payment was successful (1 = true, 0 = false)
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (SubscriptionId) REFERENCES Subscriptions(SubscriptionId)
);

CREATE TABLE Blogs (
    BlogId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Content TEXT NOT NULL,
	Image varchar(255) not null,
    Status TINYINT NOT NULL, -- 0: Draft, 1: Published, 2: Archived
    CreatedAt DATETIME DEFAULT GETDATE(),
);
CREATE TABLE Contacts (
	ContactId INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(255) NOT NULL,
	Email VARCHAR(255) NOT NULL,
	Subject VARCHAR(255) NOT NULL,
	Content TEXT,
	Reply TEXT
);
CREATE TABLE Statisticals (
	StatisticalId INT IDENTITY(1,1) PRIMARY KEY,
	ListingCount INT,
	TransactionCount INT,
	UserCount INT,
	PriceCount DECIMAL(18,2),
	CreatedAt DATETIME
);
CREATE TABLE BookMarks (
	BookMarkId INT IDENTITY(1,1) PRIMARY KEY,
	UserId INT NOT NULL,
	ListingId INT NOT NULL,
	CreatedAt DATETIME
	FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ListingId) REFERENCES Listings(ListingId)
);