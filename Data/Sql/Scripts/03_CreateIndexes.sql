USE BigDealsDb;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Utilisateurs_Email')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Utilisateurs_Email ON Utilisateurs(Email);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DemandesAnnonceur_IdUtilisateur')
BEGIN
    CREATE NONCLUSTERED INDEX IX_DemandesAnnonceur_IdUtilisateur ON DemandesAnnonceur(IdUtilisateur);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DemandesAnnonceur_Statut')
BEGIN
    CREATE NONCLUSTERED INDEX IX_DemandesAnnonceur_Statut ON DemandesAnnonceur(Statut);
END
GO
