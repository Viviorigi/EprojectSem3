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
    Address NVARCHAR(255),
    Role TINYINT NOT NULL,
    Status TINYINT NOT NULL,
    SubscriptionId INT,
	image varchar(255) NULL,
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
	Status TINYINT NOT NULL,
    Description TEXT
);

-- Bảng Listings (Phụ thuộc Users, Categories, Cities)
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
-- Bảng Blogs
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
)

ALTER TABLE Listings
ADD ContactViaForm TINYINT DEFAULT 0;

