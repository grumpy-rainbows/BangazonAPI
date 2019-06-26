
INSERT INTO Customer (FirstName, LastName) VALUES('Jameka', 'Echols');
INSERT INTO Customer (FirstName, LastName) VALUES('Ali', 'Abdulle');
INSERT INTO Customer (FirstName, LastName) VALUES('Brian', 'Jobe');
INSERT INTO Customer (FirstName, LastName) VALUES('Billy', 'Mathison');

SELECT * from Customer;

INSERT INTO ProductType (Name) VALUES('Appliances');
INSERT INTO ProductType (Name) VALUES('Electronics');
INSERT INTO ProductType (Name) VALUES('Books');
SELECT * FROM productType;

INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (1, 2, 550.00, 'Oven', 'General Electronics create an all around oven for the best bakers around.', 2);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (2, 1, 3600.99, '2019 MacBook Pro', 'Apple presents this new MacBook pro with new a new i8 processor', 5);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (3, 4, 14.00, 'Lovecraft Country', 'In the Jim Crow era, a group of individuals live through the terrors of America.', 19);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (3, 3, 11.50, 'Harry Potter and the Half-blood Prince', 'Another installation of the Wizarding World from author, J.K. Rowling, deliverying yet another magical piece of literature.', 21);

SELECT * FROM Product;