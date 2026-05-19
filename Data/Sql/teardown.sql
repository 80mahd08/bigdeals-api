-- ============================================================================
-- BIGDEALS - DATABASE TEARDOWN SCRIPT
-- ============================================================================
-- Goal: Safely drops all tables, indices, and constraints from the BigDealsDb
-- database. Does NOT drop the database itself.
-- ============================================================================

USE BigDealsDb;
GO
SET QUOTED_IDENTIFIER ON;
GO

PRINT 'Starting teardown of BigDealsDb tables...';

-- Drop foreign keys first to prevent dependency blockages
IF OBJECT_ID('dbo.Signalements', 'U') IS NOT NULL
    ALTER TABLE dbo.Signalements DROP CONSTRAINT IF EXISTS FK_Signalements_Annonces, FK_Signalements_Utilisateurs, FK_Signalements_Admins;

IF OBJECT_ID('dbo.PaiementsCommandes', 'U') IS NOT NULL
    ALTER TABLE dbo.PaiementsCommandes DROP CONSTRAINT IF EXISTS FK_PaiementsCommandes_Commandes;

IF OBJECT_ID('dbo.Commandes', 'U') IS NOT NULL
    ALTER TABLE dbo.Commandes DROP CONSTRAINT IF EXISTS FK_Commandes_Annonces, FK_Commandes_Acheteur, FK_Commandes_Annonceur;

IF OBJECT_ID('dbo.PaiementsAnnonceur', 'U') IS NOT NULL
    ALTER TABLE dbo.PaiementsAnnonceur DROP CONSTRAINT IF EXISTS FK_PaiementsAnnonceur_Utilisateurs, FK_PaiementsAnnonceur_DemandesAnnonceur;

IF OBJECT_ID('dbo.Avis', 'U') IS NOT NULL
    ALTER TABLE dbo.Avis DROP CONSTRAINT IF EXISTS FK_Avis_Annonces, FK_Avis_Utilisateurs;

IF OBJECT_ID('dbo.PasswordResetTokens', 'U') IS NOT NULL
    ALTER TABLE dbo.PasswordResetTokens DROP CONSTRAINT IF EXISTS FK_PasswordResetTokens_Utilisateurs;

IF OBJECT_ID('dbo.ContactsAnnonceur', 'U') IS NOT NULL
    ALTER TABLE dbo.ContactsAnnonceur DROP CONSTRAINT IF EXISTS FK_ContactsAnnonceur_Utilisateurs, FK_ContactsAnnonceur_Annonces, FK_ContactsAnnonceur_Annonceurs;

IF OBJECT_ID('dbo.AbonnementsAnnonceur', 'U') IS NOT NULL
    ALTER TABLE dbo.AbonnementsAnnonceur DROP CONSTRAINT IF EXISTS FK_AbonnementsAnnonceur_Utilisateurs, FK_AbonnementsAnnonceur_Annonceurs;

IF OBJECT_ID('dbo.Favoris', 'U') IS NOT NULL
    ALTER TABLE dbo.Favoris DROP CONSTRAINT IF EXISTS FK_Favoris_Utilisateurs, FK_Favoris_Annonces;

IF OBJECT_ID('dbo.ImagesAnnonce', 'U') IS NOT NULL
    ALTER TABLE dbo.ImagesAnnonce DROP CONSTRAINT IF EXISTS FK_ImagesAnnonce_Annonces;

IF OBJECT_ID('dbo.ValeursAttributAnnonce', 'U') IS NOT NULL
    ALTER TABLE dbo.ValeursAttributAnnonce DROP CONSTRAINT IF EXISTS FK_ValeursAttributAnnonce_Annonces, FK_ValeursAttributAnnonce_AttributsCategorie, FK_ValeursAttributAnnonce_OptionsAttributCategorie;

IF OBJECT_ID('dbo.Annonces', 'U') IS NOT NULL
    ALTER TABLE dbo.Annonces DROP CONSTRAINT IF EXISTS FK_Annonces_Utilisateurs, FK_Annonces_Categories;

IF OBJECT_ID('dbo.OptionsAttributCategorie', 'U') IS NOT NULL
    ALTER TABLE dbo.OptionsAttributCategorie DROP CONSTRAINT IF EXISTS FK_OptionsAttributCategorie_AttributsCategorie;

IF OBJECT_ID('dbo.AttributsCategorie', 'U') IS NOT NULL
    ALTER TABLE dbo.AttributsCategorie DROP CONSTRAINT IF EXISTS FK_AttributsCategorie_Categories;

IF OBJECT_ID('dbo.DemandesAnnonceur', 'U') IS NOT NULL
    ALTER TABLE dbo.DemandesAnnonceur DROP CONSTRAINT IF EXISTS FK_DemandesAnnonceur_Utilisateurs, FK_DemandesAnnonceur_Admins;

-- Drop tables in order of dependency
DROP TABLE IF EXISTS dbo.Signalements;
DROP TABLE IF EXISTS dbo.PaiementsCommandes;
DROP TABLE IF EXISTS dbo.Commandes;
DROP TABLE IF EXISTS dbo.PaiementsAnnonceur;
DROP TABLE IF EXISTS dbo.Avis;
DROP TABLE IF EXISTS dbo.PasswordResetTokens;
DROP TABLE IF EXISTS dbo.ContactsAnnonceur;
DROP TABLE IF EXISTS dbo.AbonnementsAnnonceur;
DROP TABLE IF EXISTS dbo.Favoris;
DROP TABLE IF EXISTS dbo.ImagesAnnonce;
DROP TABLE IF EXISTS dbo.ValeursAttributAnnonce;
DROP TABLE IF EXISTS dbo.Annonces;
DROP TABLE IF EXISTS dbo.OptionsAttributCategorie;
DROP TABLE IF EXISTS dbo.AttributsCategorie;
DROP TABLE IF EXISTS dbo.Categories;
DROP TABLE IF EXISTS dbo.DemandesAnnonceur;
DROP TABLE IF EXISTS dbo.Utilisateurs;

PRINT 'Teardown completed successfully. Database BigDealsDb is now empty.';
GO
