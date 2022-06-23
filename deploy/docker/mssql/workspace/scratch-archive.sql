IF OBJECT_ID('[dbo].[OmegaUser]', 'U') IS NULL
CREATE TABLE [dbo].[OmegaUser]
(
    [Id] INT NOT NULL PRIMARY KEY identity(1, 1),
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL
);
GO


USE master
GO
IF EXISTS (
    SELECT [name]
        FROM sys.databases
        WHERE [name] = N'OmegaDeleteMe'
)
DROP DATABASE OmegaDeleteMe
GO

INSERT INTO [dbo].[OmegaUser]
(
 [FirstName], [LastName], [Email]
)
VALUES
(
 'Chuck', 'Norris', 'chuck.norris@test.com'
),
(
 'Your', 'Mom', 'your.mom@test.com'
)
GO

