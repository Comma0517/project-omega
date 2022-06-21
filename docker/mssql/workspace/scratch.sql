USE master
GO
IF EXISTS (
    SELECT [name]
        FROM sys.databases
        WHERE [name] = N'OmegaDeleteMe'
)
DROP DATABASE OmegaDeleteMe
GO