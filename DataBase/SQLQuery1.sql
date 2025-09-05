
CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Product (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Image VARBINARY(MAX) NULL
);

CREATE TABLE ProductCategory (
    ProductID INT NOT NULL,
    CategoryID INT NOT NULL,
    CONSTRAINT PK_ProductCategory PRIMARY KEY (ProductID, CategoryID),
    CONSTRAINT FK_ProductCategory_Product FOREIGN KEY (ProductID)
        REFERENCES Product(ProductID) ON DELETE CASCADE,
    CONSTRAINT FK_ProductCategory_Category FOREIGN KEY (CategoryID)
        REFERENCES Category(CategoryID) ON DELETE CASCADE
);
