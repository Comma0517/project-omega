IF OBJECT_ID('[dbo].[OmegaUser]', 'U') IS NULL
CREATE TABLE [dbo].[OmegaUser]
(
    [Id]        INT           NOT NULL PRIMARY KEY identity (1, 1),
    [FirstName] NVARCHAR(50)  NOT NULL,
    [LastName]  NVARCHAR(50)  NOT NULL,
    [Email]     NVARCHAR(100) NOT NULL
);
GO
