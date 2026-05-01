USE BigDealsDb;
GO

-- 1. Utilisateurs
IF OBJECT_ID('Utilisateurs', 'U') IS NULL
BEGIN
    CREATE TABLE Utilisateurs (
        IdUtilisateur BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nom NVARCHAR(100) NOT NULL,
        Prenom NVARCHAR(100) NOT NULL,
        Email NVARCHAR(256) NOT NULL,
        Telephone NVARCHAR(30) NULL,
        MotDePasseHash NVARCHAR(500) NOT NULL,
        Role INT NOT NULL, -- 1=CLIENT, 2=ANNONCEUR, 3=ADMIN
        StatutCompte INT NOT NULL, -- 1=ACTIF, 2=INACTIF, 3=EN_ATTENTE, 4=BLOQUE
        DateCreation DATETIME2 NOT NULL,
        DerniereConnexion DATETIME2 NULL,
        PhotoProfilUrl NVARCHAR(500) NULL,
        Adresse NVARCHAR(300) NULL,
        EstActif BIT NOT NULL DEFAULT 1
    );
END
GO

-- 2. DemandesAnnonceur
IF OBJECT_ID('DemandesAnnonceur', 'U') IS NULL
BEGIN
    CREATE TABLE DemandesAnnonceur (
        IdDemandeAnnonceur BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdUtilisateur BIGINT NOT NULL,
        Statut INT NOT NULL, -- 1=EN_ATTENTE, 2=APPROUVEE, 3=REJETEE
        DocumentUrl NVARCHAR(500) NOT NULL,
        DocumentNomOriginal NVARCHAR(255) NOT NULL,
        DocumentType NVARCHAR(100) NOT NULL,
        DocumentTaille BIGINT NOT NULL,
        MotifRejet NVARCHAR(1000) NULL,
        DateDemande DATETIME2 NOT NULL,
        DateTraitement DATETIME2 NULL,
        IdAdminTraitant BIGINT NULL
    );
END
GO

-- 3. Categories
IF OBJECT_ID('Categories', 'U') IS NULL
BEGIN
    CREATE TABLE Categories (
        IdCategorie INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nom NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IconKey NVARCHAR(100) NULL,
        OrdreAffichage INT NOT NULL,
        EstActive BIT NOT NULL DEFAULT 1,
        DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

-- 4. AttributsCategorie
IF OBJECT_ID('AttributsCategorie', 'U') IS NULL
BEGIN
    CREATE TABLE AttributsCategorie (
        IdAttributCategorie INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdCategorie INT NOT NULL,
        Nom NVARCHAR(100) NOT NULL,
        TypeDonnee INT NOT NULL,
        Obligatoire BIT NOT NULL DEFAULT 0,
        Filtrable BIT NOT NULL DEFAULT 1,
        OrdreAffichage INT NOT NULL,
        EstActive BIT NOT NULL DEFAULT 1
    );
END
GO

-- 5. OptionsAttributCategorie
IF OBJECT_ID('OptionsAttributCategorie', 'U') IS NULL
BEGIN
    CREATE TABLE OptionsAttributCategorie (
        IdOptionAttributCategorie INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdAttributCategorie INT NOT NULL,
        Valeur NVARCHAR(150) NOT NULL,
        OrdreAffichage INT NOT NULL,
        EstActive BIT NOT NULL DEFAULT 1
    );
END
GO

-- 6. Annonces
IF OBJECT_ID('Annonces', 'U') IS NULL
BEGIN
    CREATE TABLE Annonces (
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

-- 7. ValeursAttributAnnonce
IF OBJECT_ID('ValeursAttributAnnonce', 'U') IS NULL
BEGIN
    CREATE TABLE ValeursAttributAnnonce (
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

-- 8. ImagesAnnonce
IF OBJECT_ID('ImagesAnnonce', 'U') IS NULL
BEGIN
    CREATE TABLE ImagesAnnonce (
        IdImageAnnonce BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdAnnonce BIGINT NOT NULL,
        Url NVARCHAR(500) NOT NULL,
        OrdreAffichage INT NOT NULL DEFAULT 1,
        EstPrincipale BIT NOT NULL DEFAULT 0,
        DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO
