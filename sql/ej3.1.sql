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
(4,"Zeppeli",2,1500,8000,"Head of R&D",3),
(5,"Eustaquio",4,500,0,"Intern",3),
(6,"Bernarda",5,0,150,"Intern's Intern",3),
(7,"Herminio",2,600,600,"Coffe Guy",4);

-- SOLUCIONES
-- 1. Id del jefe que dirige al trabajador con menor salario.
SELECT Jefe
FROM Empleados
WHERE Salario = ( SELECT min(Salario)
FROM Empleados);

-- Con el nombre
SELECT E2.Nombre
FROM Empleados AS E1 JOIN Empleados AS E2 ON E1.Jefe = E2.Id
WHERE E1.Salario = ( SELECT min(Salario)
FROM Empleados);

-- 2. Indique el id de los diferentes jefes de la empresa.
SELECT DISTINCT(Jefe)
FROM Empleados
WHERE Jefe IS NOT NULL;

-- 3. Muestre el nombre y los salarios de aquellos empleados cuyo nombre termine por o
SELECT Nombre, Salario
FROM Empleados
WHERE Nombre like '%o';

-- 4. Reasigna todos los empleados al departamento del empleado que menos cobra.
UPDATE Empleados
SET DepId = (SELECT DepId
FROM Empleados
WHERE Salario = (SELECT min(Salario)
FROM Empleados));

-- 5. Elimina todos los empleados que tengan una "a" y una "i" en algún lugar de su nombre y ganen menos que el salario medio.
DELETE FROM Empleados
WHERE Nombre LIKE '%a%' AND Nombre LIKE '%i%' AND Salario < (
	SELECT AVG(Salario) FROM Empleados);

-- 6. ¿En cuantos lugares diferentes tenemos departamentos?
SELECT count(DISTINCT Lugar) 'Lugares Únicos'
FROM Departamentos;

-- 7. ¿Cuánto dinero gasta la empresa en total incluyendo salarios y bonus? Llame a este dato Total.
SELECT SUM(Salario + Bonus) AS 'Total'
FROM Empleados;

-- 8. Indique el nombre de los trabajadores con un bonus mayor que la media
SELECT Nombre
FROM Empleados
WHERE Bonus > (SELECT AVG(Bonus) FROM Empleados);

-- 9. Indique el número de empleados de cada departamento
SELECT DepId, COUNT(DepId) 'Numero Empleados'
FROM Empleados
GROUP BY DepId;

-- 10. ¿Cuántos empleados tiene el departamento con más empleados?
SELECT DepId, MAX(Count) AS 'Max Empleados'
FROM (SELECT DepId, COUNT(DepId) 'Count'
FROM Empleados
GROUP BY DepId);

-- 11. Muestre, para cada jefe, el número de empleados directos que tiene a su cargo. 
SELECT Jefe, COUNT(*)
FROM Empleados
GROUP BY Jefe
HAVING Jefe IS NOT NULL;
