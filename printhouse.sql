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


--alter table aspnetusers add customerFirstName varchar(20)
--alter table aspnetusers add customerLastName varchar(20)
--alter table aspnetusers add customerPhone varchar(20)
--alter table aspnetuserroles add Email nvarchar(256)
--alter table Products add subCategoryId int foreign key references subCategories(subCategoryId)
--alter table subCategories add filter varchar(20)
--alter table products add stock int
--alter table reviews add rating int
--alter table reviews add show bit

CREATE TABLE Orders
(
  orderId int primary key identity(1,1),
  orderDate datetime,
  totalAmount decimal(10,2),
  userId nvarchar(128) foreign key references aspnetusers (Id)
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
  userId nvarchar(128) foreign key references aspnetusers (Id),
  maintencanceOrderDetails varchar(255)
);



CREATE TABLE Cart
(
  cartId int primary key identity(1,1),
  userId nvarchar(128) foreign key references aspnetusers (Id),
  productId int foreign key references Products (productId),
  quantity int,
  price decimal(10,2),
  totalPrice decimal(10,2)
);

CREATE TABLE Reviews
(
reviewId int primary key identity(1,1),
  userId nvarchar(128) foreign key references aspnetusers (Id),
  productId int foreign key references Products (productId),
	comment varchar(max) 
)

--CREATE TABLE Customers
--(
--  customerId int primary key identity(1,1),
--  customerFirstName varchar(20),
--  customerLastName varchar(20),
--  customerPhone varchar(20),
--  aspId nvarchar(128) foreign key references AspNetUsers (Id)
--);