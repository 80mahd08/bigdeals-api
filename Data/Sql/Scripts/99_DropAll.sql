USE BigDealsDb;
GO

------------------------------------------------------------
-- Drop all project tables in dependency-safe order
------------------------------------------------------------
IF OBJECT_ID('dbo.ContactsAnnonceur', 'U') IS NOT NULL DROP TABLE dbo.ContactsAnnonceur;
IF OBJECT_ID('dbo.AbonnementsAnnonceur', 'U') IS NOT NULL DROP TABLE dbo.AbonnementsAnnonceur;
IF OBJECT_ID('dbo.Favoris', 'U') IS NOT NULL DROP TABLE dbo.Favoris;
IF OBJECT_ID('dbo.PasswordResetTokens', 'U') IS NOT NULL DROP TABLE dbo.PasswordResetTokens;
IF OBJECT_ID('dbo.Avis', 'U') IS NOT NULL DROP TABLE dbo.Avis;
IF OBJECT_ID('dbo.ImagesAnnonce', 'U') IS NOT NULL DROP TABLE dbo.ImagesAnnonce;
IF OBJECT_ID('dbo.ValeursAttributAnnonce', 'U') IS NOT NULL DROP TABLE dbo.ValeursAttributAnnonce;
IF OBJECT_ID('dbo.DemandesAnnonceur', 'U') IS NOT NULL DROP TABLE dbo.DemandesAnnonceur;
IF OBJECT_ID('dbo.Annonces', 'U') IS NOT NULL DROP TABLE dbo.Annonces;
IF OBJECT_ID('dbo.OptionsAttributCategorie', 'U') IS NOT NULL DROP TABLE dbo.OptionsAttributCategorie;
IF OBJECT_ID('dbo.AttributsCategorie', 'U') IS NOT NULL DROP TABLE dbo.AttributsCategorie;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Utilisateurs', 'U') IS NOT NULL DROP TABLE dbo.Utilisateurs;
GO
