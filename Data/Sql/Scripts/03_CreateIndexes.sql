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

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Categories_OrdreAffichage')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Categories_OrdreAffichage ON Categories(OrdreAffichage);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AttributsCategorie_OrdreAffichage')
BEGIN
    CREATE NONCLUSTERED INDEX IX_AttributsCategorie_OrdreAffichage ON AttributsCategorie(OrdreAffichage);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OptionsAttributCategorie_OrdreAffichage')
BEGIN
    CREATE NONCLUSTERED INDEX IX_OptionsAttributCategorie_OrdreAffichage ON OptionsAttributCategorie(OrdreAffichage);
END
GO

