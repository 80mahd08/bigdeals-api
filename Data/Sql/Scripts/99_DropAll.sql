USE BigDealsDb;
GO

IF OBJECT_ID('DemandesAnnonceur', 'U') IS NOT NULL
BEGIN
    DROP TABLE DemandesAnnonceur;
END
GO

IF OBJECT_ID('Utilisateurs', 'U') IS NOT NULL
BEGIN
    DROP TABLE Utilisateurs;
END
GO
