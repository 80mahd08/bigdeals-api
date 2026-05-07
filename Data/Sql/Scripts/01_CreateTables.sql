USE BigDealsDb;
GO

------------------------------------------------------------
-- 1. Utilisateurs
------------------------------------------------------------
IF OBJECT_ID('dbo.Utilisateurs', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Utilisateurs (
        IdUtilisateur BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nom NVARCHAR(100) NOT NULL,
        Prenom NVARCHAR(100) NOT NULL,
        Email NVARCHAR(256) NOT NULL,
        Telephone NVARCHAR(30) NULL,
        MotDePasseHash NVARCHAR(500) NOT NULL,
        Role INT NOT NULL, -- 1=CLIENT, 2=ANNONCEUR, 3=ADMIN
        StatutCompte INT NOT NULL, -- 1=ACTIF, 2=INACTIF, 3=EN_ATTENTE, 4=BLOQUE
        DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        DerniereConnexion DATETIME2 NULL,
        PhotoProfilUrl NVARCHAR(500) NULL,
        Adresse NVARCHAR(300) NULL,
        EstActif BIT NOT NULL DEFAULT 1
    );
END
GO

------------------------------------------------------------
-- 2. DemandesAnnonceur
------------------------------------------------------------
IF OBJECT_ID('dbo.DemandesAnnonceur', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DemandesAnnonceur (
        IdDemandeAnnonceur BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdUtilisateur BIGINT NOT NULL,
        Statut INT NOT NULL, -- 1=EN_ATTENTE, 2=APPROUVEE, 3=REJETEE
        DocumentUrl NVARCHAR(500) NOT NULL,
        DocumentNomOriginal NVARCHAR(255) NOT NULL,
        DocumentType NVARCHAR(100) NOT NULL,
        DocumentTaille BIGINT NOT NULL,
        MotifRejet NVARCHAR(1000) NULL,
        DateDemande DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        DateTraitement DATETIME2 NULL,
        IdAdminTraitant BIGINT NULL
    );
END
GO

------------------------------------------------------------
-- 3. Categories - FINAL CLEAN VERSION
------------------------------------------------------------
IF OBJECT_ID('dbo.Categories', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories (
        IdCategorie INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nom NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IconKey NVARCHAR(100) NULL,
        OrdreAffichage INT NOT NULL,
        DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

------------------------------------------------------------
-- 4. AttributsCategorie - FINAL CLEAN VERSION
-- Removed: Obligatoire, Filtrable, EstActive
-- Kept: EstPlage
------------------------------------------------------------
IF OBJECT_ID('dbo.AttributsCategorie', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AttributsCategorie (
        IdAttributCategorie INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdCategorie INT NOT NULL,
        Nom NVARCHAR(100) NOT NULL,
        TypeDonnee INT NOT NULL, -- 1=TEXTE, 2=NOMBRE, 3=DATE, 4=BOOLEAN, 5=LISTE
        OrdreAffichage INT NOT NULL,
        Placeholder NVARCHAR(255) NULL,
        EstPlage BIT NOT NULL DEFAULT 0
    );
END
GO

------------------------------------------------------------
-- 5. OptionsAttributCategorie - FINAL CLEAN VERSION
-- Removed: EstActive
------------------------------------------------------------
IF OBJECT_ID('dbo.OptionsAttributCategorie', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OptionsAttributCategorie (
        IdOptionAttributCategorie INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdAttributCategorie INT NOT NULL,
        Valeur NVARCHAR(150) NOT NULL,
        OrdreAffichage INT NOT NULL
    );
END
GO

------------------------------------------------------------
-- 6. Annonces
------------------------------------------------------------
IF OBJECT_ID('dbo.Annonces', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Annonces (
        IdAnnonce BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdUtilisateur BIGINT NOT NULL,
        IdCategorie INT NOT NULL,
        Titre NVARCHAR(150) NOT NULL,
        Description NVARCHAR(2000) NOT NULL,
        Prix DECIMAL(18,3) NOT NULL,
        Localisation NVARCHAR(255) NOT NULL,
        Statut INT NOT NULL, -- 1=BROUILLON, 2=PUBLIEE, 3=SUSPENDUE, 4=EXPIREE, 5=SUPPRIMEE
        DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        DatePublication DATETIME2 NULL,
        DateExpiration DATETIME2 NULL,
        EstActive BIT NOT NULL DEFAULT 1
    );
END
GO

------------------------------------------------------------
-- 7. ValeursAttributAnnonce
------------------------------------------------------------
IF OBJECT_ID('dbo.ValeursAttributAnnonce', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ValeursAttributAnnonce (
        IdValeurAttributAnnonce BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdAnnonce BIGINT NOT NULL,
        IdAttributCategorie INT NOT NULL,
        IdOptionAttributCategorie INT NULL,
        ValeurTexte NVARCHAR(1000) NULL,
        ValeurNombre DECIMAL(18,3) NULL,
        ValeurDate DATE NULL,
        ValeurBooleen BIT NULL
    );
END
GO

------------------------------------------------------------
-- 8. ImagesAnnonce
------------------------------------------------------------
IF OBJECT_ID('dbo.ImagesAnnonce', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ImagesAnnonce (
        IdImageAnnonce BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdAnnonce BIGINT NOT NULL,
        Url NVARCHAR(500) NOT NULL,
        OrdreAffichage INT NOT NULL DEFAULT 1,
        EstPrincipale BIT NOT NULL DEFAULT 0,
        DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

------------------------------------------------------------
-- 9. Favoris
------------------------------------------------------------
IF OBJECT_ID('dbo.Favoris', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Favoris (
        IdFavori BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdUtilisateur BIGINT NOT NULL,
        IdAnnonce BIGINT NOT NULL,
        DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_Favoris_Utilisateur_Annonce UNIQUE (IdUtilisateur, IdAnnonce)
    );
END
GO

------------------------------------------------------------
-- 10. AbonnementsAnnonceur
------------------------------------------------------------
IF OBJECT_ID('dbo.AbonnementsAnnonceur', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AbonnementsAnnonceur (
        IdAbonnementAnnonceur BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdUtilisateur BIGINT NOT NULL,
        IdAnnonceur BIGINT NOT NULL,
        DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_AbonnementsAnnonceur_Utilisateur_Annonceur UNIQUE (IdUtilisateur, IdAnnonceur)
    );
END
GO

------------------------------------------------------------
-- 11. ContactsAnnonceur
------------------------------------------------------------
IF OBJECT_ID('dbo.ContactsAnnonceur', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ContactsAnnonceur (
        IdContactAnnonceur BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdUtilisateur BIGINT NULL,
        IdAnnonce BIGINT NOT NULL,
        IdAnnonceur BIGINT NOT NULL,
        TypeContact INT NOT NULL, -- 1=TELEPHONE, 2=WHATSAPP
        DateContact DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

------------------------------------------------------------
-- 12. PasswordResetTokens
------------------------------------------------------------
IF OBJECT_ID('dbo.PasswordResetTokens', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PasswordResetTokens (
        IdPasswordResetToken BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdUtilisateur BIGINT NOT NULL,
        TokenHash NVARCHAR(500) NOT NULL,
        DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        DateExpiration DATETIME2 NOT NULL,
        DateUtilisation DATETIME2 NULL,
        EstUtilise BIT NOT NULL DEFAULT 0
    );
END
GO
