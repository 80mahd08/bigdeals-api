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

-- Annonces Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Annonces_Statut_EstActive')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Annonces_Statut_EstActive ON Annonces(Statut, EstActive);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Annonces_IdCategorie')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Annonces_IdCategorie ON Annonces(IdCategorie);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Annonces_IdUtilisateur')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Annonces_IdUtilisateur ON Annonces(IdUtilisateur);
END
GO

-- --- B9 INDEXES ---

-- Favoris
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Favoris_IdUtilisateur')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Favoris_IdUtilisateur ON Favoris(IdUtilisateur);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Favoris_IdAnnonce')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Favoris_IdAnnonce ON Favoris(IdAnnonce);
END

-- AbonnementsAnnonceur
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AbonnementsAnnonceur_IdUtilisateur')
BEGIN
    CREATE NONCLUSTERED INDEX IX_AbonnementsAnnonceur_IdUtilisateur ON AbonnementsAnnonceur(IdUtilisateur);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AbonnementsAnnonceur_IdAnnonceur')
BEGIN
    CREATE NONCLUSTERED INDEX IX_AbonnementsAnnonceur_IdAnnonceur ON AbonnementsAnnonceur(IdAnnonceur);
END

-- ContactsAnnonceur
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ContactsAnnonceur_IdUtilisateur')
BEGIN
    CREATE NONCLUSTERED INDEX IX_ContactsAnnonceur_IdUtilisateur ON ContactsAnnonceur(IdUtilisateur);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ContactsAnnonceur_IdAnnonceur')
BEGIN
    CREATE NONCLUSTERED INDEX IX_ContactsAnnonceur_IdAnnonceur ON ContactsAnnonceur(IdAnnonceur);
END
GO

-- 12. PasswordResetTokens
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PasswordResetTokens_TokenHash')
BEGIN
    CREATE NONCLUSTERED INDEX IX_PasswordResetTokens_TokenHash ON PasswordResetTokens(TokenHash);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PasswordResetTokens_IdUtilisateur')
BEGIN
    CREATE NONCLUSTERED INDEX IX_PasswordResetTokens_IdUtilisateur ON PasswordResetTokens(IdUtilisateur);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PasswordResetTokens_DateExpiration')
BEGIN
    CREATE NONCLUSTERED INDEX IX_PasswordResetTokens_DateExpiration ON PasswordResetTokens(DateExpiration);
END
GO
