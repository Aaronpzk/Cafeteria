{
  "ConnectionStrings": {
    "CafeteriaDB": "Server=LAPTOP-RH9EIQGM\\SQLEXPRESS;Database=CafeteriaDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}/*
	Base de datos de Cafetería Aroma
	Compatible con SQL Server
*/

USE master;
GO

IF DB_ID(N'CafeteriaDB') IS NULL
BEGIN
	CREATE DATABASE CafeteriaDB;
END;
GO

USE CafeteriaDB;
GO

IF OBJECT_ID(N'dbo.Cafes', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Cafes
	(
		Id INT IDENTITY(1,1) NOT NULL,
		Nombre NVARCHAR(200) NOT NULL,
		Descripcion NVARCHAR(500) NOT NULL,
		Precio DECIMAL(10,2) NOT NULL,
		Disponible BIT NOT NULL CONSTRAINT DF_Cafes_Disponible DEFAULT (1),
		CONSTRAINT PK_Cafes PRIMARY KEY (Id),
		CONSTRAINT CK_Cafes_Precio_NoNegativo CHECK (Precio >= 0)
	);
END;
GO

IF OBJECT_ID(N'dbo.Pedidos', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Pedidos
	(
		Id INT IDENTITY(1,1) NOT NULL,
		Cliente NVARCHAR(200) NOT NULL,
		CafeId INT NOT NULL,
		Cantidad INT NOT NULL,
		PrecioUnitario DECIMAL(10,2) NOT NULL,
		Observaciones NVARCHAR(250) NULL,
		Fecha DATETIME2 NOT NULL CONSTRAINT DF_Pedidos_Fecha DEFAULT (SYSUTCDATETIME()),
		Estado NVARCHAR(50) NOT NULL CONSTRAINT DF_Pedidos_Estado DEFAULT (N'Pendiente'),
		CONSTRAINT PK_Pedidos PRIMARY KEY (Id),
		CONSTRAINT CK_Pedidos_Cantidad_Valida CHECK (Cantidad BETWEEN 1 AND 20),
		CONSTRAINT CK_Pedidos_Precio_NoNegativo CHECK (PrecioUnitario >= 0),
		CONSTRAINT FK_Pedidos_Cafes FOREIGN KEY (CafeId)
			REFERENCES dbo.Cafes (Id)
			ON DELETE NO ACTION
	);
END;
GO

/* Datos iniciales del catálogo mostrado en la página web. */
IF NOT EXISTS (SELECT 1 FROM dbo.Cafes WHERE Nombre = N'Espresso')
BEGIN
	INSERT INTO dbo.Cafes (Nombre, Descripcion, Precio, Disponible)
	VALUES
		(N'Espresso', N'Café intenso y aromático en una taza pequeña.', 1.80, 1),
		(N'Cappuccino', N'Espresso con leche cremosa y espuma.', 2.80, 1),
		(N'Latte', N'Suave combinación de espresso y leche vaporizada.', 3.00, 1),
		(N'Moka', N'Café con chocolate y una textura cremosa.', 3.40, 1),
		(N'Café con hielo', N'Café refrescante servido con hielo.', 2.50, 1);
END;
GO

/*
	Ejemplo opcional de pedidos.
	Descomenta este bloque si quieres cargar pedidos de prueba.
*/
/*
INSERT INTO dbo.Pedidos
	(Cliente, CafeId, Cantidad, PrecioUnitario, Observaciones, Estado)
SELECT
	N'Ana', Id, 2, Precio, N'Poco azúcar', N'Pendiente'
FROM dbo.Cafes
WHERE Nombre = N'Cappuccino';

INSERT INTO dbo.Pedidos
	(Cliente, CafeId, Cantidad, PrecioUnitario, Observaciones, Estado)
SELECT
	N'Carlos', Id, 1, Precio, NULL, N'Pendiente'
FROM dbo.Cafes
WHERE Nombre = N'Espresso';
GO
*/

/* Consultas de comprobación. */
SELECT
	Id,
	Nombre,
	Descripcion,
	Precio,
	Disponible
FROM dbo.Cafes
ORDER BY Nombre;

SELECT
	p.Id,
	p.Cliente,
	c.Nombre AS Cafe,
	p.Cantidad,
	p.PrecioUnitario,
	p.Cantidad * p.PrecioUnitario AS Total,
	p.Observaciones,
	p.Fecha,
	p.Estado
FROM dbo.Pedidos AS p
INNER JOIN dbo.Cafes AS c ON c.Id = p.CafeId
ORDER BY p.Fecha DESC;
GO
Server=LAPTOP-RH9EIQGM\SQLEXPRESSDatabase=CafeteriaDBTrusted_Connection=TrueTrustServerCertificate=True