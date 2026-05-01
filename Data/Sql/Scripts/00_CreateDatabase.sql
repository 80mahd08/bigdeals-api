IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BigDealsDb')
BEGIN
    CREATE DATABASE [BigDealsDb];
END
GO
