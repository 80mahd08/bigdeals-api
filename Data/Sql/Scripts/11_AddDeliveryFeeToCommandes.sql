USE BigDealsDb;
GO
SET QUOTED_IDENTIFIER ON;
GO

PRINT 'Adding delivery fee columns to Commandes table...';

-- 1. Add MontantAnnonce column
IF COL_LENGTH('dbo.Commandes', 'MontantAnnonce') IS NULL
BEGIN
    ALTER TABLE dbo.Commandes ADD MontantAnnonce DECIMAL(18,3) NOT NULL DEFAULT 0;
    PRINT 'Added MontantAnnonce column.';
END
GO

-- 2. Add FraisLivraison column
IF COL_LENGTH('dbo.Commandes', 'FraisLivraison') IS NULL
BEGIN
    ALTER TABLE dbo.Commandes ADD FraisLivraison DECIMAL(18,3) NOT NULL DEFAULT 0;
    PRINT 'Added FraisLivraison column.';
END
GO

-- 3. Migrate existing data for old orders
-- For existing orders, MontantAnnonce should be the original total Montant, and FraisLivraison should be 0.
-- We only update rows where MontantAnnonce is 0 (the default we just added).
PRINT 'Migrating existing data for old orders...';
UPDATE dbo.Commandes 
SET MontantAnnonce = Montant,
    FraisLivraison = 0
WHERE MontantAnnonce = 0;

PRINT 'Migration 11_AddDeliveryFeeToCommandes completed successfully.';
GO
