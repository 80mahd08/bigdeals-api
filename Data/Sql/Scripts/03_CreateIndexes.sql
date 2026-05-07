USE BigDealsDb;
GO

------------------------------------------------------------
-- Users / auth indexes
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Utilisateurs_Email')
    CREATE NONCLUSTERED INDEX IX_Utilisateurs_Email ON dbo.Utilisateurs(Email);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DemandesAnnonceur_IdUtilisateur')
    CREATE NONCLUSTERED INDEX IX_DemandesAnnonceur_IdUtilisateur ON dbo.DemandesAnnonceur(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DemandesAnnonceur_Statut')
    CREATE NONCLUSTERED INDEX IX_DemandesAnnonceur_Statut ON dbo.DemandesAnnonceur(Statut);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PasswordResetTokens_IdUtilisateur')
    CREATE NONCLUSTERED INDEX IX_PasswordResetTokens_IdUtilisateur ON dbo.PasswordResetTokens(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PasswordResetTokens_TokenHash')
    CREATE NONCLUSTERED INDEX IX_PasswordResetTokens_TokenHash ON dbo.PasswordResetTokens(TokenHash);
GO

------------------------------------------------------------
-- Category system indexes - FINAL CLEAN VERSION
-- No EstActive, Obligatoire, or Filtrable indexes.
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Categories_OrdreAffichage')
    CREATE NONCLUSTERED INDEX IX_Categories_OrdreAffichage ON dbo.Categories(OrdreAffichage, Nom);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AttributsCategorie_IdCategorie_OrdreAffichage')
    CREATE NONCLUSTERED INDEX IX_AttributsCategorie_IdCategorie_OrdreAffichage ON dbo.AttributsCategorie(IdCategorie, OrdreAffichage, Nom);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AttributsCategorie_TypeDonnee')
    CREATE NONCLUSTERED INDEX IX_AttributsCategorie_TypeDonnee ON dbo.AttributsCategorie(TypeDonnee);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OptionsAttributCategorie_IdAttribut_OrdreAffichage')
    CREATE NONCLUSTERED INDEX IX_OptionsAttributCategorie_IdAttribut_OrdreAffichage ON dbo.OptionsAttributCategorie(IdAttributCategorie, OrdreAffichage, Valeur);
GO

------------------------------------------------------------
-- Annonces indexes
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Annonces_IdUtilisateur')
    CREATE NONCLUSTERED INDEX IX_Annonces_IdUtilisateur ON dbo.Annonces(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Annonces_IdCategorie')
    CREATE NONCLUSTERED INDEX IX_Annonces_IdCategorie ON dbo.Annonces(IdCategorie);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Annonces_Statut_EstActive')
    CREATE NONCLUSTERED INDEX IX_Annonces_Statut_EstActive ON dbo.Annonces(Statut, EstActive);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Annonces_DateCreation')
    CREATE NONCLUSTERED INDEX IX_Annonces_DateCreation ON dbo.Annonces(DateCreation DESC);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Annonces_PublicSearch')
    CREATE NONCLUSTERED INDEX IX_Annonces_PublicSearch ON dbo.Annonces(IdCategorie, Statut, EstActive, DateCreation DESC)
    INCLUDE (Titre, Prix, Localisation);
GO

------------------------------------------------------------
-- Dynamic attribute value indexes
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ValeursAttributAnnonce_IdAnnonce')
    CREATE NONCLUSTERED INDEX IX_ValeursAttributAnnonce_IdAnnonce ON dbo.ValeursAttributAnnonce(IdAnnonce);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ValeursAttributAnnonce_Attribut_Option')
    CREATE NONCLUSTERED INDEX IX_ValeursAttributAnnonce_Attribut_Option ON dbo.ValeursAttributAnnonce(IdAttributCategorie, IdOptionAttributCategorie);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ValeursAttributAnnonce_Attribut_Nombre')
    CREATE NONCLUSTERED INDEX IX_ValeursAttributAnnonce_Attribut_Nombre ON dbo.ValeursAttributAnnonce(IdAttributCategorie, ValeurNombre);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ValeursAttributAnnonce_Attribut_Date')
    CREATE NONCLUSTERED INDEX IX_ValeursAttributAnnonce_Attribut_Date ON dbo.ValeursAttributAnnonce(IdAttributCategorie, ValeurDate);
GO

------------------------------------------------------------
-- Images / interactions indexes
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ImagesAnnonce_IdAnnonce')
    CREATE NONCLUSTERED INDEX IX_ImagesAnnonce_IdAnnonce ON dbo.ImagesAnnonce(IdAnnonce, OrdreAffichage);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Favoris_IdUtilisateur')
    CREATE NONCLUSTERED INDEX IX_Favoris_IdUtilisateur ON dbo.Favoris(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Favoris_IdAnnonce')
    CREATE NONCLUSTERED INDEX IX_Favoris_IdAnnonce ON dbo.Favoris(IdAnnonce);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AbonnementsAnnonceur_IdUtilisateur')
    CREATE NONCLUSTERED INDEX IX_AbonnementsAnnonceur_IdUtilisateur ON dbo.AbonnementsAnnonceur(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AbonnementsAnnonceur_IdAnnonceur')
    CREATE NONCLUSTERED INDEX IX_AbonnementsAnnonceur_IdAnnonceur ON dbo.AbonnementsAnnonceur(IdAnnonceur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ContactsAnnonceur_IdAnnonceur')
    CREATE NONCLUSTERED INDEX IX_ContactsAnnonceur_IdAnnonceur ON dbo.ContactsAnnonceur(IdAnnonceur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ContactsAnnonceur_IdUtilisateur')
    CREATE NONCLUSTERED INDEX IX_ContactsAnnonceur_IdUtilisateur ON dbo.ContactsAnnonceur(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ContactsAnnonceur_IdAnnonce')
    CREATE NONCLUSTERED INDEX IX_ContactsAnnonceur_IdAnnonce ON dbo.ContactsAnnonceur(IdAnnonce);
GO
