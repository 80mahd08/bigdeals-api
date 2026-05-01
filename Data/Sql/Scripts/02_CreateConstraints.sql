USE BigDealsDb;
GO

-- --- FOREIGN KEYS ---

-- DemandesAnnonceur -> Utilisateurs (Requester)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DemandesAnnonceur_Utilisateurs')
    ALTER TABLE DemandesAnnonceur ADD CONSTRAINT FK_DemandesAnnonceur_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES Utilisateurs(IdUtilisateur);

-- DemandesAnnonceur -> Utilisateurs (Admin)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DemandesAnnonceur_Admins')
    ALTER TABLE DemandesAnnonceur ADD CONSTRAINT FK_DemandesAnnonceur_Admins FOREIGN KEY (IdAdminTraitant) REFERENCES Utilisateurs(IdUtilisateur);

-- --- UNIQUE CONSTRAINTS ---

IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'UQ_Utilisateurs_Email' AND type = 'UQ')
    ALTER TABLE Utilisateurs ADD CONSTRAINT UQ_Utilisateurs_Email UNIQUE (Email);

-- Filtered unique index: A user cannot have more than one active EN_ATTENTE demande annonceur (Statut = 1)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_DemandesAnnonceur_EnAttente_PerUser')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UQ_DemandesAnnonceur_EnAttente_PerUser 
    ON DemandesAnnonceur(IdUtilisateur)
    WHERE Statut = 1;
END

-- --- CHECK CONSTRAINTS ---

-- Enum RoleUtilisateur: 1=CLIENT, 2=ANNONCEUR, 3=ADMIN
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'CHK_Utilisateurs_Role' AND type = 'C')
    ALTER TABLE Utilisateurs ADD CONSTRAINT CHK_Utilisateurs_Role CHECK (Role IN (1, 2, 3));

-- Enum StatutCompte: 1=ACTIF, 2=INACTIF, 3=EN_ATTENTE, 4=BLOQUE
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'CHK_Utilisateurs_StatutCompte' AND type = 'C')
    ALTER TABLE Utilisateurs ADD CONSTRAINT CHK_Utilisateurs_StatutCompte CHECK (StatutCompte IN (1, 2, 3, 4));

-- Enum StatutDemandeAnnonceur: 1=EN_ATTENTE, 2=APPROUVEE, 3=REJETEE
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'CHK_DemandesAnnonceur_Statut' AND type = 'C')
    ALTER TABLE DemandesAnnonceur ADD CONSTRAINT CHK_DemandesAnnonceur_Statut CHECK (Statut IN (1, 2, 3));
GO

-- --- CATEGORY SCHEMA CONSTRAINTS ---

-- Categories -> Nom Unique
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'UQ_Categories_Nom' AND type = 'UQ')
    ALTER TABLE Categories ADD CONSTRAINT UQ_Categories_Nom UNIQUE (Nom);

-- AttributsCategorie -> IdCategorie
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AttributsCategorie_Categories')
    ALTER TABLE AttributsCategorie ADD CONSTRAINT FK_AttributsCategorie_Categories FOREIGN KEY (IdCategorie) REFERENCES Categories(IdCategorie);

-- AttributsCategorie -> (IdCategorie, Nom) Unique
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'UQ_AttributsCategorie_Categorie_Nom' AND type = 'UQ')
    ALTER TABLE AttributsCategorie ADD CONSTRAINT UQ_AttributsCategorie_Categorie_Nom UNIQUE (IdCategorie, Nom);

-- OptionsAttributCategorie -> IdAttributCategorie
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OptionsAttributCategorie_AttributsCategorie')
    ALTER TABLE OptionsAttributCategorie ADD CONSTRAINT FK_OptionsAttributCategorie_AttributsCategorie FOREIGN KEY (IdAttributCategorie) REFERENCES AttributsCategorie(IdAttributCategorie);

-- OptionsAttributCategorie -> (IdAttributCategorie, Valeur) Unique
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'UQ_OptionsAttributCategorie_Attribut_Valeur' AND type = 'UQ')
    ALTER TABLE OptionsAttributCategorie ADD CONSTRAINT UQ_OptionsAttributCategorie_Attribut_Valeur UNIQUE (IdAttributCategorie, Valeur);

-- Enum TypeDonneeAttribut: 1=TEXTE, 2=NOMBRE, 3=DATE, 4=BOOLEAN, 5=LISTE
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'CHK_AttributsCategorie_TypeDonnee' AND type = 'C')
    ALTER TABLE AttributsCategorie ADD CONSTRAINT CHK_AttributsCategorie_TypeDonnee CHECK (TypeDonnee IN (1, 2, 3, 4, 5));
GO

-- --- ANNONCE CONSTRAINTS ---

-- Annonces -> Utilisateurs
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Annonces_Utilisateurs')
    ALTER TABLE Annonces ADD CONSTRAINT FK_Annonces_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES Utilisateurs(IdUtilisateur);

-- Annonces -> Categories
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Annonces_Categories')
    ALTER TABLE Annonces ADD CONSTRAINT FK_Annonces_Categories FOREIGN KEY (IdCategorie) REFERENCES Categories(IdCategorie);

-- ValeursAttributAnnonce -> Annonces
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ValeursAttributAnnonce_Annonces')
    ALTER TABLE ValeursAttributAnnonce ADD CONSTRAINT FK_ValeursAttributAnnonce_Annonces FOREIGN KEY (IdAnnonce) REFERENCES Annonces(IdAnnonce);

-- ValeursAttributAnnonce -> AttributsCategorie
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ValeursAttributAnnonce_AttributsCategorie')
    ALTER TABLE ValeursAttributAnnonce ADD CONSTRAINT FK_ValeursAttributAnnonce_AttributsCategorie FOREIGN KEY (IdAttributCategorie) REFERENCES AttributsCategorie(IdAttributCategorie);

-- ValeursAttributAnnonce -> OptionsAttributCategorie
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ValeursAttributAnnonce_OptionsAttributCategorie')
    ALTER TABLE ValeursAttributAnnonce ADD CONSTRAINT FK_ValeursAttributAnnonce_OptionsAttributCategorie FOREIGN KEY (IdOptionAttributCategorie) REFERENCES OptionsAttributCategorie(IdOptionAttributCategorie);

-- ImagesAnnonce -> Annonces
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ImagesAnnonce_Annonces')
    ALTER TABLE ImagesAnnonce ADD CONSTRAINT FK_ImagesAnnonce_Annonces FOREIGN KEY (IdAnnonce) REFERENCES Annonces(IdAnnonce);

-- CHECKs
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'CHK_Annonces_Prix' AND type = 'C')
    ALTER TABLE Annonces ADD CONSTRAINT CHK_Annonces_Prix CHECK (Prix >= 0);

IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'CHK_Annonces_Statut' AND type = 'C')
    ALTER TABLE Annonces ADD CONSTRAINT CHK_Annonces_Statut CHECK (Statut IN (1, 2, 3, 4, 5));

-- UNIQUE Dynamic Values
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'UQ_ValeursAttributAnnonce_Annonce_Attribut' AND type = 'UQ')
    ALTER TABLE ValeursAttributAnnonce ADD CONSTRAINT UQ_ValeursAttributAnnonce_Annonce_Attribut UNIQUE (IdAnnonce, IdAttributCategorie);
GO
