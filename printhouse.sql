CREATE TABLE Categories
(
  categoryId int primary key identity(1,1),
  categoryName varchar(50),
  categoryDescription varchar(max),
  categoryImage varchar(50)
);

CREATE TABLE Products
(
  productId int primary key identity(1,1),
  productName varchar(50),
  productDescription varchar(max),
  productImage1 varchar(50),
  productImage2 varchar(50),
  productImage3 varchar(50),
  productPrice decimal (10,2),
  categoryId int foreign key references Categories (categoryId)
);

CREATE TABLE subCategories
(
  subCategoryId int primary key identity(1,1),
    subCategoryName INT NOT NULL,
  subCategoryImage varchar(50),
  subCategoryDescription varchar(max),
  categoryId int foreign key references Categories (categoryId),
);

CREATE TABLE Customers
(
  customerId int primary key identity(1,1),
  customerFirstName varchar(20),
  customerLastName varchar(20),
  customerPhone varchar(20),
  aspId nvarchar(128) foreign key references AspNetUsers (Id)
);

CREATE TABLE Orders
(
  orderId int primary key identity(1,1),
  orderDate datetime,
  totalAmount decimal(10,2),
  customerId int foreign key references Customers (customerId)
);

CREATE TABLE OrderDetails
(
  orderDetailId int primary key identity(1,1),
  orderId int foreign key references Orders (orderId),
  productId int foreign key references Products (productId),
  quantity int,
  price decimal (10,2)
);

CREATE TABLE Maintenances
(
  maintenanceId int primary key identity(1,1),
  orderDate datetime,
  customerId int foreign key references Customers (customerId),
  maintencanceOrderDetails varchar(255)
);



CREATE TABLE Cart
(
  cartId int primary key identity(1,1),
  customerId int foreign key references Customers (customerId),
  productId int foreign key references Products (productId),
  quantity int,
  price decimal(10,2),
  totalPrice decimal(10,2)
);

CREATE TABLE Reviews
(
reviewId int primary key identity(1,1),
  customerId int foreign key references Customers (customerId),
  productId int foreign key references Products (productId),
	comment varchar(max) 
)