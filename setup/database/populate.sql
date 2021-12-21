USE master;
GO
CREATE LOGIN [vault-VaultSample] WITH PASSWORD = N'DatabaseAdminPassword2';
ALTER SERVER ROLE [securityadmin] ADD MEMBER [vault-VaultSample];
ALTER SERVER ROLE [processadmin] ADD MEMBER [vault-VaultSample];
GRANT ALTER ANY LOGIN TO [vault-VaultSample];
GO

-- create a database
CREATE DATABASE [example];
GO

USE [example];
GO

CREATE USER [vault-VaultSample] FOR LOGIN [vault-VaultSample]; -- Add a database user for the Vault root login
ALTER ROLE [db_accessadmin] ADD MEMBER [vault-VaultSample];
ALTER ROLE [db_securityadmin] ADD MEMBER [vault-VaultSample];
CREATE ROLE [vault_datareader]; -- Create a user-defined database role because only db_owner members can alter fixed database roles
ALTER ROLE [db_datareader] ADD MEMBER [vault_datareader]; -- Add user-defined role to fixed db_datareader role
GRANT ALTER ANY USER TO [vault-VaultSample]; -- Required for Vault to drop database users when the TTL expires
GO
USE master;
GO
ALTER LOGIN [vault-VaultSample] WITH DEFAULT_DATABASE = [example]; -- Change the default database for the root login
GO

-- create a couple of tables
CREATE TABLE [example].[dbo].[products] (
   [id]          INT           PRIMARY KEY IDENTITY(1,1),
   [name]        VARCHAR(255)  NOT NULL
);
GO

CREATE TABLE [example].[dbo].[customers] (
   [id]          INT           PRIMARY KEY IDENTITY(1,1),
   [first_name]  VARCHAR(50)   NOT NULL,
   [last_name]   VARCHAR(50)   NOT NULL,
   [email]       VARCHAR(255)  NOT NULL,
   [phone]       VARCHAR(15)   NOT NULL
);
GO

-- populate them
INSERT INTO [example].[dbo].[products] ([name])
VALUES
    ( 'Rustic Webcam' ),
    ( 'Haunted Coloring Book' );
GO

INSERT INTO [example].[dbo].[customers] ([first_name], [last_name], [email], [phone])
VALUES
    ( 'Winston', 'Higginsbury', 'higgs@example.com',    '555-555-5555' ),
    ( 'Vivian',  'Vavilov',     'vivivavi@example.com', '555-555-5556' );
GO
