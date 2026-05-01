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
