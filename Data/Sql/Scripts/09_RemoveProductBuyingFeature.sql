USE BigDealsDb;
GO

--------------------------------------------------------------------------------
-- SCRIPT: 09_RemoveProductBuyingFeature.sql
-- DESCRIPTION: Removes tables and constraints related to the product buying 
--              feature (Cart, Orders, Invoices, Delivery).
--              Does NOT touch Annonces, Users, or Annonceur Payments.
--------------------------------------------------------------------------------

PRINT 'Starting removal of product buying feature...';

-- 1. Drop Foreign Keys first (to avoid dependency errors)
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_LignesCommande_Commandes')
    ALTER TABLE dbo.LignesCommande DROP CONSTRAINT FK_LignesCommande_Commandes;
GO

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_LignesPanier_Paniers')
    ALTER TABLE dbo.LignesPanier DROP CONSTRAINT FK_LignesPanier_Paniers;
GO

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Factures_Commandes')
    ALTER TABLE dbo.Factures DROP CONSTRAINT FK_Factures_Commandes;
GO

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Livraisons_Commandes')
    ALTER TABLE dbo.Livraisons DROP CONSTRAINT FK_Livraisons_Commandes;
GO

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Paiements_Commandes')
    ALTER TABLE dbo.Paiements DROP CONSTRAINT FK_Paiements_Commandes;
GO

-- 2. Drop Tables in correct dependency order
IF OBJECT_ID('dbo.LignesCommande', 'U') IS NOT NULL 
BEGIN
    DROP TABLE dbo.LignesCommande;
    PRINT 'Dropped table: LignesCommande';
END
GO

IF OBJECT_ID('dbo.LignesPanier', 'U') IS NOT NULL 
BEGIN
    DROP TABLE dbo.LignesPanier;
    PRINT 'Dropped table: LignesPanier';
END
GO

IF OBJECT_ID('dbo.Factures', 'U') IS NOT NULL 
BEGIN
    DROP TABLE dbo.Factures;
    PRINT 'Dropped table: Factures';
END
GO

IF OBJECT_ID('dbo.Livraisons', 'U') IS NOT NULL 
BEGIN
    DROP TABLE dbo.Livraisons;
    PRINT 'Dropped table: Livraisons';
END
GO

IF OBJECT_ID('dbo.Paiements', 'U') IS NOT NULL 
BEGIN
    -- Only drop if it is NOT the Annonceur payments table (which is named PaiementsAnnonceur in our schema)
    DROP TABLE dbo.Paiements;
    PRINT 'Dropped table: Paiements (generic orders payment)';
END
GO

IF OBJECT_ID('dbo.Paniers', 'U') IS NOT NULL 
BEGIN
    DROP TABLE dbo.Paniers;
    PRINT 'Dropped table: Paniers';
END
GO

IF OBJECT_ID('dbo.Commandes', 'U') IS NOT NULL 
BEGIN
    DROP TABLE dbo.Commandes;
    PRINT 'Dropped table: Commandes';
END
GO

-- 3. Cleanup any potential constraints in other tables (if any existed for orders)
-- (None identified as core to Marketplace)

PRINT '--------------------------------------------------------------------------------';
PRINT 'Product buying feature tables and constraints removed successfully (if they existed).';
PRINT 'Marketplace core (Annonces, Contacts, Favoris, Avis) remains intact.';
PRINT 'Annonceur Payment flow (PaiementsAnnonceur) remains intact.';
PRINT '--------------------------------------------------------------------------------';
GO
