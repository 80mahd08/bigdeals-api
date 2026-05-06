USE BigDealsDb;
GO

-- 13. Avis
IF OBJECT_ID('Avis', 'U') IS NOT NULL
BEGIN
    DROP TABLE Avis;
END

CREATE TABLE Avis (
    IdAvis BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdAnnonce BIGINT NOT NULL,
    IdUtilisateur BIGINT NOT NULL,
    Note INT NOT NULL, -- Rating out of 5
    Commentaire NVARCHAR(2000) NOT NULL,
    DateCreation DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DateModification DATETIME2 NULL,
    EstActif BIT NOT NULL DEFAULT 1,
    
    -- Constraints
    CONSTRAINT FK_Avis_Annonces FOREIGN KEY (IdAnnonce) REFERENCES Annonces(IdAnnonce),
    CONSTRAINT FK_Avis_Utilisateurs FOREIGN KEY (IdUtilisateur) REFERENCES Utilisateurs(IdUtilisateur),
    CONSTRAINT UQ_Avis_Annonce_Utilisateur UNIQUE (IdAnnonce, IdUtilisateur),
    CONSTRAINT CHK_Avis_Note CHECK (Note >= 1 AND Note <= 5)
);
GO
