USE BigDealsDb;
GO

-- Create PaiementsAnnonceur table if it doesn't exist
IF OBJECT_ID('dbo.PaiementsAnnonceur', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PaiementsAnnonceur (
        IdPaiementAnnonceur BIGINT IDENTITY(1,1) NOT NULL,
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

        -- Primary Key
        CONSTRAINT PK_PaiementsAnnonceur PRIMARY KEY CLUSTERED (IdPaiementAnnonceur),

        -- Foreign Keys
        CONSTRAINT FK_PaiementsAnnonceur_Utilisateurs FOREIGN KEY (IdUtilisateur) 
            REFERENCES dbo.Utilisateurs(IdUtilisateur),
        
        CONSTRAINT FK_PaiementsAnnonceur_DemandesAnnonceur FOREIGN KEY (IdDemandeAnnonceur) 
            REFERENCES dbo.DemandesAnnonceur(IdDemandeAnnonceur),

        -- Unique Constraints
        CONSTRAINT UQ_PaiementsAnnonceur_DeveloperTrackingId UNIQUE (DeveloperTrackingId),
        CONSTRAINT UQ_PaiementsAnnonceur_IdDemandeAnnonceur UNIQUE (IdDemandeAnnonceur),

        -- Check Constraints
        CONSTRAINT CK_PaiementsAnnonceur_Montant_Positive CHECK (Montant > 0),
        CONSTRAINT CK_PaiementsAnnonceur_StatutPaiement CHECK (StatutPaiement IN (1, 2, 3, 4)),
        CONSTRAINT CK_PaiementsAnnonceur_Montant_AnnonceurFee CHECK (Montant = CAST(200.000 AS DECIMAL(18,3)))
    );
END
GO

-- Create Indexes
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_PaiementsAnnonceur_IdUtilisateur'
      AND object_id = OBJECT_ID('dbo.PaiementsAnnonceur')
)
BEGIN
    CREATE INDEX IX_PaiementsAnnonceur_IdUtilisateur
    ON dbo.PaiementsAnnonceur(IdUtilisateur);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_PaiementsAnnonceur_IdDemandeAnnonceur'
      AND object_id = OBJECT_ID('dbo.PaiementsAnnonceur')
)
BEGIN
    CREATE INDEX IX_PaiementsAnnonceur_IdDemandeAnnonceur
    ON dbo.PaiementsAnnonceur(IdDemandeAnnonceur);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_PaiementsAnnonceur_ProviderPaymentId'
      AND object_id = OBJECT_ID('dbo.PaiementsAnnonceur')
)
BEGIN
    CREATE INDEX IX_PaiementsAnnonceur_ProviderPaymentId
    ON dbo.PaiementsAnnonceur(ProviderPaymentId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_PaiementsAnnonceur_StatutPaiement'
      AND object_id = OBJECT_ID('dbo.PaiementsAnnonceur')
)
BEGIN
    CREATE INDEX IX_PaiementsAnnonceur_StatutPaiement
    ON dbo.PaiementsAnnonceur(StatutPaiement);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_PaiementsAnnonceur_DateCreation'
      AND object_id = OBJECT_ID('dbo.PaiementsAnnonceur')
)
BEGIN
    CREATE INDEX IX_PaiementsAnnonceur_DateCreation
    ON dbo.PaiementsAnnonceur(DateCreation);
END
GO
