USE BigDealsDb;
GO

--------------------------------------------------------------------------------
-- 09_AddProductCheckoutPayment.sql
-- Goal: Add SupportePaiement to Categories and create tables for Product Checkout
--------------------------------------------------------------------------------

PRINT 'Adding SupportePaiement to Categories...';

IF COL_LENGTH('dbo.Categories', 'SupportePaiement') IS NULL
BEGIN
    ALTER TABLE dbo.Categories
    ADD SupportePaiement BIT NOT NULL
        CONSTRAINT DF_Categories_SupportePaiement DEFAULT 0;
END
GO

PRINT 'Updating SupportePaiement values for Categories...';

UPDATE dbo.Categories
SET SupportePaiement = 0
WHERE Nom IN (
    N'Véhicules',
    N'Immobilier',
    N'Services',
    N'Emploi'
);

UPDATE dbo.Categories
SET SupportePaiement = 1
WHERE Nom IN (
    N'Téléphones',
    N'Informatique',
    N'Mode',
    N'Beauté',
    N'Maison',
    N'Jardin'
);
GO

--------------------------------------------------------------------------------
-- Commandes Table
--------------------------------------------------------------------------------
PRINT 'Creating Commandes table...';

IF OBJECT_ID('dbo.Commandes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Commandes (
        IdCommande BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdAnnonce BIGINT NOT NULL,
        IdAcheteur BIGINT NOT NULL,
        IdAnnonceur BIGINT NOT NULL,
        Montant DECIMAL(18,3) NOT NULL,
        StatutCommande INT NOT NULL, -- 1=EN_ATTENTE_PAIEMENT, 2=PAYEE, 3=ANNULEE
        StatutLivraison INT NOT NULL DEFAULT 1, -- 1-7 delivery lifecycle
        AdresseLivraison NVARCHAR(300) NULL,
        VilleLivraison NVARCHAR(100) NULL,
        TelephoneLivraison NVARCHAR(30) NULL,
        DateExpedition DATETIME2 NULL,
        DateLivraison DATETIME2 NULL,
        DateDerniereMiseAJourLivraison DATETIME2 NULL,
        DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

--------------------------------------------------------------------------------
-- PaiementsCommandes Table
--------------------------------------------------------------------------------
PRINT 'Creating PaiementsCommandes table...';

IF OBJECT_ID('dbo.PaiementsCommandes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PaiementsCommandes (
        IdPaiementCommande BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdCommande BIGINT NOT NULL,
        Montant DECIMAL(18,3) NOT NULL,
        MethodePaiement NVARCHAR(50) NOT NULL,
        StatutPaiement INT NOT NULL, -- 1=EN_ATTENTE, 2=ACCEPTE, 3=REFUSE
        NumeroCarteMasque NVARCHAR(30) NULL,
        DatePaiement DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

--------------------------------------------------------------------------------
-- Constraints & Foreign Keys
--------------------------------------------------------------------------------
PRINT 'Adding constraints and foreign keys...';

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Commandes_Annonces')
    ALTER TABLE dbo.Commandes ADD CONSTRAINT FK_Commandes_Annonces FOREIGN KEY (IdAnnonce) REFERENCES dbo.Annonces(IdAnnonce);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Commandes_Acheteur')
    ALTER TABLE dbo.Commandes ADD CONSTRAINT FK_Commandes_Acheteur FOREIGN KEY (IdAcheteur) REFERENCES dbo.Utilisateurs(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Commandes_Annonceur')
    ALTER TABLE dbo.Commandes ADD CONSTRAINT FK_Commandes_Annonceur FOREIGN KEY (IdAnnonceur) REFERENCES dbo.Utilisateurs(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_Commandes_Montant' AND type = 'C')
    ALTER TABLE dbo.Commandes ADD CONSTRAINT CHK_Commandes_Montant CHECK (Montant > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_Commandes_Statut' AND type = 'C')
    ALTER TABLE dbo.Commandes ADD CONSTRAINT CHK_Commandes_Statut CHECK (StatutCommande IN (1, 2, 3));
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PaiementsCommandes_Commandes')
    ALTER TABLE dbo.PaiementsCommandes ADD CONSTRAINT FK_PaiementsCommandes_Commandes FOREIGN KEY (IdCommande) REFERENCES dbo.Commandes(IdCommande);
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_PaiementsCommandes_Montant' AND type = 'C')
    ALTER TABLE dbo.PaiementsCommandes ADD CONSTRAINT CHK_PaiementsCommandes_Montant CHECK (Montant > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_PaiementsCommandes_Statut' AND type = 'C')
    ALTER TABLE dbo.PaiementsCommandes ADD CONSTRAINT CHK_PaiementsCommandes_Statut CHECK (StatutPaiement IN (1, 2, 3));
GO

--------------------------------------------------------------------------------
-- Indexes
--------------------------------------------------------------------------------
PRINT 'Adding indexes...';

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Commandes_IdAcheteur')
    CREATE NONCLUSTERED INDEX IX_Commandes_IdAcheteur ON dbo.Commandes(IdAcheteur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Commandes_IdAnnonce')
    CREATE NONCLUSTERED INDEX IX_Commandes_IdAnnonce ON dbo.Commandes(IdAnnonce);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Commandes_StatutCommande')
    CREATE NONCLUSTERED INDEX IX_Commandes_StatutCommande ON dbo.Commandes(StatutCommande);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PaiementsCommandes_IdCommande')
    CREATE NONCLUSTERED INDEX IX_PaiementsCommandes_IdCommande ON dbo.PaiementsCommandes(IdCommande);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PaiementsCommandes_StatutPaiement')
    CREATE NONCLUSTERED INDEX IX_PaiementsCommandes_StatutPaiement ON dbo.PaiementsCommandes(StatutPaiement);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Commandes_Pending_Acheteur_Annonce')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UQ_Commandes_Pending_Acheteur_Annonce
    ON dbo.Commandes(IdAcheteur, IdAnnonce)
    WHERE StatutCommande = 1;
END
GO

PRINT 'Product Checkout tables and updates applied successfully.';
GO
