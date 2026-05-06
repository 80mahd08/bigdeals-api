USE BigDealsDb;
GO

IF OBJECT_ID('ContactsAnnonceur', 'U') IS NOT NULL DROP TABLE ContactsAnnonceur;
IF OBJECT_ID('AbonnementsAnnonceur', 'U') IS NOT NULL DROP TABLE AbonnementsAnnonceur;
IF OBJECT_ID('Favoris', 'U') IS NOT NULL DROP TABLE Favoris;
IF OBJECT_ID('PasswordResetTokens', 'U') IS NOT NULL DROP TABLE PasswordResetTokens;
IF OBJECT_ID('Avis', 'U') IS NOT NULL DROP TABLE Avis;
IF OBJECT_ID('DemandesAnnonceur', 'U') IS NOT NULL DROP TABLE DemandesAnnonceur;
IF OBJECT_ID('OptionsAttributCategorie', 'U') IS NOT NULL DROP TABLE OptionsAttributCategorie;
IF OBJECT_ID('ValeursAttributAnnonce', 'U') IS NOT NULL DROP TABLE ValeursAttributAnnonce;
IF OBJECT_ID('ImagesAnnonce', 'U') IS NOT NULL DROP TABLE ImagesAnnonce;
IF OBJECT_ID('Annonces', 'U') IS NOT NULL DROP TABLE Annonces;
IF OBJECT_ID('AttributsCategorie', 'U') IS NOT NULL DROP TABLE AttributsCategorie;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('Utilisateurs', 'U') IS NOT NULL DROP TABLE Utilisateurs;
GO
