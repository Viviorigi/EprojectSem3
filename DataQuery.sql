use EprojectSem3
go

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

INSERT INTO UserSubscriptions (UserId, SubscriptionId, StartDate, EndDate) VALUES
-- Monthly Plans for Salers
(1, 1, '2024-01-01', '2024-01-31'),  -- John Doe (30 days)
(2, 2, '2024-02-01', '2024-02-29'),  -- Jane Smith (30 days)
(3, 3, '2024-03-01', '2024-03-31'),  -- Michael Brown (30 days)
(4, 4, '2024-04-01', '2025-03-31'),  -- Emily Clark (365 days, 1 year)
(5, 5, '2024-05-01', '2025-04-30'),  -- David Wilson (365 days, 1 year)
(6, 6, '2024-06-01', '2025-05-31'),  -- Sarah Miller (365 days, 1 year)
(7, 1, '2024-07-01', '2024-07-31'),  -- William Lee (30 days)
(8, 2, '2024-08-01', '2024-08-31'),  -- Olivia Taylor (30 days)
(9, 3, '2024-09-01', '2024-09-30'),  -- James Harris (30 days)
(10, 4, '2024-10-01', '2025-09-30'), -- Sophia Martin (365 days, 1 year)

-- Monthly Plans for Agents
(11, 7, '2024-01-15', '2024-02-14'),  -- Alex Carter (30 days)
(12, 8, '2024-02-15', '2024-03-15'),  -- Emma Walker (30 days)
(13, 9, '2024-03-15', '2025-03-14'),  -- Michael Scott (365 days, 1 year)
(14, 7, '2024-04-15', '2024-05-15'),  -- Liam Johnson (30 days)
(15, 8, '2024-05-15', '2024-06-14');  -- Ava White (30 days)

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
