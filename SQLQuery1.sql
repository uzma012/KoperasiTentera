CREATE TABLE UserStateType (
  UserStateTypeID tinyint IDENTITY NOT NULL, 
  Name            nvarchar(20) NOT NULL, 
  Description     nvarchar(100) NULL, 
  PRIMARY KEY (UserStateTypeID));

CREATE TABLE ApplicationUser (
  UserID                       int IDENTITY NOT NULL, 
  UUID                         uniqueidentifier NOT NULL UNIQUE, 
  FirstName                    nvarchar(50) NOT NULL, 
  LastName                     nvarchar(50) NOT NULL, 
  MobileNumber                 nvarchar(20) NOT NULL, 
  Email                        nvarchar(100) NOT NULL,
  ICNumber                     nvarchar(100) NOT NULL, 
  UserStateTypeId              tinyint NOT NULL,
  CreatedOn                    datetime NOT NULL, 
  CreatedBy                    int NOT NULL, 
  LastModifiedOn               dateTime NOT NULL, 
  LastModifiedBy               int NOT NULL, 
  PRIMARY KEY (UserID));
ALTER TABLE ApplicationUser ADD CONSTRAINT FKApplicatio473179 FOREIGN KEY (UserStateTypeId) REFERENCES UserStateType (UserStateTypeID);

ALTER TABLE ApplicationUser
ADD CONSTRAINT UQ_Email_MobileNumber_ICNumber UNIQUE (Email, MobileNumber, ICNumber);


CREATE TABLE Account (
  AccountId        bigint IDENTITY NOT NULL, 
  UserId                int NOT NULL, 
  AccountName   nvarchar(50) Not Null
  PRIMARY KEY (AccountId));
ALTER TABLE UserAccount ADD CONSTRAINT FKUserAccoun895879 FOREIGN KEY (UserID) REFERENCES ApplicationUser (UserID);



CREATE TABLE UserDevice (
  UserDeviceID      bigint IDENTITY NOT NULL, 
  UserID            int NOT NULL, 
  DeviceProfile     nvarchar(4000) NULL, 
  DeviceProfileHash nvarchar(50) NULL, 
  PRIMARY KEY (UserDeviceID));
ALTER TABLE UserDevice ADD CONSTRAINT FKUserDevice845920 FOREIGN KEY (UserID) REFERENCES ApplicationUser (UserID);



CREATE TABLE UserCredential (
  UserCredentialID       bigint IDENTITY NOT NULL, 
  UserDeviceID           bigint NOT NULL, 
  SharedSecret           nvarchar(255) NOT NULL, 
  UserPIN                nvarchar(255) NOT NULL, 
  BiometricEnabled       bit NOT NULL, 
  BiometericSharedSecret nvarchar(255) NULL, 
  RetryCounter           tinyint NOT NULL, 
  PRIMARY KEY (UserCredentialID));
ALTER TABLE UserCredential ADD CONSTRAINT FKUserCreden289141 FOREIGN KEY (UserDeviceID) REFERENCES UserDevice (UserDeviceID);



CREATE TABLE OTPChannelType (
  OTPChannelTypeID tinyint IDENTITY NOT NULL, 
  Name             nvarchar(20) NOT NULL, 
  Description      nvarchar(100) NULL, 
  PRIMARY KEY (OTPChannelTypeID));


CREATE TABLE OTPMessageStateType (
  OTPMessageStateTypeID tinyint IDENTITY NOT NULL, 
  Name                  nvarchar(20) NOT NULL, 
  Description           nvarchar(100) NULL, 
  PRIMARY KEY (OTPMessageStateTypeID));


CREATE TABLE OTPPurposeType (
  OTPPurposeTypeID tinyint IDENTITY NOT NULL, 
  Name             nvarchar(50) NOT NULL, 
  Description      nvarchar(100) NULL, 
  PRIMARY KEY (OTPPurposeTypeID));

  CREATE TABLE OTPStateType (
  OTPStateTypeID tinyint IDENTITY NOT NULL, 
  Name           nvarchar(20) NOT NULL, 
  Description    nvarchar(100) NULL, 
  PRIMARY KEY (OTPStateTypeID));

 CREATE TABLE UserOTP (
  UserOTPID                  bigint IDENTITY NOT NULL, 
  UserID                     int NOT NULL, 
  OTPPurposeTypeID           tinyint NOT NULL, 
  OTP                        nvarchar(50) NOT NULL, 
  OTPStateTypeID             tinyint NOT NULL, 
  OTPStateTypeOTPStateTypeID tinyint NULL, 
  RetryCounter               tinyint NOT NULL, 
  CreatedOn                  datetime NOT NULL, 
  PRIMARY KEY (UserOTPID));
ALTER TABLE UserOTP ADD CONSTRAINT FKUserOTP241932 FOREIGN KEY (OTPPurposeTypeID) REFERENCES OTPPurposeType (OTPPurposeTypeID);
ALTER TABLE UserOTP ADD CONSTRAINT FKUserOTP708155 FOREIGN KEY (OTPStateTypeOTPStateTypeID) REFERENCES OTPStateType (OTPStateTypeID);
ALTER TABLE UserOTP ADD CONSTRAINT FKUserOTP926566 FOREIGN KEY (UserID) REFERENCES ApplicationUser (UserID);

CREATE TABLE UserOTPMessage (
  UserOTPMessageID      bigint IDENTITY NOT NULL, 
  UserOTPID             bigint NOT NULL, 
  OTPChannelTypeID      tinyint NOT NULL, 
  OTPMessageStateTypeID tinyint NOT NULL, 
  ExternalMessageId     nvarchar 
  PRIMARY KEY (UserOTPMessageID));
ALTER TABLE UserOTPMessage ADD CONSTRAINT FKUserOTPMes528385 FOREIGN KEY (UserOTPID) REFERENCES UserOTP (UserOTPID);
ALTER TABLE UserOTPMessage ADD CONSTRAINT FKUserOTPMes314179 FOREIGN KEY (OTPChannelTypeID) REFERENCES OTPChannelType (OTPChannelTypeID);
ALTER TABLE UserOTPMessage ADD CONSTRAINT FKUserOTPMes43763 FOREIGN KEY (OTPMessageStateTypeID) REFERENCES OTPMessageStateType (OTPMessageStateTypeID);

Insert into UserStateType (Name) values 
		('Pending Verification'),
		('Registered'),
		('Suspended'),
		('Deleted'),
		('Locked')
Insert into OTPChannelType (Name) Values ('Email'),('SMS')
Insert into OTPStateType (Name) values ('Pending Verification'),
			('Verified'),
			('Expired')
Insert into OTPPurposeType (Name) values ('Registration'),
			('New Device Enrollment')
Insert into OTPMessageStateType (Name) Values
				('Accepted'),
				('Queued'),
				('Sending'),
				('Sent'),
				('Delivered'),
				('Undelivered'),
				('Failed')


CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(10, 2) NOT NULL,
    Category NVARCHAR(100),
    StockQuantity INT DEFAULT 0,
    ImageUrl NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE() 
);

CREATE TABLE Ads (
    AdId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    TargetUrl NVARCHAR(255) NOT NULL,
    ImageUrl NVARCHAR(255),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE() 
);


INSERT INTO Products (Name, Description, Price, Category, StockQuantity, ImageUrl, CreatedAt, UpdatedAt)
VALUES
    ('Laptop', 'High-performance laptop for gaming and work.', 999.99, 'Electronics', 50, 'http://example.com/images/laptop.jpg', GETDATE(), GETDATE()),
    ('Smartphone', 'Latest model smartphone with excellent features.', 799.99, 'Electronics', 100, 'http://example.com/images/smartphone.jpg', GETDATE(), GETDATE()),
    ('Headphones', 'Noise-cancelling over-ear headphones.', 199.99, 'Accessories', 75, 'http://example.com/images/headphones.jpg', GETDATE(), GETDATE()),
    ('Smartwatch', 'Wearable tech to monitor your health.', 249.99, 'Electronics', 30, 'http://example.com/images/smartwatch.jpg', GETDATE(), GETDATE()),
    ('Backpack', 'Durable backpack for travel and daily use.', 49.99, 'Accessories', 150, 'http://example.com/images/backpack.jpg', GETDATE(), GETDATE());


	INSERT INTO Ads (Title, Description, TargetUrl, ImageUrl, StartDate, EndDate, CreatedAt, UpdatedAt)
VALUES
    ('Summer Sale', 'Get up to 50% off on select items during our summer sale!', 'http://example.com/sale', 'http://example.com/images/summer_sale.jpg', '2024-06-01 00:00:00', '2024-06-30 23:59:59', GETDATE(), GETDATE()),
    ('New Arrivals', 'Check out the latest arrivals in our store!', 'http://example.com/new-arrivals', 'http://example.com/images/new_arrivals.jpg', '2024-05-01 00:00:00', '2024-05-31 23:59:59', GETDATE(), GETDATE()),
    ('Holiday Specials', 'Special discounts for the holiday season!', 'http://example.com/holiday-specials', 'http://example.com/images/holiday_specials.jpg', '2024-12-01 00:00:00', '2024-12-31 23:59:59', GETDATE(), GETDATE()),
    ('Flash Sale', 'Limited time flash sale on select products!', 'http://example.com/flash-sale', 'http://example.com/images/flash_sale.jpg', '2024-04-15 00:00:00', '2024-04-15 23:59:59', GETDATE(), GETDATE()),
    ('Free Shipping', 'Enjoy free shipping on orders over $50!', 'http://example.com/free-shipping', 'http://example.com/images/free_shipping.jpg', '2024-01-01 00:00:00', '2024-01-31 23:59:59', GETDATE(), GETDATE());
