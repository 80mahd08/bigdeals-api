USE BigDealsDb;
GO

------------------------------------------------------------
-- 1. Utilisateurs / DemandesAnnonceur constraints
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'UQ_Utilisateurs_Email' AND type = 'UQ')
    ALTER TABLE dbo.Utilisateurs ADD CONSTRAINT UQ_Utilisateurs_Email UNIQUE (Email);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_DemandesAnnonceur_Utilisateurs')
    ALTER TABLE dbo.DemandesAnnonceur ADD CONSTRAINT FK_DemandesAnnonceur_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_DemandesAnnonceur_Admins')
    ALTER TABLE dbo.DemandesAnnonceur ADD CONSTRAINT FK_DemandesAnnonceur_Admins FOREIGN KEY (IdAdminTraitant) REFERENCES dbo.Utilisateurs(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_DemandesAnnonceur_EnAttente_PerUser')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UQ_DemandesAnnonceur_EnAttente_PerUser
    ON dbo.DemandesAnnonceur(IdUtilisateur)
    WHERE Statut = 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_Utilisateurs_Role' AND type = 'C')
    ALTER TABLE dbo.Utilisateurs ADD CONSTRAINT CHK_Utilisateurs_Role CHECK (Role IN (1, 2, 3));
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_Utilisateurs_StatutCompte' AND type = 'C')
    ALTER TABLE dbo.Utilisateurs ADD CONSTRAINT CHK_Utilisateurs_StatutCompte CHECK (StatutCompte IN (1, 2, 3, 4));
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_DemandesAnnonceur_Statut' AND type = 'C')
    ALTER TABLE dbo.DemandesAnnonceur ADD CONSTRAINT CHK_DemandesAnnonceur_Statut CHECK (Statut IN (1, 2, 3));
GO

------------------------------------------------------------
-- 2. Category schema constraints - FINAL CLEAN VERSION
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'UQ_Categories_Nom' AND type = 'UQ')
    ALTER TABLE dbo.Categories ADD CONSTRAINT UQ_Categories_Nom UNIQUE (Nom);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AttributsCategorie_Categories')
    ALTER TABLE dbo.AttributsCategorie ADD CONSTRAINT FK_AttributsCategorie_Categories FOREIGN KEY (IdCategorie) REFERENCES dbo.Categories(IdCategorie);
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'UQ_AttributsCategorie_Categorie_Nom' AND type = 'UQ')
    ALTER TABLE dbo.AttributsCategorie ADD CONSTRAINT UQ_AttributsCategorie_Categorie_Nom UNIQUE (IdCategorie, Nom);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_OptionsAttributCategorie_AttributsCategorie')
    ALTER TABLE dbo.OptionsAttributCategorie ADD CONSTRAINT FK_OptionsAttributCategorie_AttributsCategorie FOREIGN KEY (IdAttributCategorie) REFERENCES dbo.AttributsCategorie(IdAttributCategorie);
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'UQ_OptionsAttributCategorie_Attribut_Valeur' AND type = 'UQ')
    ALTER TABLE dbo.OptionsAttributCategorie ADD CONSTRAINT UQ_OptionsAttributCategorie_Attribut_Valeur UNIQUE (IdAttributCategorie, Valeur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_AttributsCategorie_TypeDonnee' AND type = 'C')
    ALTER TABLE dbo.AttributsCategorie ADD CONSTRAINT CHK_AttributsCategorie_TypeDonnee CHECK (TypeDonnee IN (1, 2, 3, 4, 5));
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_AttributsCategorie_OrdreAffichage' AND type = 'C')
    ALTER TABLE dbo.AttributsCategorie ADD CONSTRAINT CHK_AttributsCategorie_OrdreAffichage CHECK (OrdreAffichage > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_OptionsAttributCategorie_OrdreAffichage' AND type = 'C')
    ALTER TABLE dbo.OptionsAttributCategorie ADD CONSTRAINT CHK_OptionsAttributCategorie_OrdreAffichage CHECK (OrdreAffichage > 0);
GO

------------------------------------------------------------
-- 3. Annonce constraints
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Annonces_Utilisateurs')
    ALTER TABLE dbo.Annonces ADD CONSTRAINT FK_Annonces_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Annonces_Categories')
    ALTER TABLE dbo.Annonces ADD CONSTRAINT FK_Annonces_Categories FOREIGN KEY (IdCategorie) REFERENCES dbo.Categories(IdCategorie);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ValeursAttributAnnonce_Annonces')
    ALTER TABLE dbo.ValeursAttributAnnonce ADD CONSTRAINT FK_ValeursAttributAnnonce_Annonces FOREIGN KEY (IdAnnonce) REFERENCES dbo.Annonces(IdAnnonce);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ValeursAttributAnnonce_AttributsCategorie')
    ALTER TABLE dbo.ValeursAttributAnnonce ADD CONSTRAINT FK_ValeursAttributAnnonce_AttributsCategorie FOREIGN KEY (IdAttributCategorie) REFERENCES dbo.AttributsCategorie(IdAttributCategorie);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ValeursAttributAnnonce_OptionsAttributCategorie')
    ALTER TABLE dbo.ValeursAttributAnnonce ADD CONSTRAINT FK_ValeursAttributAnnonce_OptionsAttributCategorie FOREIGN KEY (IdOptionAttributCategorie) REFERENCES dbo.OptionsAttributCategorie(IdOptionAttributCategorie);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ImagesAnnonce_Annonces')
    ALTER TABLE dbo.ImagesAnnonce ADD CONSTRAINT FK_ImagesAnnonce_Annonces FOREIGN KEY (IdAnnonce) REFERENCES dbo.Annonces(IdAnnonce);
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_Annonces_Prix' AND type = 'C')
    ALTER TABLE dbo.Annonces ADD CONSTRAINT CHK_Annonces_Prix CHECK (Prix >= 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_Annonces_Statut' AND type = 'C')
    ALTER TABLE dbo.Annonces ADD CONSTRAINT CHK_Annonces_Statut CHECK (Statut IN (1, 2, 3, 4, 5));
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'UQ_ValeursAttributAnnonce_Annonce_Attribut' AND type = 'UQ')
    ALTER TABLE dbo.ValeursAttributAnnonce ADD CONSTRAINT UQ_ValeursAttributAnnonce_Annonce_Attribut UNIQUE (IdAnnonce, IdAttributCategorie);
GO

------------------------------------------------------------
-- 4. Interaction constraints
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Favoris_Utilisateurs')
    ALTER TABLE dbo.Favoris ADD CONSTRAINT FK_Favoris_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Favoris_Annonces')
    ALTER TABLE dbo.Favoris ADD CONSTRAINT FK_Favoris_Annonces FOREIGN KEY (IdAnnonce) REFERENCES dbo.Annonces(IdAnnonce);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AbonnementsAnnonceur_Utilisateurs')
    ALTER TABLE dbo.AbonnementsAnnonceur ADD CONSTRAINT FK_AbonnementsAnnonceur_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AbonnementsAnnonceur_Annonceurs')
    ALTER TABLE dbo.AbonnementsAnnonceur ADD CONSTRAINT FK_AbonnementsAnnonceur_Annonceurs FOREIGN KEY (IdAnnonceur) REFERENCES dbo.Utilisateurs(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ContactsAnnonceur_Utilisateurs')
    ALTER TABLE dbo.ContactsAnnonceur ADD CONSTRAINT FK_ContactsAnnonceur_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ContactsAnnonceur_Annonces')
    ALTER TABLE dbo.ContactsAnnonceur ADD CONSTRAINT FK_ContactsAnnonceur_Annonces FOREIGN KEY (IdAnnonce) REFERENCES dbo.Annonces(IdAnnonce);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ContactsAnnonceur_Annonceurs')
    ALTER TABLE dbo.ContactsAnnonceur ADD CONSTRAINT FK_ContactsAnnonceur_Annonceurs FOREIGN KEY (IdAnnonceur) REFERENCES dbo.Utilisateurs(IdUtilisateur);
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_ContactsAnnonceur_TypeContact' AND type = 'C')
    ALTER TABLE dbo.ContactsAnnonceur ADD CONSTRAINT CHK_ContactsAnnonceur_TypeContact CHECK (TypeContact IN (1, 2));
GO

------------------------------------------------------------
-- 5. Password reset constraints
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PasswordResetTokens_Utilisateurs')
    ALTER TABLE dbo.PasswordResetTokens ADD CONSTRAINT FK_PasswordResetTokens_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES dbo.Utilisateurs(IdUtilisateur);
GO
