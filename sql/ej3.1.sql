CREATE TABLE Empleados(
Id INTEGER UNIQUE NOT NULL PRIMARY KEY,
Nombre TEXT NOT NULL,
Jefe INTEGER REFERENCES Empleados,
Salario INTEGER,
Bonus INTEGER,
Puesto TEXT,
DepId INTEGER REFERENCES Departamentos);

CREATE TABLE Departamentos(
Id INTEGER UNIQUE NOT NULL PRIMARY KEY,
Nombre TEXT NOT NULL,
Lugar TEXT);

INSERT INTO Departamentos VALUES
(1,"Management","Madrid"),
(2,"HR","New York"),
(3,"R&D","Burgos"),
(4,"Coffe Central","Madrid"),
(5,"Garbage Collection","Burgos");

INSERT INTO Empleados VALUES
(1,"Lorenzo",NULL,10000,1000,"CEO",1),
(2,"Manola",1,1000,10000,"CTO",3),
(3,"Polnareff",1,5000,5000,"Head of HR",2),
(4,"Zeppeli",3,1500,8000,"Head of R&D",3),
(5,"Eustaquio",4,500,0,"Intern",3),
(6,"Bernarda",5,0,150,"Intern's Intern",3),
(7,"Herminio",2,600,600,"Coffe Guy",4);
