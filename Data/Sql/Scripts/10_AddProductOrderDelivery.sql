USE BigDealsDb;
GO
SET QUOTED_IDENTIFIER ON;
GO

PRINT 'Adding delivery columns to Commandes...';

-- StatutLivraison
IF COL_LENGTH('dbo.Commandes', 'StatutLivraison') IS NULL
BEGIN
    ALTER TABLE dbo.Commandes
    ADD StatutLivraison INT NOT NULL
        CONSTRAINT DF_Commandes_StatutLivraison DEFAULT 1;
    PRINT 'Added StatutLivraison column.';
END
GO

-- AdresseLivraison
IF COL_LENGTH('dbo.Commandes', 'AdresseLivraison') IS NULL
BEGIN
    ALTER TABLE dbo.Commandes ADD AdresseLivraison NVARCHAR(300) NULL;
    PRINT 'Added AdresseLivraison column.';
END
GO

-- VilleLivraison
IF COL_LENGTH('dbo.Commandes', 'VilleLivraison') IS NULL
BEGIN
    ALTER TABLE dbo.Commandes ADD VilleLivraison NVARCHAR(100) NULL;
    PRINT 'Added VilleLivraison column.';
END
GO

-- TelephoneLivraison
IF COL_LENGTH('dbo.Commandes', 'TelephoneLivraison') IS NULL
BEGIN
    ALTER TABLE dbo.Commandes ADD TelephoneLivraison NVARCHAR(30) NULL;
    PRINT 'Added TelephoneLivraison column.';
END
GO


GO

-- DateExpedition
IF COL_LENGTH('dbo.Commandes', 'DateExpedition') IS NULL
BEGIN
    ALTER TABLE dbo.Commandes ADD DateExpedition DATETIME2 NULL;
    PRINT 'Added DateExpedition column.';
END
GO

-- DateLivraison
IF COL_LENGTH('dbo.Commandes', 'DateLivraison') IS NULL
BEGIN
    ALTER TABLE dbo.Commandes ADD DateLivraison DATETIME2 NULL;
    PRINT 'Added DateLivraison column.';
END
GO

-- DateDerniereMiseAJourLivraison
IF COL_LENGTH('dbo.Commandes', 'DateDerniereMiseAJourLivraison') IS NULL
BEGIN
    ALTER TABLE dbo.Commandes ADD DateDerniereMiseAJourLivraison DATETIME2 NULL;
    PRINT 'Added DateDerniereMiseAJourLivraison column.';
END
GO

--------------------------------------------------------------------------------
-- Constraints
--------------------------------------------------------------------------------
PRINT 'Adding delivery constraints...';

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_Commandes_StatutLivraison' AND type = 'C')
    ALTER TABLE dbo.Commandes ADD CONSTRAINT CHK_Commandes_StatutLivraison CHECK (StatutLivraison IN (1, 2, 3, 4, 5, 6, 7));
GO

--------------------------------------------------------------------------------
-- Indexes
--------------------------------------------------------------------------------
PRINT 'Adding delivery indexes...';

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Commandes_StatutLivraison')
    CREATE NONCLUSTERED INDEX IX_Commandes_StatutLivraison ON dbo.Commandes(StatutLivraison);
GO

PRINT 'Delivery lifecycle columns added successfully.';
GO
