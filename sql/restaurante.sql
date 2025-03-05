CREATE TABLE Pedidos(
Id INTEGER UNIQUE NOT NULL PRIMARY KEY,
Mesa INTEGER,
Fecha NUMERIC,
Hora NUMERIC);

CREATE TABLE "Pedidos-Productos"(
PedidoID INTEGER NOT NULL REFERENCES Pedidos,
ProductoID INTEGER NOT NULL REFERENCES Productos,
Cantidad INTEGER,
PRIMARY KEY (PedidoID, ProductoID));

CREATE TABLE "Productos"(
Id INTEGER UNIQUE NOT NULL PRIMARY KEY,
Nombre TEXT,
Precio REAL);

INSERT INTO Pedidos VALUES
(1,1,'2021-02-22','13:20'),
(2,2,'2021-02-22','13:25'),
(3,3,'2021-02-22','13:40'),
(4,2,'2021-02-22','13:55'),
(5,3,'2021-02-23','13:05'),
(6,1,'2021-02-23','13:30');

INSERT INTO Productos VALUES
(1,'Hamburguesa',7.5),
(2,'Perrito',5),
(3,'Pizza',8.45),
(4,'Ensalada',6.1),
(5,'Nuggets',4.95);

INSERT INTO "Pedidos-Productos" VALUES
(1,1,1),
(1,2,1),
(2,4,1),
(2,1,2),
(3,3,2),
(4,1,4),
(5,2,1),
(5,3,2),
(6,2,3);
