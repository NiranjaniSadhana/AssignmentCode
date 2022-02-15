CREATE TABLE Customers (
    CustomerId int  IDENTITY(1,1) NOT NULL,
    CustomerName varchar(255) NOT NULL,
    City varchar(255),
    State varchar(255),
    Country varchar(255),
	 PRIMARY KEY (CustomerId)
);

GO

CREATE UNIQUE INDEX Customer_dup
ON Customers (CustomerName, City,State,Country);
