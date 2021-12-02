-- create a database
CREATE DATABASE [example];
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
    ('Rustic Webcam'),
    ('Haunted Coloring Book');
GO

INSERT INTO [example].[dbo].[customers] ([first_name], [last_name], [email], [phone])
VALUES
    ('Winston', 'Higginsbury', 'higgs@example.com',    '555-555-5555'),
    ('Vivian',  'Vavilov',     'vivivavi@example.com', '555-555-5556');
GO
