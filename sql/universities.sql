CREATE TABLE Universities(
    Name TEXT UNIQUE NOT NULL PRIMARY KEY,
	City TEXT,
	Students INTEGER
);
CREATE TABLE Students (
    Id INTEGER UNIQUE NOT NULL PRIMARY KEY,
    Name TEXT,
    Score REAL,
    Photo BLOB,
    University TEXT REFERENCES Universities
);