-- Ejecuta este script desde SQL Server Management Studio (SSMS).
CREATE DATABASE ClinicaDB;
GO

USE ClinicaDB;
GO

CREATE TABLE Contacts
(
    ContactId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(120) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Phone NVARCHAR(25) NOT NULL,
    BirthDate DATE NOT NULL,
    Address NVARCHAR(250) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Contacts_CreatedAt DEFAULT SYSDATETIME()
);
GO

-- Datos de prueba opcionales:
INSERT INTO Contacts (FullName, Email, Phone, BirthDate, Address)
VALUES
('María Pérez', 'maria.perez@example.com', '809-555-0101', '1998-04-12', 'Santo Domingo'),
('Carlos Gómez', 'carlos.gomez@example.com', '829-555-0102', '1987-09-23', 'Santiago');
GO

SELECT * FROM Contacts;
GO
