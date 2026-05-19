USE BigDealsDb;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Signalements]') AND type in (N'U'))
BEGIN
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

    CREATE INDEX [IX_Signalements_Statut_DateCreation] ON [dbo].[Signalements] ([Statut], [DateCreation] DESC);
    CREATE INDEX [IX_Signalements_IdAnnonce] ON [dbo].[Signalements] ([IdAnnonce]);
    CREATE INDEX [IX_Signalements_IdUtilisateur] ON [dbo].[Signalements] ([IdUtilisateur]);
END
GO
