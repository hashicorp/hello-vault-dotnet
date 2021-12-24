-- Create a database
CREATE DATABASE [example];
GO

-- Add a database user for the Vault root login
CREATE LOGIN [vault-db-user] WITH PASSWORD = N'VaultDatabasePassword1';
GO

ALTER SERVER ROLE [securityadmin] ADD MEMBER [vault-db-user];
ALTER SERVER ROLE [processadmin] ADD MEMBER [vault-db-user];
GO

GRANT ALTER ANY LOGIN TO [vault-db-user];
GO

USE [example];
GO

CREATE USER [vault-db-user] FOR LOGIN [vault-db-user];
GO

ALTER ROLE [db_accessadmin] ADD MEMBER [vault-db-user];
ALTER ROLE [db_securityadmin] ADD MEMBER [vault-db-user];
GO

 -- Required for Vault to drop database users when the TTL expires
GRANT ALTER ANY USER TO [vault-db-user];
GO

-- Change the default database for the root login
ALTER LOGIN [vault-db-user] WITH DEFAULT_DATABASE = [example];
GO

-- Create a user-defined database role because only db_owner members can alter fixed database roles
CREATE ROLE [vault_datareader];

-- Add user-defined role to fixed db_datareader role
ALTER ROLE [db_datareader] ADD MEMBER [vault_datareader];

-- Create a couple of tables
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

-- Populate them
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
