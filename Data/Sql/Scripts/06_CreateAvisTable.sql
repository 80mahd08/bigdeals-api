USE BigDealsDb;
GO

------------------------------------------------------------
-- 13. Avis
------------------------------------------------------------
IF OBJECT_ID('dbo.Avis', 'U') IS NULL
BEGIN
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
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Avis_IdAnnonce')
    CREATE NONCLUSTERED INDEX IX_Avis_IdAnnonce ON dbo.Avis(IdAnnonce);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Avis_IdUtilisateur')
    CREATE NONCLUSTERED INDEX IX_Avis_IdUtilisateur ON dbo.Avis(IdUtilisateur);
GO
