-- ============================================================================
-- BIGDEALS - CONSOLIDATED DATABASE SETUP SCRIPT
-- ============================================================================
-- Goal: Set up the BigDealsDb database, all tables, indices, checks, and initial
-- seeds (admin user and predefined categories) in a single run.
-- Safe to run on blank or existing SQL Server instances.
-- ============================================================================

USE master;
GO

-- 1. Create Database if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'BigDealsDb')
BEGIN
    CREATE DATABASE [BigDealsDb];
    PRINT 'Created database BigDealsDb.';
END
ELSE
BEGIN
    PRINT 'Database BigDealsDb already exists.';
END
GO

USE BigDealsDb;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ============================================================================
-- SECTION A: DROP ALL TABLES IF THEY EXIST (FOR FRESH RE-INSTALL)
-- ============================================================================
PRINT 'Performing clean drop of existing tables...';

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

PRINT 'Existing tables successfully dropped.';
GO

-- ============================================================================
-- SECTION B: CREATE CORE TABLES
-- ============================================================================
PRINT 'Creating core tables...';

-- 1. Utilisateurs
CREATE TABLE dbo.Utilisateurs (
    IdUtilisateur BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nom NVARCHAR(100) NOT NULL,
    Prenom NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Telephone NVARCHAR(30) NULL,
    MotDePasseHash NVARCHAR(500) NOT NULL,
    Role INT NOT NULL, -- 1=CLIENT, 2=ANNONCEUR, 3=ADMIN
    StatutCompte INT NOT NULL, -- 1=ACTIF, 2=BLOQUE
    DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    PhotoProfilUrl NVARCHAR(500) NULL,
    Adresse NVARCHAR(300) NULL,
    Ville NVARCHAR(100) NULL,
    RefreshToken NVARCHAR(500) NULL,
    RefreshTokenExpiry DATETIME2 NULL,
    CONSTRAINT UQ_Utilisateurs_Email UNIQUE (Email),
    CONSTRAINT CHK_Utilisateurs_Role CHECK (Role IN (1, 2, 3)),
    CONSTRAINT CHK_Utilisateurs_StatutCompte CHECK (StatutCompte IN (1, 2))
);
GO

-- 2. DemandesAnnonceur
CREATE TABLE dbo.DemandesAnnonceur (
    IdDemandeAnnonceur BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUtilisateur BIGINT NOT NULL,
    Statut INT NOT NULL, -- 1=EN_ATTENTE_VERIFICATION, 2=APPROUVEE, 3=REJETEE, 4=EN_ATTENTE_PAIEMENT
    DocumentUrl NVARCHAR(500) NOT NULL,
    DocumentNomOriginal NVARCHAR(255) NOT NULL,
    DocumentType NVARCHAR(100) NOT NULL,
    DocumentTaille BIGINT NOT NULL,
    MotifRejet NVARCHAR(1000) NULL,
    DateDemande DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DateTraitement DATETIME2 NULL,
    IdAdminTraitant BIGINT NULL,
    CONSTRAINT FK_DemandesAnnonceur_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT FK_DemandesAnnonceur_Admins FOREIGN KEY (IdAdminTraitant) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT CHK_DemandesAnnonceur_Statut CHECK (Statut IN (1, 2, 3, 4))
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_DemandesAnnonceur_EnAttente_PerUser
ON dbo.DemandesAnnonceur(IdUtilisateur)
WHERE Statut IN (1, 4);
GO

-- 3. Categories
CREATE TABLE dbo.Categories (
    IdCategorie INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nom NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    IconKey NVARCHAR(100) NULL,
    OrdreAffichage INT NOT NULL,
    SupportePaiement BIT NOT NULL DEFAULT 0,
    DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_Categories_Nom UNIQUE (Nom)
);
GO

-- 4. AttributsCategorie
CREATE TABLE dbo.AttributsCategorie (
    IdAttributCategorie INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdCategorie INT NOT NULL,
    Nom NVARCHAR(100) NOT NULL,
    TypeDonnee INT NOT NULL, -- 1=TEXTE, 2=NOMBRE, 3=DATE, 4=BOOLEAN, 5=LISTE
    OrdreAffichage INT NOT NULL,
    Placeholder NVARCHAR(255) NULL,
    EstPlage BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_AttributsCategorie_Categories FOREIGN KEY (IdCategorie) REFERENCES dbo.Categories(IdCategorie),
    CONSTRAINT UQ_AttributsCategorie_Categorie_Nom UNIQUE (IdCategorie, Nom),
    CONSTRAINT CHK_AttributsCategorie_TypeDonnee CHECK (TypeDonnee IN (1, 2, 3, 4, 5)),
    CONSTRAINT CHK_AttributsCategorie_OrdreAffichage CHECK (OrdreAffichage > 0)
);
GO

-- 5. OptionsAttributCategorie
CREATE TABLE dbo.OptionsAttributCategorie (
    IdOptionAttributCategorie INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdAttributCategorie INT NOT NULL,
    Valeur NVARCHAR(150) NOT NULL,
    OrdreAffichage INT NOT NULL,
    CONSTRAINT FK_OptionsAttributCategorie_AttributsCategorie FOREIGN KEY (IdAttributCategorie) REFERENCES dbo.AttributsCategorie(IdAttributCategorie),
    CONSTRAINT UQ_OptionsAttributCategorie_Attribut_Valeur UNIQUE (IdAttributCategorie, Valeur),
    CONSTRAINT CHK_OptionsAttributCategorie_OrdreAffichage CHECK (OrdreAffichage > 0)
);
GO

-- 6. Annonces
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
    EstActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Annonces_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT FK_Annonces_Categories FOREIGN KEY (IdCategorie) REFERENCES dbo.Categories(IdCategorie),
    CONSTRAINT CHK_Annonces_Prix CHECK (Prix >= 0),
    CONSTRAINT CHK_Annonces_Statut CHECK (Statut IN (1, 2, 3, 4, 5))
);
GO

-- 7. ValeursAttributAnnonce
CREATE TABLE dbo.ValeursAttributAnnonce (
    IdValeurAttributAnnonce BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdAnnonce BIGINT NOT NULL,
    IdAttributCategorie INT NOT NULL,
    IdOptionAttributCategorie INT NULL,
    ValeurTexte NVARCHAR(1000) NULL,
    ValeurNombre DECIMAL(18,3) NULL,
    ValeurDate DATE NULL,
    ValeurBooleen BIT NULL,
    CONSTRAINT FK_ValeursAttributAnnonce_Annonces FOREIGN KEY (IdAnnonce) REFERENCES dbo.Annonces(IdAnnonce),
    CONSTRAINT FK_ValeursAttributAnnonce_AttributsCategorie FOREIGN KEY (IdAttributCategorie) REFERENCES dbo.AttributsCategorie(IdAttributCategorie),
    CONSTRAINT FK_ValeursAttributAnnonce_OptionsAttributCategorie FOREIGN KEY (IdOptionAttributCategorie) REFERENCES dbo.OptionsAttributCategorie(IdOptionAttributCategorie),
    CONSTRAINT UQ_ValeursAttributAnnonce_Annonce_Attribut UNIQUE (IdAnnonce, IdAttributCategorie)
);
GO

-- 8. ImagesAnnonce
CREATE TABLE dbo.ImagesAnnonce (
    IdImageAnnonce BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdAnnonce BIGINT NOT NULL,
    Url NVARCHAR(500) NOT NULL,
    OrdreAffichage INT NOT NULL DEFAULT 1,
    EstPrincipale BIT NOT NULL DEFAULT 0,
    DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_ImagesAnnonce_Annonces FOREIGN KEY (IdAnnonce) REFERENCES dbo.Annonces(IdAnnonce)
);
GO

-- 9. Favoris
CREATE TABLE dbo.Favoris (
    IdFavori BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUtilisateur BIGINT NOT NULL,
    IdAnnonce BIGINT NOT NULL,
    DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Favoris_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT FK_Favoris_Annonces FOREIGN KEY (IdAnnonce) REFERENCES dbo.Annonces(IdAnnonce),
    CONSTRAINT UQ_Favoris_Utilisateur_Annonce UNIQUE (IdUtilisateur, IdAnnonce)
);
GO

-- 10. AbonnementsAnnonceur
CREATE TABLE dbo.AbonnementsAnnonceur (
    IdAbonnementAnnonceur BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUtilisateur BIGINT NOT NULL,
    IdAnnonceur BIGINT NOT NULL,
    DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_AbonnementsAnnonceur_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT FK_AbonnementsAnnonceur_Annonceurs FOREIGN KEY (IdAnnonceur) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT UQ_AbonnementsAnnonceur_Utilisateur_Annonceur UNIQUE (IdUtilisateur, IdAnnonceur)
);
GO

-- 11. ContactsAnnonceur
CREATE TABLE dbo.ContactsAnnonceur (
    IdContactAnnonceur BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUtilisateur BIGINT NULL,
    IdAnnonce BIGINT NOT NULL,
    IdAnnonceur BIGINT NOT NULL,
    TypeContact INT NOT NULL, -- 1=TELEPHONE, 2=WHATSAPP
    DateContact DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_ContactsAnnonceur_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT FK_ContactsAnnonceur_Annonces FOREIGN KEY (IdAnnonce) REFERENCES dbo.Annonces(IdAnnonce),
    CONSTRAINT FK_ContactsAnnonceur_Annonceurs FOREIGN KEY (IdAnnonceur) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT CHK_ContactsAnnonceur_TypeContact CHECK (TypeContact IN (1, 2))
);
GO

-- 12. PasswordResetTokens
CREATE TABLE dbo.PasswordResetTokens (
    IdPasswordResetToken BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUtilisateur BIGINT NOT NULL,
    TokenHash NVARCHAR(500) NOT NULL,
    DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DateExpiration DATETIME2 NOT NULL,
    DateUtilisation DATETIME2 NULL,
    EstUtilise BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_PasswordResetTokens_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur)
);
GO

-- 13. Avis
CREATE TABLE dbo.Avis (
    IdAvis BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdAnnonce BIGINT NOT NULL,
    IdUtilisateur BIGINT NOT NULL,
    Note INT NOT NULL, -- Rating from 1 to 5
    Commentaire NVARCHAR(2000) NOT NULL,
    DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DateModification DATETIME2 NULL,
    EstActif BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Avis_Annonces FOREIGN KEY (IdAnnonce) REFERENCES dbo.Annonces(IdAnnonce),
    CONSTRAINT FK_Avis_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT UQ_Avis_Annonce_Utilisateur UNIQUE (IdAnnonce, IdUtilisateur),
    CONSTRAINT CHK_Avis_Note CHECK (Note BETWEEN 1 AND 5)
);
GO

-- 14. PaiementsAnnonceur
CREATE TABLE dbo.PaiementsAnnonceur (
    IdPaiementAnnonceur BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUtilisateur BIGINT NOT NULL,
    IdDemandeAnnonceur BIGINT NOT NULL,
    Provider NVARCHAR(50) NOT NULL,
    ProviderPaymentId NVARCHAR(200) NULL,
    DeveloperTrackingId NVARCHAR(200) NOT NULL,
    Montant DECIMAL(18,3) NOT NULL,
    StatutPaiement INT NOT NULL,
    PaymentUrl NVARCHAR(1000) NULL,
    RawResponseJson NVARCHAR(MAX) NULL,
    DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DateConfirmation DATETIME2 NULL,
    CONSTRAINT FK_PaiementsAnnonceur_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT FK_PaiementsAnnonceur_DemandesAnnonceur FOREIGN KEY (IdDemandeAnnonceur) REFERENCES dbo.DemandesAnnonceur(IdDemandeAnnonceur),
    CONSTRAINT UQ_PaiementsAnnonceur_DeveloperTrackingId UNIQUE (DeveloperTrackingId),
    CONSTRAINT UQ_PaiementsAnnonceur_IdDemandeAnnonceur UNIQUE (IdDemandeAnnonceur),
    CONSTRAINT CK_PaiementsAnnonceur_Montant_Positive CHECK (Montant > 0),
    CONSTRAINT CK_PaiementsAnnonceur_StatutPaiement CHECK (StatutPaiement IN (1, 2, 3, 4)),
    CONSTRAINT CK_PaiementsAnnonceur_Montant_AnnonceurFee CHECK (Montant = CAST(200.000 AS DECIMAL(18,3)))
);
GO

-- 15. Commandes
CREATE TABLE dbo.Commandes (
    IdCommande BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdAnnonce BIGINT NOT NULL,
    IdAcheteur BIGINT NOT NULL,
    IdAnnonceur BIGINT NOT NULL,
    Montant DECIMAL(18,3) NOT NULL,
    MontantAnnonce DECIMAL(18,3) NOT NULL DEFAULT 0,
    FraisLivraison DECIMAL(18,3) NOT NULL DEFAULT 0,
    StatutCommande INT NOT NULL, -- 1=EN_ATTENTE_PAIEMENT, 2=PAYEE, 3=ANNULEE
    StatutLivraison INT NOT NULL DEFAULT 1, -- 1-7 lifecycle
    AdresseLivraison NVARCHAR(300) NULL,
    VilleLivraison NVARCHAR(100) NULL,
    TelephoneLivraison NVARCHAR(30) NULL,
    DateExpedition DATETIME2 NULL,
    DateLivraison DATETIME2 NULL,
    DateDerniereMiseAJourLivraison DATETIME2 NULL,
    DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Commandes_Annonces FOREIGN KEY (IdAnnonce) REFERENCES dbo.Annonces(IdAnnonce),
    CONSTRAINT FK_Commandes_Acheteur FOREIGN KEY (IdAcheteur) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT FK_Commandes_Annonceur FOREIGN KEY (IdAnnonceur) REFERENCES dbo.Utilisateurs(IdUtilisateur),
    CONSTRAINT CHK_Commandes_Montant CHECK (Montant > 0),
    CONSTRAINT CHK_Commandes_Statut CHECK (StatutCommande IN (1, 2, 3)),
    CONSTRAINT CHK_Commandes_StatutLivraison CHECK (StatutLivraison IN (1, 2, 3, 4, 5, 6, 7))
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_Commandes_Pending_Acheteur_Annonce
ON dbo.Commandes(IdAcheteur, IdAnnonce)
WHERE StatutCommande = 1;
GO

-- 16. PaiementsCommandes
CREATE TABLE dbo.PaiementsCommandes (
    IdPaiementCommande BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdCommande BIGINT NOT NULL,
    Montant DECIMAL(18,3) NOT NULL,
    MethodePaiement NVARCHAR(50) NOT NULL,
    StatutPaiement INT NOT NULL, -- 1=EN_ATTENTE, 2=ACCEPTE, 3=REFUSE
    NumeroCarteMasque NVARCHAR(30) NULL,
    DatePaiement DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_PaiementsCommandes_Commandes FOREIGN KEY (IdCommande) REFERENCES dbo.Commandes(IdCommande),
    CONSTRAINT CHK_PaiementsCommandes_Montant CHECK (Montant > 0),
    CONSTRAINT CHK_PaiementsCommandes_Statut CHECK (StatutPaiement IN (1, 2, 3))
);
GO

-- 17. Signalements
CREATE TABLE [dbo].[Signalements] (
    [IdSignalement] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [IdAnnonce] BIGINT NOT NULL,
    [IdUtilisateur] BIGINT NOT NULL,
    [TypeSignalement] INT NOT NULL,
    [Motif] NVARCHAR(1000) NOT NULL,
    [Statut] INT NOT NULL DEFAULT 1,
    [DateCreation] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    [DateTraitement] DATETIME2 NULL,
    [IdAdminTraitant] BIGINT NULL,
    CONSTRAINT [FK_Signalements_Annonces] FOREIGN KEY ([IdAnnonce]) REFERENCES [dbo].[Annonces]([IdAnnonce]),
    CONSTRAINT [FK_Signalements_Utilisateurs] FOREIGN KEY ([IdUtilisateur]) REFERENCES [dbo].[Utilisateurs]([IdUtilisateur]),
    CONSTRAINT [FK_Signalements_Admins] FOREIGN KEY ([IdAdminTraitant]) REFERENCES [dbo].[Utilisateurs]([IdUtilisateur]),
    CONSTRAINT [CHK_Signalements_TypeSignalement] CHECK ([TypeSignalement] IN (1, 2, 3, 4)),
    CONSTRAINT [CHK_Signalements_Statut] CHECK ([Statut] IN (1, 2, 3)),
    CONSTRAINT [UQ_Signalements_Annonce_Utilisateur] UNIQUE ([IdAnnonce], [IdUtilisateur])
);
GO

PRINT 'Core tables and initial indexes built successfully.';
GO

-- ============================================================================
-- SECTION C: CREATE REMAINING INDEXES
-- ============================================================================
PRINT 'Creating supplementary performance indexes...';

CREATE NONCLUSTERED INDEX IX_Utilisateurs_Email ON dbo.Utilisateurs(Email);
CREATE NONCLUSTERED INDEX IX_DemandesAnnonceur_IdUtilisateur ON dbo.DemandesAnnonceur(IdUtilisateur);
CREATE NONCLUSTERED INDEX IX_DemandesAnnonceur_Statut ON dbo.DemandesAnnonceur(Statut);
CREATE NONCLUSTERED INDEX IX_PasswordResetTokens_IdUtilisateur ON dbo.PasswordResetTokens(IdUtilisateur);
CREATE NONCLUSTERED INDEX IX_PasswordResetTokens_TokenHash ON dbo.PasswordResetTokens(TokenHash);
CREATE NONCLUSTERED INDEX IX_Categories_OrdreAffichage ON dbo.Categories(OrdreAffichage, Nom);
CREATE NONCLUSTERED INDEX IX_AttributsCategorie_IdCategorie_OrdreAffichage ON dbo.AttributsCategorie(IdCategorie, OrdreAffichage, Nom);
CREATE NONCLUSTERED INDEX IX_AttributsCategorie_TypeDonnee ON dbo.AttributsCategorie(TypeDonnee);
CREATE NONCLUSTERED INDEX IX_OptionsAttributCategorie_IdAttribut_OrdreAffichage ON dbo.OptionsAttributCategorie(IdAttributCategorie, OrdreAffichage, Valeur);
CREATE NONCLUSTERED INDEX IX_Annonces_IdUtilisateur ON dbo.Annonces(IdUtilisateur);
CREATE NONCLUSTERED INDEX IX_Annonces_IdCategorie ON dbo.Annonces(IdCategorie);
CREATE NONCLUSTERED INDEX IX_Annonces_Statut_EstActive ON dbo.Annonces(Statut, EstActive);
CREATE NONCLUSTERED INDEX IX_Annonces_DateCreation ON dbo.Annonces(DateCreation DESC);
CREATE NONCLUSTERED INDEX IX_Annonces_PublicSearch ON dbo.Annonces(IdCategorie, Statut, EstActive, DateCreation DESC) INCLUDE (Titre, Prix, Localisation);
CREATE NONCLUSTERED INDEX IX_ValeursAttributAnnonce_IdAnnonce ON dbo.ValeursAttributAnnonce(IdAnnonce);
CREATE NONCLUSTERED INDEX IX_ValeursAttributAnnonce_Attribut_Option ON dbo.ValeursAttributAnnonce(IdAttributCategorie, IdOptionAttributCategorie);
CREATE NONCLUSTERED INDEX IX_ValeursAttributAnnonce_Attribut_Nombre ON dbo.ValeursAttributAnnonce(IdAttributCategorie, ValeurNombre);
CREATE NONCLUSTERED INDEX IX_ValeursAttributAnnonce_Attribut_Date ON dbo.ValeursAttributAnnonce(IdAttributCategorie, ValeurDate);
CREATE NONCLUSTERED INDEX IX_ImagesAnnonce_IdAnnonce ON dbo.ImagesAnnonce(IdAnnonce, OrdreAffichage);
CREATE NONCLUSTERED INDEX IX_Favoris_IdUtilisateur ON dbo.Favoris(IdUtilisateur);
CREATE NONCLUSTERED INDEX IX_Favoris_IdAnnonce ON dbo.Favoris(IdAnnonce);
CREATE NONCLUSTERED INDEX IX_AbonnementsAnnonceur_IdUtilisateur ON dbo.AbonnementsAnnonceur(IdUtilisateur);
CREATE NONCLUSTERED INDEX IX_AbonnementsAnnonceur_IdAnnonceur ON dbo.AbonnementsAnnonceur(IdAnnonceur);
CREATE NONCLUSTERED INDEX IX_ContactsAnnonceur_IdAnnonceur ON dbo.ContactsAnnonceur(IdAnnonceur);
CREATE NONCLUSTERED INDEX IX_ContactsAnnonceur_IdUtilisateur ON dbo.ContactsAnnonceur(IdUtilisateur);
CREATE NONCLUSTERED INDEX IX_ContactsAnnonceur_IdAnnonce ON dbo.ContactsAnnonceur(IdAnnonce);
CREATE NONCLUSTERED INDEX IX_Avis_IdAnnonce ON dbo.Avis(IdAnnonce);
CREATE NONCLUSTERED INDEX IX_Avis_IdUtilisateur ON dbo.Avis(IdUtilisateur);
CREATE NONCLUSTERED INDEX IX_PaiementsAnnonceur_IdUtilisateur ON dbo.PaiementsAnnonceur(IdUtilisateur);
CREATE NONCLUSTERED INDEX IX_PaiementsAnnonceur_IdDemandeAnnonceur ON dbo.PaiementsAnnonceur(IdDemandeAnnonceur);
CREATE NONCLUSTERED INDEX IX_PaiementsAnnonceur_ProviderPaymentId ON dbo.PaiementsAnnonceur(ProviderPaymentId);
CREATE NONCLUSTERED INDEX IX_PaiementsAnnonceur_StatutPaiement ON dbo.PaiementsAnnonceur(StatutPaiement);
CREATE NONCLUSTERED INDEX IX_PaiementsAnnonceur_DateCreation ON dbo.PaiementsAnnonceur(DateCreation);
CREATE NONCLUSTERED INDEX IX_Commandes_IdAcheteur ON dbo.Commandes(IdAcheteur);
CREATE NONCLUSTERED INDEX IX_Commandes_IdAnnonce ON dbo.Commandes(IdAnnonce);
CREATE NONCLUSTERED INDEX IX_Commandes_StatutCommande ON dbo.Commandes(StatutCommande);
CREATE NONCLUSTERED INDEX IX_Commandes_StatutLivraison ON dbo.Commandes(StatutLivraison);
CREATE NONCLUSTERED INDEX IX_PaiementsCommandes_IdCommande ON dbo.PaiementsCommandes(IdCommande);
CREATE NONCLUSTERED INDEX IX_PaiementsCommandes_StatutPaiement ON dbo.PaiementsCommandes(StatutPaiement);
CREATE INDEX [IX_Signalements_Statut_DateCreation] ON [dbo].[Signalements] ([Statut], [DateCreation] DESC);
CREATE INDEX [IX_Signalements_IdAnnonce] ON [dbo].[Signalements] ([IdAnnonce]);
CREATE INDEX [IX_Signalements_IdUtilisateur] ON [dbo].[Signalements] ([IdUtilisateur]);

PRINT 'Supplementary indexes completed successfully.';
GO

-- ============================================================================
-- SECTION D: SEED INIITAL DATA (ADMIN)
-- ============================================================================
PRINT 'Seeding default administrator account...';

-- Email: admin@admin.com | Password: 123456789
INSERT INTO dbo.Utilisateurs
(
    Nom, Prenom, Email, MotDePasseHash, Role, StatutCompte, DateCreation
)
VALUES
(
    N'admin', N'admin', N'admin@admin.com',
    N'PBKDF2$100000$xz7LSmwzbI20+Pyb+lBebQ==$vintzW0EzLXlQdtJe7yJZbZHUUbLK1nRlh9S3kAF/lg=',
    3, -- ADMIN
    1, -- ACTIF
    SYSUTCDATETIME()
);

PRINT 'Initial admin seeded successfully.';
GO

-- ============================================================================
-- SECTION E: SEED DYNAMIC CATEGORIES AND ATTRIBUTES
-- ============================================================================
PRINT 'Seeding predefined categories system...';

DECLARE @TEXTE INT = 1;
DECLARE @NOMBRE INT = 2;
DECLARE @DATE INT = 3;
DECLARE @BOOLEAN INT = 4;
DECLARE @LISTE INT = 5;

-- 1. Create Temporary Seed Tables
IF OBJECT_ID('tempdb..#CategorySeed') IS NOT NULL DROP TABLE #CategorySeed;
IF OBJECT_ID('tempdb..#AttributeSeed') IS NOT NULL DROP TABLE #AttributeSeed;
IF OBJECT_ID('tempdb..#OptionSeed') IS NOT NULL DROP TABLE #OptionSeed;

CREATE TABLE #CategorySeed (
    Nom NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    IconKey NVARCHAR(100) NULL,
    OrdreAffichage INT NOT NULL,
    SupportePaiement BIT NOT NULL
);

CREATE TABLE #AttributeSeed (
    CategoryName NVARCHAR(100) NOT NULL,
    Nom NVARCHAR(100) NOT NULL,
    TypeDonnee INT NOT NULL,
    OrdreAffichage INT NOT NULL,
    Placeholder NVARCHAR(255) NULL,
    EstPlage BIT NOT NULL
);

CREATE TABLE #OptionSeed (
    CategoryName NVARCHAR(100) NOT NULL,
    AttributeName NVARCHAR(100) NOT NULL,
    Valeur NVARCHAR(150) NOT NULL,
    OrdreAffichage INT NOT NULL
);

-- 2. Insert Categories
INSERT INTO #CategorySeed (Nom, Description, IconKey, OrdreAffichage, SupportePaiement)
VALUES
(N'Véhicules', N'Voitures, motos, camions et autres véhicules.', N'ri-car-line', 1, 0),
(N'Immobilier', N'Maisons, appartements, terrains et biens immobiliers.', N'ri-home-4-line', 2, 0),
(N'Téléphones', N'Téléphones, smartphones et accessoires mobiles.', N'ri-smartphone-line', 3, 1),
(N'Informatique', N'Ordinateurs, composants, écrans et matériel informatique.', N'ri-computer-line', 4, 1),
(N'Mode', N'Vêtements, chaussures, accessoires et articles de mode.', N'ri-shirt-line', 5, 1),
(N'Beauté', N'Produits de beauté, soin, parfums et cosmétiques.', N'ri-magic-line', 6, 1),
(N'Maison', N'Meubles, décoration, électroménager et articles maison.', N'ri-home-heart-line', 7, 1),
(N'Jardin', N'Plantes, outils, mobilier et articles de jardin.', N'ri-leaf-line', 8, 1),
(N'Services', N'Services professionnels, réparation, transport et assistance.', N'ri-tools-line', 9, 0),
(N'Emploi', N'Offres d’emploi, stages, missions et opportunités de travail.', N'ri-briefcase-line', 10, 0);


-- 3. Insert Attributes
INSERT INTO #AttributeSeed (CategoryName, Nom, TypeDonnee, OrdreAffichage, Placeholder, EstPlage)
VALUES
-- Véhicules
(N'Vehicules', N'Type véhicule', 5, 1, NULL, 0),
(N'Véhicules', N'Marque', 1, 2, N'Ex: Toyota, BMW...', 0),
(N'Véhicules', N'Modèle', 1, 3, N'Ex: Corolla, Série 3...', 0),
(N'Véhicules', N'Année', 2, 4, N'Ex: 2026', 1),
(N'Véhicules', N'Carburant', 5, 5, NULL, 0),
(N'Véhicules', N'Boîte vitesse', 5, 6, NULL, 0),
(N'Véhicules', N'Puissance fiscale', 2, 7, N'CV', 1),
(N'Véhicules', N'Couleur', 1, 8, N'Ex: Noir, Blanc...', 0),
(N'Véhicules', N'État', 5, 9, NULL, 0),
(N'Véhicules', N'Climatisation', 4, 10, NULL, 0),
(N'Véhicules', N'Nombre portes', 2, 11, NULL, 0),
(N'Véhicules', N'Garantie', 4, 12, NULL, 0),
(N'Véhicules', N'Durée garantie', 2, 13, N'mois', 1),

-- Immobilier
(N'Immobilier', N'Type bien', 5, 1, NULL, 0),
(N'Immobilier', N'Transaction', 5, 2, NULL, 0),
(N'Immobilier', N'Surface', 2, 3, N'm²', 1),
(N'Immobilier', N'Nombre pièces', 2, 4, NULL, 1),
(N'Immobilier', N'Nombre chambres', 2, 5, NULL, 1),
(N'Immobilier', N'Nombre salles de bain', 2, 6, NULL, 1),
(N'Immobilier', N'Étage', 2, 7, NULL, 0),
(N'Immobilier', N'Meublé', 4, 8, NULL, 0),
(N'Immobilier', N'Ascenseur', 4, 9, NULL, 0),
(N'Immobilier', N'Garage', 4, 10, NULL, 0),
(N'Immobilier', N'Jardin', 4, 11, NULL, 0),
(N'Immobilier', N'Terrasse / Balcon', 4, 12, NULL, 0),
(N'Immobilier', N'Chauffage', 5, 13, NULL, 0),
(N'Immobilier', N'État du bien', 5, 14, NULL, 0),

-- Téléphones
(N'Téléphones', N'Marque', 1, 1, N'Ex: Samsung, Apple...', 0),
(N'Téléphones', N'Modèle', 1, 2, N'Ex: Galaxy S24, iPhone 15...', 0),
(N'Téléphones', N'État', 5, 3, NULL, 0),
(N'Téléphones', N'Stockage', 2, 4, N'GB', 1),
(N'Téléphones', N'RAM', 2, 5, N'GB', 1),
(N'Téléphones', N'Couleur', 1, 6, N'Ex: Noir, Bleu...', 0),
(N'Téléphones', N'Double SIM', 4, 7, NULL, 0),
(N'Téléphones', N'5G', 4, 8, NULL, 0),
(N'Téléphones', N'Chargeur inclus', 4, 9, NULL, 0),
(N'Téléphones', N'Boîte scellée', 4, 10, NULL, 0),
(N'Téléphones', N'Garantie', 4, 11, NULL, 0),
(N'Téléphones', N'Durée garantie', 2, 12, N'mois', 1),

-- Informatique
(N'Informatique', N'Type produit', 5, 1, NULL, 0),
(N'Informatique', N'Marque', 1, 2, N'Ex: HP, Dell, Asus...', 0),
(N'Informatique', N'Modèle', 1, 3, N'Ex: Latitude, MacBook Pro...', 0),
(N'Informatique', N'Processeur', 1, 4, N'Ex: i5, Ryzen 5...', 0),
(N'Informatique', N'RAM', 2, 5, N'GB', 1),
(N'Informatique', N'Stockage', 2, 6, N'GB', 1),
(N'Informatique', N'Carte graphique', 1, 7, N'Ex: RTX 3060, Intel Iris...', 0),
(N'Informatique', N'Taille écran', 2, 8, N'pouces', 1),
(N'Informatique', N'Résolution', 1, 9, N'Ex: 1920x1080', 0),
(N'Informatique', N'Système', 1, 10, N'Ex: Windows 11', 0),
(N'Informatique', N'État', 5, 11, NULL, 0),
(N'Informatique', N'Garantie', 4, 12, NULL, 0),
(N'Informatique', N'Durée garantie', 2, 13, N'mois', 1),

-- Mode
(N'Mode', N'Type article', 5, 1, NULL, 0),
(N'Mode', N'Genre', 5, 2, NULL, 0),
(N'Mode', N'Marque', 1, 3, N'Ex: Nike, Zara...', 0),
(N'Mode', N'Taille', 1, 4, N'Ex: M, L, 42...', 0),
(N'Mode', N'Couleur', 1, 5, N'Ex: Noir, Bleu...', 0),
(N'Mode', N'Matière', 1, 6, N'Ex: Coton, cuir...', 0),
(N'Mode', N'État', 5, 7, NULL, 0),
(N'Mode', N'Authenticité', 5, 8, NULL, 0),
(N'Mode', N'Saison', 5, 9, NULL, 0),

-- Beauté
(N'Beauté', N'Type produit', 5, 1, NULL, 0),
(N'Beauté', N'Marque', 1, 2, N'Ex: L''Oréal, Nivea...', 0),
(N'Beauté', N'Genre', 5, 3, NULL, 0),
(N'Beauté', N'État', 5, 4, NULL, 0),
(N'Beauté', N'Volume', 2, 5, N'ml', 1),
(N'Beauté', N'Type peau', 5, 6, NULL, 0),
(N'Beauté', N'Type cheveux', 5, 7, NULL, 0),
(N'Beauté', N'Bio / Naturel', 4, 8, NULL, 0),
(N'Beauté', N'Date expiration', 3, 9, NULL, 0),
(N'Beauté', N'Authenticité', 5, 10, NULL, 0),

-- Maison
(N'Maison', N'Type article', 5, 1, NULL, 0),
(N'Maison', N'Marque', 1, 2, N'Ex: IKEA, Samsung...', 0),
(N'Maison', N'Matière', 5, 3, NULL, 0),
(N'Maison', N'Couleur', 1, 4, N'Ex: Blanc, Gris...', 0),
(N'Maison', N'État', 5, 5, NULL, 0),
(N'Maison', N'Longueur', 2, 6, N'cm', 1),
(N'Maison', N'Largeur', 2, 7, N'cm', 1),
(N'Maison', N'Hauteur', 2, 8, N'cm', 1),
(N'Maison', N'Surface', 2, 9, N'm²', 1),
(N'Maison', N'Poids', 2, 10, N'kg', 1),
(N'Maison', N'Démontable', 4, 11, NULL, 0),
(N'Maison', N'Livraison possible', 4, 12, NULL, 0),
(N'Maison', N'Garantie', 4, 13, NULL, 0),
(N'Maison', N'Durée garantie', 2, 14, N'mois', 1),

-- Jardin
(N'Jardin', N'Type article', 5, 1, NULL, 0),
(N'Jardin', N'État', 5, 2, NULL, 0),
(N'Jardin', N'Matière', 5, 3, NULL, 0),
(N'Jardin', N'Taille', 2, 4, N'cm', 1),
(N'Jardin', N'Type plante', 5, 5, NULL, 0),
(N'Jardin', N'Exposition soleil', 5, 6, NULL, 0),
(N'Jardin', N'Arrosage', 5, 7, NULL, 0),
(N'Jardin', N'Avec pot', 4, 8, NULL, 0),
(N'Jardin', N'Livraison possible', 4, 9, NULL, 0),

-- Services
(N'Services', N'Type service', 5, 1, NULL, 0),
(N'Services', N'Expérience', 2, 2, N'années', 1),
(N'Services', N'Disponibilité', 5, 3, NULL, 0),
(N'Services', N'Déplacement possible', 4, 4, NULL, 0),
(N'Services', N'Service à distance', 4, 5, NULL, 0),
(N'Services', N'Tarif par', 5, 6, NULL, 0),
(N'Services', N'Durée estimée', 2, 7, N'heures', 1),
(N'Services', N'Certification', 4, 8, NULL, 0),
(N'Services', N'Urgence acceptée', 4, 9, NULL, 0),

-- Emploi
(N'Emploi', N'Type annonce', 5, 1, NULL, 0),
(N'Emploi', N'Domaine', 1, 2, N'Ex: Informatique, Commerce...', 0),
(N'Emploi', N'Type contrat', 5, 3, NULL, 0),
(N'Emploi', N'Niveau étude', 5, 4, NULL, 0),
(N'Emploi', N'Expérience requise', 2, 5, N'années', 1),
(N'Emploi', N'Salaire min', 2, 6, N'TND', 1),
(N'Emploi', N'Salaire max', 2, 7, N'TND', 1),
(N'Emploi', N'Télétravail', 4, 8, NULL, 0),
(N'Emploi', N'Compétences', 1, 9, N'Ex: Angular, SQL, Gestion...', 0),
(N'Emploi', N'Date début', 3, 10, NULL, 0),
(N'Emploi', N'Langues', 1, 11, N'Ex: Français, Anglais', 0);


-- 4. Insert Options for LISTE Attributes
INSERT INTO #OptionSeed (CategoryName, AttributeName, Valeur, OrdreAffichage)
VALUES
-- Véhicules
(N'Véhicules', N'Type véhicule', N'Voiture', 1),
(N'Véhicules', N'Type véhicule', N'Moto', 2),
(N'Véhicules', N'Type véhicule', N'Camion', 3),
(N'Véhicules', N'Type véhicule', N'Bus', 4),
(N'Véhicules', N'Type véhicule', N'Utilitaire', 5),
(N'Véhicules', N'Type véhicule', N'Tracteur', 6),
(N'Véhicules', N'Type véhicule', N'Remorque', 7),
(N'Véhicules', N'Carburant', N'Essence', 1),
(N'Véhicules', N'Carburant', N'Diesel', 2),
(N'Véhicules', N'Carburant', N'Hybride', 3),
(N'Véhicules', N'Carburant', N'Électrique', 4),
(N'Véhicules', N'Carburant', N'GPL', 5),
(N'Véhicules', N'Boîte vitesse', N'Manuelle', 1),
(N'Véhicules', N'Boîte vitesse', N'Automatique', 2),
(N'Vehicules', N'État', N'Neuf', 1),
(N'Véhicules', N'État', N'Véhicule d''exposition', 2),
(N'Véhicules', N'État', N'Sur commande', 3),

-- Immobilier
(N'Immobilier', N'Type bien', N'Appartement', 1),
(N'Immobilier', N'Type bien', N'Maison', 2),
(N'Immobilier', N'Type bien', N'Villa', 3),
(N'Immobilier', N'Type bien', N'Studio', 4),
(N'Immobilier', N'Type bien', N'Terrain', 5),
(N'Immobilier', N'Type bien', N'Bureau', 6),
(N'Immobilier', N'Type bien', N'Local commercial', 7),
(N'Immobilier', N'Type bien', N'Garage', 8),
(N'Immobilier', N'Type bien', N'Immeuble', 9),
(N'Immobilier', N'Type bien', N'Ferme', 10),
(N'Immobilier', N'Transaction', N'Vente', 1),
(N'Immobilier', N'Transaction', N'Location', 2),
(N'Immobilier', N'Chauffage', N'Aucun', 1),
(N'Immobilier', N'Chauffage', N'Gaz', 2),
(N'Immobilier', N'Chauffage', N'Électrique', 3),
(N'Immobilier', N'Chauffage', N'Central', 4),
(N'Immobilier', N'Chauffage', N'Climatisation réversible', 5),
(N'Immobilier', N'Chauffage', N'Solaire', 6),
(N'Immobilier', N'État du bien', N'Neuf', 1),
(N'Immobilier', N'État du bien', N'En construction', 2),
(N'Immobilier', N'État du bien', N'Sur plan', 3),
(N'Immobilier', N'État du bien', N'Rénové à neuf', 4),

-- Téléphones
(N'Téléphones', N'État', N'Neuf scellé', 1),
(N'Téléphones', N'État', N'Neuf ouvert', 2),
(N'Téléphones', N'État', N'Produit d''exposition', 3),

-- Informatique
(N'Informatique', N'Type produit', N'PC portable', 1),
(N'Informatique', N'Type produit', N'PC de bureau', 2),
(N'Informatique', N'Type produit', N'Écran', 3),
(N'Informatique', N'Type produit', N'Clavier', 4),
(N'Informatique', N'Type produit', N'Souris', 5),
(N'Informatique', N'Type produit', N'Imprimante', 6),
(N'Informatique', N'Type produit', N'Scanner', 7),
(N'Informatique', N'Type produit', N'Composant PC', 8),
(N'Informatique', N'Type produit', N'Carte graphique', 9),
(N'Informatique', N'Type produit', N'Disque dur / SSD', 10),
(N'Informatique', N'Type produit', N'Accessoire informatique', 11),
(N'Informatique', N'Type produit', N'Réseau', 12),
(N'Informatique', N'État', N'Neuf scellé', 1),
(N'Informatique', N'État', N'Neuf ouvert', 2),
(N'Informatique', N'État', N'Produit d''exposition', 3),

-- Mode
(N'Mode', N'Type article', N'T-shirt', 1),
(N'Mode', N'Type article', N'Chemise', 2),
(N'Mode', N'Type article', N'Pantalon', 3),
(N'Mode', N'Type article', N'Jean', 4),
(N'Mode', N'Type article', N'Robe', 5),
(N'Mode', N'Type article', N'Jupe', 6),
(N'Mode', N'Type article', N'Veste', 7),
(N'Mode', N'Type article', N'Manteau', 8),
(N'Mode', N'Type article', N'Pull', 9),
(N'Mode', N'Type article', N'Chaussures', 10),
(N'Mode', N'Type article', N'Sac', 11),
(N'Mode', N'Type article', N'Montre', 12),
(N'Mode', N'Type article', N'Bijoux', 13),
(N'Mode', N'Type article', N'Lunettes', 14),
(N'Mode', N'Type article', N'Ceinture', 15),
(N'Mode', N'Type article', N'Casquette', 16),
(N'Mode', N'Type article', N'Hijab / Foulard', 17),
(N'Mode', N'Type article', N'Accessoire', 18),
(N'Mode', N'Genre', N'Homme', 1),
(N'Mode', N'Genre', N'Femme', 2),
(N'Mode', N'Genre', N'Enfant', 3),
(N'Mode', N'Genre', N'Bébé', 4),
(N'Mode', N'Genre', N'Unisexe', 5),
(N'Mode', N'État', N'Neuf', 1),
(N'Mode', N'État', N'Neuf avec étiquette', 2),
(N'Mode', N'État', N'Produit d''exposition', 3),
(N'Mode', N'Authenticité', N'Original', 1),
(N'Mode', N'Authenticité', N'Marque locale', 2),
(N'Mode', N'Authenticité', N'Sans marque', 3),
(N'Mode', N'Saison', N'Printemps', 1),
(N'Mode', N'Saison', N'Été', 2),
(N'Mode', N'Saison', N'Automne', 3),
(N'Mode', N'Saison', N'Hiver', 4),
(N'Mode', N'Saison', N'Toutes saisons', 5),

-- Beauté
(N'Beauté', N'Type produit', N'Parfum', 1),
(N'Beauté', N'Type produit', N'Maquillage', 2),
(N'Beauté', N'Type produit', N'Soin visage', 3),
(N'Beauté', N'Type produit', N'Soin corps', 4),
(N'Beauté', N'Type produit', N'Soin cheveux', 5),
(N'Beauté', N'Type produit', N'Hygiène', 6),
(N'Beauté', N'Type produit', N'Accessoire beauté', 7),
(N'Beauté', N'Type produit', N'Produit naturel', 8),
(N'Beauté', N'Type produit', N'Crème', 9),
(N'Beauté', N'Type produit', N'Shampoing', 10),
(N'Beauté', N'Type produit', N'Gel douche', 11),
(N'Beauté', N'Type produit', N'Déodorant', 12),
(N'Beauté', N'Genre', N'Homme', 1),
(N'Beauté', N'Genre', N'Femme', 2),
(N'Beauté', N'Genre', N'Unisexe', 3),
(N'Beauté', N'État', N'Neuf scellé', 1),
(N'Beauté', N'État', N'Neuf non scellé', 2),
(N'Beauté', N'Type peau', N'Peau normale', 1),
(N'Beauté', N'Type peau', N'Peau sèche', 2),
(N'Beauté', N'Type peau', N'Peau grasse', 3),
(N'Beauté', N'Type peau', N'Peau mixte', 4),
(N'Beauté', N'Type peau', N'Peau sensible', 5),
(N'Beauté', N'Type peau', N'Peau acnéique', 6),
(N'Beauté', N'Type peau', N'Tous types de peau', 7),
(N'Beauté', N'Type cheveux', N'Cheveux normaux', 1),
(N'Beauté', N'Type cheveux', N'Cheveux secs', 2),
(N'Beauté', N'Type cheveux', N'Cheveux gras', 3),
(N'Beauté', N'Type cheveux', N'Cheveux bouclés', 4),
(N'Beauté', N'Type cheveux', N'Cheveux frisés', 5),
(N'Beauté', N'Type cheveux', N'Cheveux colorés', 6),
(N'Beauté', N'Type cheveux', N'Cheveux abîmés', 7),
(N'Beauté', N'Type cheveux', N'Tous types de cheveux', 8),
(N'Beauté', N'Authenticité', N'Original', 1),
(N'Beauté', N'Authenticité', N'Marque locale', 2),
(N'Beauté', N'Authenticité', N'Sans marque', 3),

-- Maison
(N'Maison', N'Type article', N'Meuble', 1),
(N'Maison', N'Type article', N'Canapé', 2),
(N'Maison', N'Type article', N'Lit', 3),
(N'Maison', N'Type article', N'Table', 4),
(N'Maison', N'Type article', N'Chaise', 5),
(N'Maison', N'Type article', N'Armoire', 6),
(N'Maison', N'Type article', N'Décoration', 7),
(N'Maison', N'Type article', N'Tapis', 8),
(N'Maison', N'Type article', N'Rideau', 9),
(N'Maison', N'Type article', N'Cuisine', 10),
(N'Maison', N'Type article', N'Vaisselle', 11),
(N'Maison', N'Type article', N'Électroménager', 12),
(N'Maison', N'Type article', N'Literie', 13),
(N'Maison', N'Type article', N'Rangement', 14),
(N'Maison', N'Type article', N'Luminaire', 15),
(N'Maison', N'Matière', N'Bois', 1),
(N'Maison', N'Matière', N'Métal', 2),
(N'Maison', N'Matière', N'Plastique', 3),
(N'Maison', N'Matière', N'Verre', 4),
(N'Maison', N'Matière', N'Tissu', 5),
(N'Maison', N'Matière', N'Cuir', 6),
(N'Maison', N'Matière', N'Simili cuir', 7),
(N'Maison', N'Matière', N'Céramique', 8),
(N'Maison', N'Matière', N'Marbre', 9),
(N'Maison', N'Matière', N'Rotin', 10),
(N'Maison', N'Matière', N'Bambou', 11),
(N'Maison', N'État', N'Neuf', 1),
(N'Maison', N'État', N'Neuf avec garantie', 2),
(N'Maison', N'État', N'Produit d''exposition', 3),
(N'Maison', N'État', N'Sur commande', 4),

-- Jardin
(N'Jardin', N'Type article', N'Plante', 1),
(N'Jardin', N'Type article', N'Arbre', 2),
(N'Jardin', N'Type article', N'Fleur', 3),
(N'Jardin', N'Type article', N'Pot', 4),
(N'Jardin', N'Type article', N'Terreau', 5),
(N'Jardin', N'Type article', N'Outil jardin', 6),
(N'Jardin', N'Type article', N'Mobilier jardin', 7),
(N'Jardin', N'Type article', N'Décoration extérieure', 8),
(N'Jardin', N'Type article', N'Arrosage', 9),
(N'Jardin', N'Type article', N'Gazon', 10),
(N'Jardin', N'Type article', N'Graines', 11),
(N'Jardin', N'Type article', N'Engrais', 12),
(N'Jardin', N'État', N'Neuf', 1),
(N'Jardin', N'État', N'Produit d''exposition', 2),
(N'Jardin', N'État', N'Jeune plante', 3),
(N'Jardin', N'État', N'Plante adulte', 4),
(N'Jardin', N'Matière', N'Bois', 1),
(N'Jardin', N'Matière', N'Métal', 2),
(N'Jardin', N'Matière', N'Plastique', 3),
(N'Jardin', N'Matière', N'Terre cuite', 4),
(N'Jardin', N'Matière', N'Céramique', 5),
(N'Jardin', N'Matière', N'Rotin', 6),
(N'Jardin', N'Matière', N'Bambou', 7),
(N'Jardin', N'Matière', N'Tissu', 8),
(N'Jardin', N'Matière', N'Pierre', 9),
(N'Jardin', N'Type plante', N'Plante intérieure', 1),
(N'Jardin', N'Type plante', N'Plante extérieure', 2),
(N'Jardin', N'Type plante', N'Fleur', 3),
(N'Jardin', N'Type plante', N'Arbre fruitier', 4),
(N'Jardin', N'Type plante', N'Arbre décoratif', 5),
(N'Jardin', N'Type plante', N'Cactus', 6),
(N'Jardin', N'Type plante', N'Succulente', 7),
(N'Jardin', N'Type plante', N'Plante aromatique', 8),
(N'Jardin', N'Type plante', N'Plante grimpante', 9),
(N'Jardin', N'Exposition soleil', N'Plein soleil', 1),
(N'Jardin', N'Exposition soleil', N'Mi-ombre', 2),
(N'Jardin', N'Exposition soleil', N'Ombre', 3),
(N'Jardin', N'Exposition soleil', N'Intérieur lumineux', 4),
(N'Jardin', N'Exposition soleil', N'Faible lumière', 5),
(N'Jardin', N'Arrosage', N'Faible', 1),
(N'Jardin', N'Arrosage', N'Moyen', 2),
(N'Jardin', N'Arrosage', N'Fréquent', 3),
(N'Jardin', N'Arrosage', N'Quotidien', 4),
(N'Jardin', N'Arrosage', N'Rare', 5),

-- Services
(N'Services', N'Type service', N'Réparation', 1),
(N'Services', N'Type service', N'Transport', 2),
(N'Services', N'Type service', N'Nettoyage', 3),
(N'Services', N'Type service', N'Formation', 4),
(N'Services', N'Type service', N'Design', 5),
(N'Services', N'Type service', N'Développement web', 6),
(N'Services', N'Type service', N'Marketing', 7),
(N'Services', N'Type service', N'Photographie', 8),
(N'Services', N'Type service', N'Vidéo', 9),
(N'Services', N'Type service', N'Plomberie', 10),
(N'Services', N'Type service', N'Électricité', 11),
(N'Services', N'Type service', N'Peinture', 12),
(N'Services', N'Type service', N'Menuiserie', 13),
(N'Services', N'Type service', N'Maçonnerie', 14),
(N'Services', N'Type service', N'Jardinage', 15),
(N'Services', N'Type service', N'Mécanique', 16),
(N'Services', N'Type service', N'Livraison', 17),
(N'Services', N'Type service', N'Déménagement', 18),
(N'Services', N'Type service', N'Assistance informatique', 19),
(N'Services', N'Disponibilité', N'Sur demande', 1),
(N'Services', N'Disponibilité', N'Aujourd’hui', 2),
(N'Services', N'Disponibilité', N'Cette semaine', 3),
(N'Services', N'Disponibilité', N'Week-end', 4),
(N'Services', N'Disponibilité', N'Matin', 5),
(N'Services', N'Disponibilité', N'Après-midi', 6),
(N'Services', N'Disponibilité', N'Soir', 7),
(N'Services', N'Disponibilité', N'Urgence', 8),
(N'Services', N'Tarif par', N'Heure', 1),
(N'Services', N'Tarif par', N'Jour', 2),
(N'Services', N'Tarif par', N'Projet', 3),
(N'Services', N'Tarif par', N'Intervention', 4),
(N'Services', N'Tarif par', N'Mois', 5),

-- Emploi
(N'Emploi', N'Type annonce', N'Offre d’emploi', 1),
(N'Emploi', N'Type annonce', N'Stage', 2),
(N'Emploi', N'Type annonce', N'Mission temporaire', 3),
(N'Emploi', N'Type contrat', N'CDI', 1),
(N'Emploi', N'Type contrat', N'CDD', 2),
(N'Emploi', N'Type contrat', N'Stage', 3),
(N'Emploi', N'Type contrat', N'Freelance', 4),
(N'Emploi', N'Type contrat', N'Temps partiel', 5),
(N'Emploi', N'Type contrat', N'Temps plein', 6),
(N'Emploi', N'Type contrat', N'Saisonnier', 7),
(N'Emploi', N'Type contrat', N'Contrat temporaire', 8),
(N'Emploi', N'Type contrat', N'Alternance', 9),
(N'Emploi', N'Niveau étude', N'Aucun diplôme', 1),
(N'Emploi', N'Niveau étude', N'Primaire', 2),
(N'Emploi', N'Niveau étude', N'Collège', 3),
(N'Emploi', N'Niveau étude', N'Bac', 4),
(N'Emploi', N'Niveau étude', N'Formation professionnelle', 5),
(N'Emploi', N'Niveau étude', N'BTP', 6),
(N'Emploi', N'Niveau étude', N'BTS', 7),
(N'Emploi', N'Niveau étude', N'Bac+2', 8),
(N'Emploi', N'Niveau étude', N'Licence', 9),
(N'Emploi', N'Niveau étude', N'Bac+3', 10),
(N'Emploi', N'Niveau étude', N'Master', 11),
(N'Emploi', N'Niveau étude', N'Bac+5', 12),
(N'Emploi', N'Niveau étude', N'Ingénieur', 13),
(N'Emploi', N'Niveau étude', N'Doctorat', 14);
-- 5. Insert categories into dbo.Categories
INSERT INTO dbo.Categories (Nom, Description, IconKey, OrdreAffichage, SupportePaiement, DateCreation)
SELECT Nom, Description, IconKey, OrdreAffichage, SupportePaiement, SYSUTCDATETIME()
FROM #CategorySeed;

-- 6. Insert attributes into dbo.AttributsCategorie
DECLARE @InsertedAttributes TABLE (
    IdAttributCategorie INT NOT NULL,
    IdCategorie INT NOT NULL,
    Nom NVARCHAR(100) NOT NULL
);

INSERT INTO dbo.AttributsCategorie (IdCategorie, Nom, TypeDonnee, OrdreAffichage, Placeholder, EstPlage)
OUTPUT inserted.IdAttributCategorie, inserted.IdCategorie, inserted.Nom
INTO @InsertedAttributes (IdAttributCategorie, IdCategorie, Nom)
SELECT c.IdCategorie, a.Nom, a.TypeDonnee, a.OrdreAffichage, a.Placeholder, a.EstPlage
FROM #AttributeSeed a
JOIN dbo.Categories c ON c.Nom = a.CategoryName;

-- 7. Insert options into dbo.OptionsAttributCategorie
INSERT INTO dbo.OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage)
SELECT ia.IdAttributCategorie, o.Valeur, o.OrdreAffichage
FROM #OptionSeed o
JOIN dbo.Categories c ON c.Nom = o.CategoryName
JOIN @InsertedAttributes ia ON ia.IdCategorie = c.IdCategorie AND ia.Nom = o.AttributeName;

PRINT 'Categories, attributes, and options successfully seeded.';
GO

-- ============================================================================
-- SETUP COMPLETED SUCCESSFULLY
-- ============================================================================
PRINT 'BigDeals Db setup completed successfully with 0 errors.';
GO
