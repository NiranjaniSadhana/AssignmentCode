CREATE PROCEDURE [dbo].[InsertCustomerXML]
@xml XML
AS
BEGIN
      SET NOCOUNT ON;
 
      INSERT INTO dbo.Customers
      SELECT
      --Customer.value('@CustomerId','INT') AS CustomerId, --ATTRIBUTE
      Customer.value('(CustomerName/text())[1]','VARCHAR(100)') AS CustomerName, --TAG
      Customer.value('(City/text())[1]','VARCHAR(100)') AS City, --TAG
	  Customer.value('(State/text())[1]','VARCHAR(100)') AS State,--TAG
	  Customer.value('(Country/text())[1]','VARCHAR(100)') AS Country --TAG
      FROM
      @xml.nodes('/ArrayOfCustomerInfo/CustomerInfo')AS TEMPTABLE(Customer)
END