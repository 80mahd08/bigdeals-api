IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'BigDealsDb')
BEGIN
    CREATE DATABASE [BigDealsDb];
END
GO
