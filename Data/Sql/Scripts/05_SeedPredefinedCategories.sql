USE BigDealsDb;
GO

-- ==========================================
-- COMPREHENSIVE CLEANUP SECTION
-- ==========================================

-- Identify corrupted characters (Encoding mismatch results)
DECLARE @CorruptedChar NVARCHAR(1) = NCHAR(1571); -- 'أ' (Arabic Alef with Hamza)
DECLARE @QuoteCorrupted NVARCHAR(3) = NCHAR(226) + NCHAR(8364) + NCHAR(8482); -- 'â€™' (Quote corruption)

-- 1. Remove references in ValeursAttributAnnonce
DELETE v FROM ValeursAttributAnnonce v
WHERE v.IdOptionAttributCategorie IN (
    SELECT o.IdOptionAttributCategorie FROM OptionsAttributCategorie o
    WHERE o.Valeur LIKE '%' + @CorruptedChar + '%'
    OR o.Valeur LIKE '%' + @QuoteCorrupted + '%'
    OR o.IdAttributCategorie IN (
        SELECT a.IdAttributCategorie FROM AttributsCategorie a
        WHERE a.Nom LIKE '%' + @CorruptedChar + '%'
        OR a.Nom LIKE '%' + @QuoteCorrupted + '%'
        OR a.IdCategorie IN (SELECT c.IdCategorie FROM Categories c WHERE c.Nom LIKE '%' + @CorruptedChar + '%')
    )
);

-- 2. Remove corrupted Options
DELETE o FROM OptionsAttributCategorie o
WHERE o.Valeur LIKE '%' + @CorruptedChar + '%'
OR o.Valeur LIKE '%' + @QuoteCorrupted + '%'
OR o.IdAttributCategorie IN (
    SELECT a.IdAttributCategorie FROM AttributsCategorie a
    WHERE a.Nom LIKE '%' + @CorruptedChar + '%'
    OR a.Nom LIKE '%' + @QuoteCorrupted + '%'
    OR a.IdCategorie IN (SELECT c.IdCategorie FROM Categories c WHERE c.Nom LIKE '%' + @CorruptedChar + '%')
);

-- 3. Remove corrupted Attributes
DELETE a FROM AttributsCategorie a
WHERE a.Nom LIKE '%' + @CorruptedChar + '%'
OR a.Nom LIKE '%' + @QuoteCorrupted + '%'
OR a.IdCategorie IN (SELECT c.IdCategorie FROM Categories c WHERE c.Nom LIKE '%' + @CorruptedChar + '%');

-- 4. Remove corrupted Categories
DELETE c FROM Categories c
WHERE c.Nom LIKE '%' + @CorruptedChar + '%'
OR c.Nom LIKE '%' + @QuoteCorrupted + '%';

-- 5. Finalize the Immobilier "Type de bien" / "Type bien" cleanup
-- (Ensure only one version exists and it is correctly named)
DECLARE @IdCatImmo INT = (SELECT IdCategorie FROM Categories WHERE Nom = N'Immobilier');
IF @IdCatImmo IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCatImmo AND Nom = N'Type bien')
    BEGIN
        IF EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCatImmo AND Nom = N'Type de bien')
        BEGIN
            UPDATE OptionsAttributCategorie SET IdAttributCategorie = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCatImmo AND Nom = N'Type de bien')
            WHERE IdAttributCategorie = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCatImmo AND Nom = N'Type bien');
            DELETE FROM AttributsCategorie WHERE IdCategorie = @IdCatImmo AND Nom = N'Type bien';
        END
        ELSE
            UPDATE AttributsCategorie SET Nom = N'Type de bien' WHERE IdCategorie = @IdCatImmo AND Nom = N'Type bien';
    END
END

-- 6. Ensure Unique Index to prevent future duplicates
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_OptionsAttributCategorie_Attribut_Valeur')
BEGIN
    CREATE UNIQUE INDEX UX_OptionsAttributCategorie_Attribut_Valeur ON OptionsAttributCategorie (IdAttributCategorie, Valeur);
END

-- ==========================================
-- MASTER SEEDING SECTION
-- ==========================================

DECLARE @IdCat INT;
DECLARE @IdAttr INT;

-- 1. Véhicules
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Véhicules')
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) VALUES (N'Véhicules', N'Voitures, motos, camions et accessoires.', N'directions_car', 1);
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Véhicules');

-- Type véhicule
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type véhicule')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type véhicule', 5, 0, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type véhicule');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Voiture',1),(N'Moto',2),(N'Camion',3),(N'Bus',4),(N'Utilitaire',5),(N'Tracteur',6),(N'Remorque',7)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

-- Marque
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Marque')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Marque', 5, 0, 1, 2);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Marque');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'BMW',1),(N'Mercedes',2),(N'Audi',3),(N'Volkswagen',4),(N'Peugeot',5),(N'Renault',6),(N'Toyota',7),(N'Hyundai',8),(N'Kia',9),(N'Fiat',10),(N'Ford',11)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

-- Modèle (TEXTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Modèle')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Modèle', 1, 0, 1, 3);

-- Année (NOMBRE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Année')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Année', 2, 0, 1, 4);

-- Kilométrage (NOMBRE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Kilométrage')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Kilométrage', 2, 0, 1, 5);

-- Carburant
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Carburant')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Carburant', 5, 0, 1, 6);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Carburant');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Essence',1),(N'Diesel',2),(N'Hybride',3),(N'Électrique',4),(N'GPL',5)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

-- Boîte vitesse
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Boîte vitesse')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Boîte vitesse', 5, 0, 1, 7);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Boîte vitesse');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Manuelle',1),(N'Automatique',2)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

-- État
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'État', 5, 0, 1, 10);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Neuf',1),(N'Très bon état',2),(N'Bon état',3),(N'État moyen',4),(N'À réparer',5)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

-- Booleans
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Première main') INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Première main', 4, 0, 1, 11);
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Climatisation') INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Climatisation', 4, 0, 1, 12);


-- 2. Immobilier
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Immobilier')
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) VALUES (N'Immobilier', N'Appartements, maisons, terrains et bureaux.', N'home', 2);
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Immobilier');

-- Type de bien
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type de bien')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type de bien', 5, 0, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type de bien');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Appartement',1),(N'Maison',2),(N'Villa',3),(N'Studio',4),(N'Terrain',5),(N'Local commercial',6),(N'Bureau',7),(N'Dépôt',8),(N'Garage',9),(N'Ferme',10),(N'Immeuble',11)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

-- Transaction
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Transaction')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Transaction', 5, 0, 1, 2);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Transaction');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Vente',1),(N'Location',2)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

-- Features (NOMBRE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Surface') INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Surface', 2, 0, 1, 3);
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Nombre pièces') INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Nombre pièces', 2, 0, 1, 4);

-- Chauffage
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Chauffage')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Chauffage', 5, 0, 1, 13);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Chauffage');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Aucun',1),(N'Gaz',2),(N'Électrique',3),(N'Central',4),(N'Climatisation réversible',5),(N'Solaire',6)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

-- État du bien
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État du bien')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'État du bien', 5, 0, 1, 14);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État du bien');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Neuf',1),(N'Très bon état',2),(N'Bon état',3),(N'À rénover',4),(N'En construction',5)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);


-- 3. Téléphones
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Téléphones')
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) VALUES (N'Téléphones', N'Smartphones, tablettes et accessoires.', N'smartphone', 3);
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Téléphones');

-- État
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'État', 5, 0, 1, 3);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Neuf',1),(N'Très bon état',2),(N'Bon état',3),(N'Écran cassé',4),(N'Batterie faible',5),(N'À réparer',6),(N'Comme neuf',7),(N'Utilisé',8)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);


-- 9. Services
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Services')
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) VALUES (N'Services', N'Plomberie, électricité, cours et autres services.', N'build', 9);
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Services');

-- Type de service
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type de service')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type de service', 5, 0, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type de service');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Réparation',1),(N'Transport',2),(N'Nettoyage',3),(N'Formation',4),(N'Design',5),(N'Développement web',6),(N'Marketing',7),(N'Photographie',8),(N'Vidéo',9),(N'Plomberie',10),(N'Électricité',11),(N'Peinture',12),(N'Menuiserie',13),(N'Maçonnerie',14),(N'Jardinage',15),(N'Mécanique',16),(N'Livraison',17),(N'Déménagement',18),(N'Assistance informatique',19),(N'Ménage',20),(N'Cours particuliers',21)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

-- Disponibilité
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Disponibilité')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Disponibilité', 5, 0, 1, 3);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Disponibilité');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Temps plein',1),(N'Temps partiel',2),(N'Week-end',3),(N'Soir',4),(N'Matin',5),(N'Après-midi',6),(N'Sur demande',7),(N'Urgence',8)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);


-- 10. Emploi
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Emploi')
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) VALUES (N'Emploi', N'Offres d''emploi, stages et formations.', N'work', 10);
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Emploi');

-- Type annonce
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type annonce')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type annonce', 5, 0, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type annonce');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Offre d’emploi',1),(N'Stage',2),(N'Freelance',3),(N'Alternance',4),(N'Mission temporaire',5)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

-- Type contrat
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type contrat')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type contrat', 5, 0, 1, 3);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type contrat');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'CDI',1),(N'CDD',2),(N'Stage',3),(N'Freelance',4),(N'Temps partiel',5),(N'Temps plein',6),(N'Saisonnier',7),(N'Contrat temporaire',8)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

-- Niveau étude
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Niveau étude')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Niveau étude', 5, 0, 1, 4);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Niveau étude');
INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) SELECT @IdAttr, T.V, T.O FROM (VALUES (N'Aucun diplôme',1),(N'Primaire',2),(N'Collège',3),(N'Bac',4),(N'Formation professionnelle',5),(N'BTP',6),(N'BTS',7),(N'Bac+2',8),(N'Licence',9),(N'Bac+3',10),(N'Master',11),(N'Bac+5',12),(N'Ingénieur',13),(N'Doctorat',14)) AS T(V,O) WHERE NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = T.V);

GO
