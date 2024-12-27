--use master
--go
--drop database EprojectSem3
--go
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
	ContactViaForm TINYINT DEFAULT 0,
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (CityId) REFERENCES Cities(CityId)
);

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
    Status TINYINT NOT NULL,
    CreatedAt DATETIME,
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
	CreatedAt DATETIME,
	FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ListingId) REFERENCES Listings(ListingId)
);
CREATE TABLE Ratings (
    RatingId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ListingId INT NOT NULL,
    RatingValue FLOAT NOT NULL,
    Comment NVARCHAR(MAX),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ListingId) REFERENCES Listings(ListingId)
);

--Insert Data

-- Thêm dữ liệu vào bảng Regions
INSERT INTO Regions (Name) VALUES 
('North America'),
('Europe'),
('East Asia'),
('Southeast Asia');

-- Thêm dữ liệu vào bảng Cities
INSERT INTO Cities (Name, RegionId) VALUES 
-- Cities in North America
('New York', 1),
('Los Angeles', 1),
('Chicago', 1),
('Toronto', 1),
('Vancouver', 1),
('Mexico City', 1),
('Miami', 1),
('San Francisco', 1),

-- Cities in Europe
('London', 2),
('Paris', 2),
('Berlin', 2),
('Madrid', 2),
('Rome', 2),
('Amsterdam', 2),
('Vienna', 2),
('Stockholm', 2),
('Prague', 2),
('Lisbon', 2),

-- Cities in East Asia
('Tokyo', 3),
('Seoul', 3),
('Beijing', 3),
('Shanghai', 3),
('Hong Kong', 3),
('Taipei', 3),
('Osaka', 3),
('Nagoya', 3),

-- Cities in Southeast Asia
('Hanoi', 4),
('Ho Chi Minh City', 4),
('Da Nang', 4),
('Bangkok', 4),
('Jakarta', 4),
('Manila', 4),
('Kuala Lumpur', 4),
('Singapore', 4),
('Yangon', 4),
('Phnom Penh', 4),
('Vientiane', 4),
('Bandar Seri Begawan', 4),
('Davao City', 4),
('Chiang Mai', 4),
('Hue', 4);


-- Thêm dữ liệu vào bảng Subscriptions
INSERT INTO Subscriptions (Name, Price, Duration, MaxAds, IsAgent) VALUES 
-- Monthly Plans (6 gói cá nhân)
('Basic Monthly Plan', 9.99, 30, 3, 0),        -- 3 quảng cáo/tháng
('Standard Monthly Plan', 19.99, 30, 6, 0),   -- 6 quảng cáo/tháng
('Premium Monthly Plan', 29.99, 30, 10, 0),   -- 10 quảng cáo/tháng
('Basic Yearly Plan', 99.99, 365, 30, 0),     -- 30 quảng cáo/năm
('Standard Yearly Plan', 149.99, 365, 60, 0), -- 60 quảng cáo/năm
('Premium Yearly Plan', 199.99, 365, 100, 0), -- 100 quảng cáo/năm

-- Agent Plans (3 gói cho đại lý)
('Agent Starter Monthly Plan', 39.99, 30, 15, 1),    -- 15 quảng cáo/tháng
('Agent Professional Monthly Plan', 69.99, 30, 30, 1), -- 30 quảng cáo/tháng
('Agent Ultimate Yearly Plan', 499.99, 365, 200, 1);  -- 200 quảng cáo/năm

INSERT INTO Users (FullName, Email, Password, PhoneNumber, Address, Role, Status, SubscriptionId, Image, ResetPasswordToken, ResetTokenExpiration, CreatedAt, UpdatedAt) VALUES
--(Saler)
('John Doe', 'johndoe@example.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567890', '123 Main St, City, Country', 0, 1, 1, 'images/users.png', NULL, NULL, '2024-01-01', NULL),
('Jane Smith', 'janesmith@example.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567891', '456 Oak St, City, Country', 0, 1, 2, 'images/users.png', NULL, NULL, '2024-02-01', NULL),
('Michael Brown', 'michaelbrown@example.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567892', '789 Pine St, City, Country', 0, 1, 3, 'images/users.png', NULL, NULL, '2024-03-01', NULL),
('Emily Clark', 'emilyclark@example.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567893', '101 Maple St, City, Country', 0, 1, 4, 'images/users.png', NULL, NULL, '2024-04-01', NULL),
('David Wilson', 'davidwilson@example.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567894', '202 Cedar St, City, Country', 0, 1, 5, 'images/users.png', NULL, NULL, '2024-05-01', NULL),
('Sarah Miller', 'sarahmiller@example.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567895', '303 Birch St, City, Country', 0, 1, 6, 'images/users.png', NULL, NULL, '2024-06-01', NULL),
('William Lee', 'williamlee@example.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567896', '404 Cherry St, City, Country', 0, 1, 1, 'images/users.png', NULL, NULL, '2024-07-01', NULL),
('Olivia Taylor', 'oliviataylor@example.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567897', '505 Walnut St, City, Country', 0, 1, 2, 'images/users.png', NULL, NULL, '2024-08-01', NULL),
('James Harris', 'jamestaylor@example.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567898', '606 Ash St, City, Country', 0, 1, 3, 'images/users.png', NULL, NULL, '2024-09-01', NULL),
('Sophia Martin', 'sophiamartin@example.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567899', '707 Elm St, City, Country', 0, 1, 4, 'images/users.png', NULL, NULL, '2024-10-01', NULL),

--(Agent)
('Alex Carter', 'alexcarter@realestate.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567801', '123 Business St, City, Country', 1, 1, 7, 'images/users.png', NULL, NULL, '2024-01-15', NULL),
('Emma Walker', 'emmawalker@realestate.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567802', '234 Corporate St, City, Country', 1, 1, 8, 'images/users.png', NULL, NULL, '2024-02-15', NULL),
('Michael Scott', 'michaelscott@realestate.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567803', '345 Commerce St, City, Country', 1, 1, 9, 'images/users.png', NULL, NULL, '2024-03-15', NULL),
('Liam Johnson', 'liamjohnson@realestate.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567804', '456 Investment St, City, Country', 1, 1, 7, 'images/users.png', NULL, NULL, '2024-04-15', NULL),
('Ava White', 'avawhite@realestate.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567805', '567 RealEstate St, City, Country', 1, 1, 8, 'images/users.png', NULL, NULL, '2024-05-15', NULL);

--(User not sub)
INSERT INTO Users (FullName, Email, Password, PhoneNumber, Address, Role, Status, Image, ResetPasswordToken, ResetTokenExpiration, CreatedAt, UpdatedAt) VALUES 
	('Sophia Taylor', 'sophiataylor@gmail.com', '$2a$11$IdURuAbLaQokViiInVfhbuNiV5G7SOukWIL3CvtQyKxMko./Q9hMy', '1234567899', '707 Elm St, City, Country', 0, 1, 'images/users.png', NULL, NULL, '2024-10-01', NULL)

INSERT INTO UserSubscriptions (UserId, SubscriptionId, StartDate, EndDate) VALUES
-- Monthly Plans for Salers
--(1, 1, '2024-01-01', '2024-01-31'),  -- John Doe (30 days)
--(2, 2, '2024-02-01', '2024-02-29'),  -- Jane Smith (30 days)
--(3, 3, '2024-03-01', '2024-03-31'),  -- Michael Brown (30 days)
(4, 4, '2024-04-01', '2025-03-31'),  -- Emily Clark (365 days, 1 year)
(5, 5, '2024-05-01', '2025-04-30'),  -- David Wilson (365 days, 1 year)
(6, 6, '2024-06-01', '2025-05-31'),  -- Sarah Miller (365 days, 1 year)
--(7, 1, '2024-07-01', '2024-07-31'),  -- William Lee (30 days)
--(8, 2, '2024-08-01', '2024-08-31'),  -- Olivia Taylor (30 days)
--(9, 3, '2024-09-01', '2024-09-30'),  -- James Harris (30 days)
(10, 4, '2024-10-01', '2025-09-30'), -- Sophia Martin (365 days, 1 year)

-- Monthly Plans for Agents
--(11, 7, '2024-01-15', '2024-02-14'),  -- Alex Carter (30 days)
--(12, 8, '2024-02-15', '2024-03-15'),  -- Emma Walker (30 days)
(13, 9, '2024-03-15', '2025-03-14'),  -- Michael Scott (365 days, 1 year)
--(14, 7, '2024-04-15', '2024-05-15'),  -- Liam Johnson (30 days)
--(15, 8, '2024-05-15', '2024-06-14');  -- Ava White (30 days)

INSERT INTO Transactions (UserId, SubscriptionId, Amount, TransactionDate, PaymentDate, IsPaid) VALUES
-- Monthly Plans for Salers
(1, 1, 9.99, '2024-01-01', '2024-01-01', 1),  -- John Doe (Price: 9.99)
(2, 2, 19.99, '2024-02-01', '2024-02-01', 1),  -- Jane Smith (Price: 19.99)
(3, 3, 29.99, '2024-03-01', '2024-03-01', 1),  -- Michael Brown (Price: 29.99)
(4, 4, 99.99, '2024-04-01', '2024-04-01', 1),  -- Emily Clark (Price: 99.99)
(5, 5, 149.99, '2024-05-01', '2024-05-01', 1), -- David Wilson (Price: 149.99)
(6, 6, 199.99, '2024-06-01', '2024-06-01', 1), -- Sarah Miller (Price: 199.99)
(7, 1, 9.99, '2024-07-01', '2024-07-01', 1),  -- William Lee (Price: 9.99)
(8, 2, 19.99, '2024-08-01', '2024-08-01', 1),  -- Olivia Taylor (Price: 19.99)
(9, 3, 29.99, '2024-09-01', '2024-09-01', 1),  -- James Harris (Price: 29.99)
(10, 4, 99.99, '2024-10-01', '2024-10-01', 1), -- Sophia Martin (Price: 99.99)

-- Monthly Plans for Agents
(11, 7, 39.99, '2024-01-15', '2024-01-15', 1),  -- Alex Carter (Price: 39.99)
(12, 8, 69.99, '2024-02-15', '2024-02-15', 1),  -- Emma Walker (Price: 69.99)
(13, 9, 499.99, '2024-03-15', '2024-03-15', 1), -- Michael Scott (Price: 499.99)
(14, 7, 39.99, '2024-04-15', '2024-04-15', 1),  -- Liam Johnson (Price: 39.99)
(15, 8, 69.99, '2024-05-15', '2024-05-15', 1);  -- Ava White (Price: 69.99)

INSERT INTO Categories (Name, Status, Description) VALUES
('Houses for Sale', 1, 'Properties including detached houses, semi-detached houses, townhouses, and villas for sale.'),
('Apartments for Sale', 1, 'Residential apartments and flats available for sale in various locations.'),
('Commercial Properties', 1, 'Commercial real estate including office spaces, retail shops, and business premises for sale or lease.'),
('Land for Sale', 1, 'Plots of land available for purchase for residential, commercial, or agricultural purposes.'),
('Houses for Rent', 1, 'Properties including detached houses, semi-detached houses, townhouses, and villas available for rent.'),
('Apartments for Rent', 1, 'Residential apartments and flats available for rent in different locations.'),
('Luxury Properties', 1, 'High-end residential and commercial properties with premium features and amenities.'),
('Vacation Homes', 1, 'Vacation rental properties such as holiday homes, cottages, and beachfront villas for short-term stay.');

INSERT INTO Listings (Title, Description, Price, CategoryId, UserId, CityId, Status, Image, Priority, ContactViaForm, CreatedAt, UpdatedAt) VALUES
-- Listings by Salers
('Cozy 3-Bedroom House in New York', 'A beautiful 3-bedroom house with a garden and parking space, located in a quiet neighborhood.', 350000, 1, 4, 1, 1, 'images/image1v1.jpeg', 0, 1, '2024-01-10', NULL),
('Modern Apartment in Downtown Los Angeles', 'Luxury 2-bedroom apartment with modern amenities and stunning city views.', 550000, 2, 5, 2, 1, 'images/image2v1.jpeg', 0, 1, '2024-01-15', NULL),
('Commercial Office Space in Chicago', 'Prime office space in a central business district, ideal for startups and established businesses.', 1200000, 3, 3, 3, 1, 'images/image3v1.jpeg', 0, 1, '2024-01-20', NULL),
('Land for Sale in Toronto', 'A 10-acre plot of land suitable for residential development, located near major amenities.', 800000, 4, 6, 4, 1, 'images/image4v1.jpeg', 0, 1, '2024-01-25', NULL),
('Spacious House for Rent in Vancouver', 'Fully furnished 4-bedroom house available for long-term rental.', 2500, 5, 10, 5, 1, 'images/image5v1.jpeg', 0, 1, '2024-01-30', NULL),
('Affordable Apartment for Rent in Miami', '1-bedroom apartment located near public transport and shopping areas.', 1200, 6, 4, 6, 1, 'images/image6v1.jpeg', 0, 1, '2024-02-05', NULL),
('Luxury Villa in Paris', 'High-end villa with 5 bedrooms, a swimming pool, and a private garden.', 4500000, 7, 5, 10, 1, 'images/image7v1.jpeg', 0, 1, '2024-02-10', NULL),
('Vacation Home in Bangkok', 'Beachfront villa with 3 bedrooms and stunning sea views.', 300000, 8, 6, 29, 1, 'images/image8v1.jpeg', 0, 0, '2024-02-15', NULL),
('Modern Apartment in Berlin', 'Stylish 3-bedroom apartment in a central location, close to shopping and entertainment.', 600000, 2, 10, 11, 1, 'images/image9v1.jpeg', 0, 1, '2024-02-20', NULL),
('Farm Land for Sale in Hanoi', 'Large agricultural land ideal for farming or investment.', 200000, 4, 4, 31, 1, 'images/image10v1.jpeg', 0, 1, '2024-02-25', NULL),
('Spacious House in Lisbon', 'Beautiful 4-bedroom house with a large backyard and modern amenities.', 400000, 1, 5, 19, 1, 'images/image11v1.jpeg', 0, 1, '2024-03-01', NULL),
('Affordable Apartment in Toronto', '2-bedroom apartment with convenient access to public transport and schools.', 350000, 2, 6, 4, 1, 'images/image12v1.jpeg', 0, 1, '2024-03-05', NULL),
('Retail Space in London', 'A spacious retail unit located in a bustling shopping district.', 1000000, 3, 10, 9, 1, 'images/image13v1.jpeg', 0, 1, '2024-03-10', NULL),
('Farm Land in Da Nang', '5 hectares of prime farmland suitable for agricultural projects.', 150000, 4, 4, 33, 1, 'images/image14v1.jpeg', 0, 1, '2024-03-15', NULL),
('Luxury Penthouse in Tokyo', 'Exclusive penthouse with panoramic views and state-of-the-art facilities.', 9000000, 7, 5, 21, 1, 'images/image15v1.jpeg', 0, 1, '2024-03-20', NULL),
('Charming House in Vienna', '3-bedroom house with a classic European design and modern upgrades.', 450000, 1, 6, 13, 1, 'images/image16v1.jpeg', 0, 1, '2024-03-25', NULL),
('Modern Apartment in Madrid', '2-bedroom apartment with high-end finishes and central air.', 550000, 2, 10, 12, 1, 'images/image17v1.jpeg', 0, 1, '2024-03-30', NULL),
('Vacation Home in Singapore', 'Luxury villa available for short-term vacation rentals, includes a private pool.', 800000, 8, 4, 28, 1, 'images/image18v1.jpeg', 1, 1, '2024-04-01', NULL),
('Commercial Space in Seoul', 'Prime commercial property located in the heart of the city.', 2000000, 3, 5, 22, 1, 'images/image19v1.jpeg', 1, 1, '2024-04-05', NULL),
('Affordable Apartment in Ho Chi Minh City', 'Compact 1-bedroom apartment in a busy neighborhood.', 100000, 2, 6, 30, 1, 'images/image20v1.jpeg', 1, 1, '2024-04-10', NULL),

-- Listings by Agents
('Farm Land for Sale in Yangon', 'Affordable agricultural land in a growing region.', 120000, 4, 13, 34, 1, 'images/image21v1.jpeg', 1, 0, '2024-04-15', NULL),
('Luxury Apartment in Amsterdam', 'High-end apartment in a prime location with modern features.', 750000, 2, 13, 16, 1, 'images/image22v1.jpeg', 1, 0, '2024-04-20', NULL),
('Vacation Villa in Manila', 'Beautiful villa available for vacation stays, close to the beach.', 300000, 8, 13, 27, 1, 'images/image23v1.jpeg', 0, 1, '2024-04-25', NULL),
('Cozy House in Stockholm', '2-bedroom house in a quiet and scenic area.', 300000, 1, 13, 14, 1, 'images/image24v1.jpeg', 0, 0, '2024-05-01', NULL),
('Office Space in Hong Kong', 'Modern office building with ample parking and high visibility.', 5000000, 3, 13, 23, 1, 'images/image25v1.jpeg', 1, 1, '2024-05-05', NULL);


INSERT INTO Images (ListingId, ImagePath) VALUES
(1, 'images/image1v2.jpeg'),
(1, 'images/image1v3.jpeg'),
(1, 'images/image1v4.jpeg'),
(2, 'images/image2v2.jpeg'),
(2, 'images/image2v3.jpeg'),
(2, 'images/image2v4.jpeg'),
(3, 'images/image3v2.jpeg'),
(3, 'images/image3v3.jpeg'),
(3, 'images/image3v4.jpeg'),
(4, 'images/image4v2.jpeg'),
(4, 'images/image4v3.jpeg'),
(4, 'images/image4v4.jpeg'),
(5, 'images/image5v2.jpeg'),
(5, 'images/image5v3.jpeg'),
(5, 'images/image5v4.jpeg'),
(6, 'images/image6v2.jpeg'),
(6, 'images/image6v3.jpeg'),
(6, 'images/image6v4.jpeg'),
(7, 'images/image7v2.jpeg'),
(7, 'images/image7v3.jpeg'),
(7, 'images/image7v4.jpeg'),
(8, 'images/image8v2.jpeg'),
(8, 'images/image8v3.jpeg'),
(8, 'images/image8v4.jpeg'),
(9, 'images/image9v2.jpeg'),
(9, 'images/image9v3.jpeg'),
(9, 'images/image9v4.jpeg'),
(10, 'images/image10v2.jpeg'),
(10, 'images/image10v3.jpeg'),
(10, 'images/image10v4.jpeg'),
(11, 'images/image11v2.jpeg'),
(11, 'images/image11v3.jpeg'),
(11, 'images/image11v4.jpeg'),
(12, 'images/image12v2.jpeg'),
(12, 'images/image12v3.jpeg'),
(12, 'images/image12v4.jpeg'),
(13, 'images/image13v2.jpeg'),
(13, 'images/image13v3.jpeg'),
(13, 'images/image13v4.jpeg'),
(14, 'images/image14v2.jpeg'),
(14, 'images/image14v3.jpeg'),
(14, 'images/image14v4.jpeg'),
(15, 'images/image15v2.jpeg'),
(15, 'images/image15v3.jpeg'),
(15, 'images/image15v4.jpeg'),
(16, 'images/image16v2.jpeg'),
(16, 'images/image16v3.jpeg'),
(16, 'images/image16v4.jpeg'),
(17, 'images/image17v2.jpeg'),
(17, 'images/image17v3.jpeg'),
(17, 'images/image17v4.jpeg'),
(18, 'images/image18v2.jpeg'),
(18, 'images/image18v3.jpeg'),
(18, 'images/image18v4.jpeg'),
(19, 'images/image19v2.jpeg'),
(19, 'images/image19v3.jpeg'),
(19, 'images/image19v4.jpeg'),
(20, 'images/image20v2.jpeg'),
(20, 'images/image20v3.jpeg'),
(20, 'images/image20v4.jpeg'),
(21, 'images/image21v2.jpeg'),
(21, 'images/image21v3.jpeg'),
(21, 'images/image21v4.jpeg'),
(22, 'images/image22v2.jpeg'),
(22, 'images/image22v3.jpeg'),
(22, 'images/image22v4.jpeg'),
(23, 'images/image23v2.jpeg'),
(23, 'images/image23v3.jpeg'),
(23, 'images/image23v4.jpeg'),
(24, 'images/image24v2.jpeg'),
(24, 'images/image24v3.jpeg'),
(24, 'images/image24v4.jpeg'),
(25, 'images/image25v2.jpeg'),
(25, 'images/image25v3.jpeg'),
(25, 'images/image25v4.jpeg');

INSERT INTO Blogs (Title, Content, Image, Status, CreatedAt)
VALUES 
('First-time Home Buying Tips', 'Buying a home for the first time can be a huge challenge. In this article, we share important tips and advice when choosing and purchasing a property. You will need to consider factors such as location, property value, and legal procedures.', 'images/blogs/blog1.jpeg', 1, '2024-12-25 10:00:00'),
('Top 5 Best Real Estate Investment Areas in 2024', 'In 2024, the real estate market is experiencing many fluctuations. Here are the top 5 areas predicted to offer high returns for investors. Read this article to learn more about these areas.', 'images/blogs/blog2.jpeg', 1, '2024-12-20 08:30:00'),
('Luxury Condo Projects in Ho Chi Minh City', 'Ho Chi Minh City is home to many luxury condo projects. This article introduces the most noteworthy projects this year, helping you find the perfect option for modern living.', 'images/blogs/blog3.jpeg', 1, '2024-12-18 14:45:00'),
('Common Mistakes to Avoid When Renting a House', 'Renting a house is a big decision, and it’s easy to make some common mistakes. Let’s explore the most frequent mistakes renters make and how to avoid them.', 'images/blogs/blog4.jpeg', 1, '2024-12-15 16:00:00'),
('Vacation Real Estate: Investment Opportunities in the Near Future', 'Vacation real estate is becoming an increasingly attractive investment trend. Explore the potential of this type of property and discover the best locations for investment in Vietnam.', 'images/blogs/blog5.jpeg', 1, '2024-12-10 09:30:00');

-- Chèn dữ liệu vào bảng Contacts
INSERT INTO Contacts (Name, Email, Subject, Content, Reply) VALUES
('John Doe', 'johndoe@example.com', 'Inquiry about property pricing', 'Hello, I am interested in learning more about the pricing of the properties you offer. Could you provide more details?', 'Thank you for reaching out! We will send you the pricing information shortly.'),
('Michael Brown', 'michaelbrown@example.com', 'Request for property details', 'Hi, I would like more information about the available properties in the city center. Can you provide some details?', 'Thank you for your interest! We will send you the property details as soon as possible.'),
('Sarah Miller', 'sarahmiller@example.com', 'Property rental inquiry', 'I am looking for a rental property in your area. Can you provide me with a list of available rental options?', 'Thank you for your inquiry. We will send you a list of available rental properties shortly.'),
('Alex Carter', 'alexcarter@realestate.com', 'Real estate investment advice', 'Hello, I am looking for some advice regarding real estate investments in the area. Can you help me out?', 'Thank you for your inquiry. We would be happy to discuss investment opportunities with you.'),
('Emma Walker', 'emmawalker@realestate.com', 'Commercial property for rent', 'I am interested in renting commercial space in your portfolio. Could you provide details on availability and pricing?', 'Thank you for your interest in our commercial properties. We will send you more information shortly.');

INSERT INTO Statisticals (ListingCount, TransactionCount, UserCount, PriceCount, CreatedAt)
VALUES
-- Tháng 1
(25, 17, 5, 340.60, '2024-01-22'),
(13, 20, 15, 480.32, '2024-01-07'),
(16, 21, 6, 180.32, '2024-01-11'),
(24, 4, 10, 45.75, '2024-01-28'),
(44, 22, 12, 44.17, '2024-01-26'),

-- Tháng 2
(35, 12, 9, 220.45, '2024-02-14'),
(47, 21, 17, 410.67, '2024-02-09'),
(14, 9, 4, 123.33, '2024-02-27'),
(39, 13, 9, 317.50, '2024-02-18'),
(20, 8, 5, 104.78, '2024-02-23'),

-- Tháng 3
(31, 11, 7, 274.80, '2024-03-05'),
(19, 6, 3, 155.60, '2024-03-15'),
(48, 24, 21, 445.90, '2024-03-22'),
(27, 14, 9, 234.00, '2024-03-11'),
(36, 17, 11, 317.10, '2024-03-30'),

-- Tháng 4
(20, 10, 6, 198.45, '2024-04-12'),
(39, 22, 15, 375.67, '2024-04-27'),
(17, 8, 4, 144.12, '2024-04-03'),
(26, 15, 9, 278.00, '2024-04-18'),
(31, 14, 11, 313.50, '2024-04-22'),

-- Tháng 5
(42, 21, 18, 401.78, '2024-05-10'),
(24, 12, 7, 214.20, '2024-05-21'),
(36, 17, 12, 309.88, '2024-05-30'),
(28, 13, 8, 250.34, '2024-05-15'),
(33, 14, 10, 287.22, '2024-05-05'),

-- Tháng 6
(30, 14, 10, 269.50, '2024-06-18'),
(22, 9, 5, 198.67, '2024-06-10'),
(47, 22, 20, 423.80, '2024-06-23'),
(36, 16, 13, 315.25, '2024-06-12'),
(41, 19, 17, 390.10, '2024-06-05'),

-- Tháng 7
(23, 11, 8, 214.78, '2024-07-14'),
(40, 20, 15, 376.45, '2024-07-22'),
(19, 8, 6, 155.33, '2024-07-03'),
(32, 14, 10, 284.88, '2024-07-28'),
(35, 18, 13, 308.90, '2024-07-16'),

-- Tháng 8
(38, 20, 16, 378.67, '2024-08-09'),
(41, 19, 17, 392.78, '2024-08-15'),
(27, 13, 9, 256.22, '2024-08-23'),
(49, 25, 21, 310.45, '2024-08-30'),

-- Tháng 9
(35, 18, 14, 341.88, '2024-09-05'),
(29, 15, 11, 289.75, '2024-09-14'),
(42, 22, 17, 367.20, '2024-09-25'),
(33, 17, 12, 310.55, '2024-09-12'),

-- Tháng 10
(28, 14, 10, 275.10, '2024-10-04'),
(39, 19, 15, 352.45, '2024-10-20'),
(30, 16, 12, 290.33, '2024-10-15'),
(24, 12, 8, 240.67, '2024-10-08'),

-- Tháng 11
(32, 15, 12, 310.78, '2024-11-05'),
(40, 20, 17, 395.12, '2024-11-18'),
(25, 12, 9, 230.89, '2024-11-12'),
(38, 18, 15, 378.67, '2024-11-25');

-- Thêm dữ liệu mẫu
INSERT INTO BookMarks (UserId, ListingId, CreatedAt)
VALUES 
    (1, 1, '2024-12-20 14:30:00'),
    (2, 2, '2024-12-21 09:15:00'),
    (3, 3, '2024-12-22 11:45:00'),
    (4, 4, '2024-12-23 08:00:00'),
    (5, 5, '2024-12-24 17:20:00'),
    (6, 6, '2024-12-25 12:10:00'),
    (7, 7, '2024-12-26 10:05:00'),
    (8, 8, '2024-12-27 15:30:00'),
    (9, 9, '2024-12-28 18:45:00'),
    (10, 10, '2024-12-29 08:20:00'),
    (11, 11, '2024-12-30 09:30:00'),
    (12, 12, '2024-12-31 11:50:00'),
    (13, 13, '2025-01-01 14:10:00'),
	(13, 16, '2025-01-01 14:10:00'),
	(13, 17, '2025-01-01 14:10:00'),
	(13, 18, '2025-01-01 14:10:00'),
    (14, 14, '2025-01-02 13:30:00'),
    (15, 15, '2025-01-03 16:25:00');

-- Thêm dữ liệu mẫu cho bảng Ratings
INSERT INTO Ratings (UserId, ListingId, RatingValue, Comment, CreatedAt)
VALUES
    (1, 1, 4.5, N'Great place! Highly recommend.', '2024-12-20 14:30:00'),
    (2, 2, 3.0, N'It was okay, could be better.', '2024-12-21 10:15:00'),
    (3, 3, 5.0, N'Perfect experience, will come back again!', '2024-12-22 09:00:00'),
    (4, 4, 2.5, N'Too expensive for the quality.', '2024-12-23 16:45:00'),
    (5, 5, 4.0, N'Good overall, but a bit noisy.', '2024-12-24 18:20:00'),
    (6, 6, 3.5, N'Average service, but friendly staff.', '2024-12-25 11:00:00'),
    (7, 7, 5.0, N'Amazing experience! Highly satisfied.', '2024-12-26 13:30:00'),
    (8, 8, 4.2, N'Nice place, but could improve cleanliness.', '2024-12-27 15:10:00'),
    (9, 9, 3.8, N'Decent value for the price.', '2024-12-28 17:45:00'),
    (10, 10, 4.8, N'Exceptional service and location.', '2024-12-29 19:15:00'),
    (11, 11, 2.0, N'Not worth the money, very disappointing.', '2024-12-30 08:00:00'),
    (12, 12, 4.0, N'Good choice for a short stay.', '2024-12-31 10:30:00'),
    (13, 13, 4.6, N'Great amenities and friendly staff.', '2025-01-01 14:20:00'),
    (14, 14, 3.9, N'Comfortable stay, but room was small.', '2025-01-02 09:45:00'),
    (15, 15, 5.0, N'Absolutely loved everything about it!', '2025-01-03 18:00:00'),
    (1, 16, 4.7, N'Excellent place with great service.', '2025-01-04 09:00:00'),
    (2, 17, 3.2, N'Good location, but overpriced.', '2025-01-05 11:30:00'),
    (3, 18, 5.0, N'The best experience I have ever had!', '2025-01-06 14:00:00'),
    (4, 19, 2.8, N'Not as expected, needs improvement.', '2025-01-07 16:20:00'),
    (5, 20, 4.3, N'Comfortable and clean, worth the price.', '2025-01-08 10:15:00'),
    (6, 21, 3.5, N'Adequate, but the staff could be friendlier.', '2025-01-09 12:40:00'),
    (7, 22, 4.9, N'Absolutely fantastic stay, highly recommend.', '2025-01-10 15:10:00'),
    (8, 23, 4.1, N'Nice ambiance, but food options are limited.', '2025-01-11 17:30:00'),
    (9, 24, 3.7, N'Affordable and decent, nothing special.', '2025-01-12 09:50:00'),
    (10, 25, 4.5, N'Lovely experience, very welcoming staff.', '2025-01-13 11:25:00'),
    (11, 1, 3.8, N'Decent stay with good amenities.', '2025-01-14 08:45:00'),
    (12, 2, 4.0, N'Good value for the price.', '2025-01-15 10:15:00'),
    (13, 3, 5.0, N'Wonderful experience, will recommend.', '2025-01-16 13:20:00'),
    (14, 4, 2.6, N'Not great, needs better service.', '2025-01-17 15:10:00'),
    (15, 5, 4.8, N'Amazing place with friendly staff.', '2025-01-18 17:30:00'),
    (1, 6, 3.9, N'Comfortable, but parking is an issue.', '2025-01-19 09:00:00'),
    (2, 7, 4.2, N'Good overall experience.', '2025-01-20 11:10:00'),
    (3, 8, 4.5, N'Lovely ambiance and clean rooms.', '2025-01-21 13:45:00'),
    (4, 9, 2.9, N'Too noisy, not ideal for families.', '2025-01-22 16:00:00'),
    (5, 10, 4.3, N'Great location, close to everything.', '2025-01-23 18:30:00'),
    (6, 11, 3.6, N'Decent stay, but could be cleaner.', '2025-01-24 09:30:00'),
    (7, 12, 4.7, N'Excellent service and comfortable rooms.', '2025-01-25 11:15:00'),
    (8, 13, 4.0, N'Good value for a short trip.', '2025-01-26 14:10:00'),
    (9, 14, 5.0, N'Perfect stay, will come back again!', '2025-01-27 16:20:00'),
    (10, 15, 3.5, N'Adequate, but staff could be better.', '2025-01-28 18:50:00'),
    (11, 16, 4.9, N'Amazing experience, highly recommend.', '2025-01-29 09:40:00'),
    (12, 17, 3.7, N'Affordable, but limited facilities.', '2025-01-30 11:50:00'),
    (13, 18, 4.5, N'Lovely place, very welcoming.', '2025-01-31 14:30:00'),
    (14, 19, 2.5, N'Disappointing, not as advertised.', '2025-02-01 16:10:00'),
    (15, 20, 4.0, N'Good for a weekend getaway.', '2025-02-02 18:20:00'),
	(1, 21, 4.8, N'Amazing hospitality and great location.', '2025-02-03 09:00:00'),
    (2, 22, 3.2, N'Could be better, but overall acceptable.', '2025-02-04 10:30:00'),
    (3, 23, 5.0, N'Phenomenal! Will definitely recommend.', '2025-02-05 12:00:00'),
    (4, 24, 3.5, N'Decent experience, but staff needs training.', '2025-02-06 13:45:00'),
    (5, 25, 4.6, N'Lovely ambiance and great amenities.', '2025-02-07 15:15:00'),
    (6, 1, 2.9, N'Needs improvement in cleanliness.', '2025-02-08 16:30:00'),
    (7, 2, 4.1, N'Good value for the price.', '2025-02-09 17:50:00'),
    (8, 3, 3.8, N'Satisfactory stay, nothing extraordinary.', '2025-02-10 09:20:00'),
    (9, 4, 4.4, N'Great location and comfortable rooms.', '2025-02-11 10:40:00'),
    (10, 5, 4.0, N'Good stay, but food options were limited.', '2025-02-12 12:15:00'),
    (11, 6, 3.6, N'Adequate for short stays.', '2025-02-13 13:50:00'),
    (12, 7, 5.0, N'Perfect experience, exceeded expectations.', '2025-02-14 15:30:00'),
    (13, 8, 3.4, N'Affordable, but service was slow.', '2025-02-15 17:10:00'),
    (14, 9, 4.3, N'Comfortable rooms and friendly staff.', '2025-02-16 18:50:00'),
    (15, 10, 4.5, N'Lovely place with excellent service.', '2025-02-17 09:10:00'),
    (1, 11, 2.5, N'Not worth the price.', '2025-02-18 10:30:00'),
    (2, 12, 4.8, N'Exceptional experience, will recommend!', '2025-02-19 12:00:00'),
    (3, 13, 4.2, N'Good value, but could improve amenities.', '2025-02-20 13:45:00'),
    (4, 14, 3.0, N'Too noisy and crowded.', '2025-02-21 15:15:00'),
    (5, 15, 4.7, N'Excellent location and clean facilities.', '2025-02-22 16:30:00'),
    (6, 16, 3.7, N'Comfortable, but needs better maintenance.', '2025-02-23 17:50:00'),
    (7, 17, 5.0, N'Outstanding service and value!', '2025-02-24 09:20:00'),
    (8, 18, 4.1, N'Nice stay, but room was small.', '2025-02-25 10:40:00'),
    (9, 19, 4.5, N'Lovely experience, will visit again.', '2025-02-26 12:15:00'),
    (10, 20, 3.9, N'Decent for the price.', '2025-02-27 13:50:00'),
    (11, 21, 4.0, N'Good choice for family trips.', '2025-02-28 15:30:00'),
    (12, 22, 4.3, N'Comfortable and clean.', '2025-03-01 17:10:00'),
    (13, 23, 3.8, N'Affordable, but average experience.', '2025-03-02 18:50:00'),
    (14, 24, 4.9, N'Simply the best!', '2025-03-03 09:10:00'),
    (15, 25, 4.1, N'Good amenities, but slightly overpriced.', '2025-03-04 10:30:00'),
    (1, 1, 4.2, N'Great ambiance and friendly staff.', '2025-03-05 12:00:00'),
    (2, 2, 4.4, N'Lovely place for a weekend getaway.', '2025-03-06 13:45:00'),
    (3, 3, 5.0, N'Perfect experience, highly recommend.', '2025-03-07 15:15:00'),
    (4, 4, 2.6, N'Not what I expected.', '2025-03-08 16:30:00'),
    (5, 5, 4.3, N'Comfortable stay, but needs better food.', '2025-03-09 17:50:00'),
    (6, 6, 3.9, N'Decent for short visits.', '2025-03-10 09:20:00'),
    (7, 7, 4.5, N'Great value and clean rooms.', '2025-03-11 10:40:00'),
    (8, 8, 3.7, N'Affordable, but limited facilities.', '2025-03-12 12:15:00'),
    (9, 9, 4.0, N'Good overall experience.', '2025-03-13 13:50:00'),
    (10, 10, 4.8, N'Exceptional, will return soon!', '2025-03-14 15:30:00'),
    (11, 11, 3.2, N'Adequate for business trips.', '2025-03-15 17:10:00'),
    (12, 12, 4.6, N'Lovely place, highly recommend.', '2025-03-16 18:50:00'),
    (13, 13, 4.1, N'Comfortable and great service.', '2025-03-17 09:10:00'),
    (14, 14, 3.5, N'Decent stay, but parking is an issue.', '2025-03-18 10:30:00'),
    (15, 15, 4.9, N'Perfect for a family trip!', '2025-03-19 12:00:00');