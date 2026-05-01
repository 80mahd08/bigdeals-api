USE BigDealsDb;
GO

DECLARE @IdCat INT;
DECLARE @IdAttr INT;

-- ==========================================
-- 1. Véhicules
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Véhicules')
BEGIN
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) 
    VALUES (N'Véhicules', N'Voitures, motos, camions et accessoires.', N'directions_car', 1);
END
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Véhicules');

-- Attributes for Véhicules
-- Marque (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Marque')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Marque', 5, 1, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Marque');

-- Options for Marque
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'BMW') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'BMW', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Mercedes') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Mercedes', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Audi') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Audi', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Volkswagen') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Volkswagen', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Peugeot') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Peugeot', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Renault') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Renault', 6);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Toyota') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Toyota', 7);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Hyundai') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Hyundai', 8);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Kia') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Kia', 9);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Fiat') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Fiat', 10);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Ford') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Ford', 11);

-- Modèle (TEXTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Modèle')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Modèle', 1, 1, 1, 2);

-- Année (NOMBRE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Année')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Année', 2, 1, 1, 3);

-- Couleur (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Couleur')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Couleur', 5, 0, 1, 4);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Couleur');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Noir') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Noir', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Blanc') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Blanc', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Gris') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Gris', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Rouge') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Rouge', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Bleu') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Bleu', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Vert') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Vert', 6);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Jaune') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Jaune', 7);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Marron') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Marron', 8);

-- Kilométrage (NOMBRE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Kilométrage')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Kilométrage', 2, 0, 1, 5);

-- Carburant (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Carburant')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Carburant', 5, 0, 1, 6);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Carburant');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Essence') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Essence', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Diesel') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Diesel', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Hybride') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Hybride', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Électrique') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Électrique', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'GPL') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'GPL', 5);

-- Transmission (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Transmission')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Transmission', 5, 0, 1, 7);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Transmission');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Manuelle') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Manuelle', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Automatique') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Automatique', 2);

-- Chevaux (NOMBRE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Chevaux')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Chevaux', 2, 0, 1, 8);


-- ==========================================
-- 2. Immobilier
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Immobilier')
BEGIN
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) 
    VALUES (N'Immobilier', N'Appartements, maisons, terrains et bureaux.', N'home', 2);
END
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Immobilier');

-- Type de bien (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type de bien')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type de bien', 5, 1, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type de bien');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Appartement') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Appartement', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Maison') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Maison', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Villa') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Villa', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Studio') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Studio', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Terrain') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Terrain', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Local commercial') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Local commercial', 6);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Bureau') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Bureau', 7);

-- Surface (NOMBRE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Surface')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Surface', 2, 0, 1, 2);

-- Nombre de pièces (NOMBRE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Nombre de pièces')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Nombre de pièces', 2, 0, 1, 3);

-- Meublé (BOOLEAN)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Meublé')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Meublé', 4, 0, 1, 4);

-- Adresse approximative (TEXTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Adresse approximative')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Adresse approximative', 1, 0, 0, 5);


-- ==========================================
-- 3. Téléphones
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Téléphones')
BEGIN
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) 
    VALUES (N'Téléphones', N'Smartphones, tablettes et accessoires.', N'smartphone', 3);
END
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Téléphones');

-- Marque (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Marque')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Marque', 5, 1, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Marque');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Apple') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Apple', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Samsung') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Samsung', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Xiaomi') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Xiaomi', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Oppo') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Oppo', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Huawei') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Huawei', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Honor') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Honor', 6);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Infinix') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Infinix', 7);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Tecno') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Tecno', 8);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Nokia') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Nokia', 9);

-- Modèle (TEXTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Modèle')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Modèle', 1, 1, 1, 2);

-- Stockage (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Stockage')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Stockage', 5, 0, 1, 3);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Stockage');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'32 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'32 Go', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'64 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'64 Go', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'128 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'128 Go', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'256 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'256 Go', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'512 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'512 Go', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'1 To') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'1 To', 6);

-- RAM (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'RAM')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'RAM', 5, 0, 1, 4);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'RAM');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'2 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'2 Go', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'3 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'3 Go', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'4 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'4 Go', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'6 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'6 Go', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'8 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'8 Go', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'12 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'12 Go', 6);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'16 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'16 Go', 7);

-- État (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'État', 5, 1, 1, 5);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Neuf', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Comme neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Comme neuf', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Bon état') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Bon état', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Utilisé') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Utilisé', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'À réparer') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'À réparer', 5);

-- Couleur (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Couleur')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Couleur', 5, 0, 1, 6);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Couleur');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Noir') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Noir', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Blanc') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Blanc', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Gris') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Gris', 3);


-- ==========================================
-- 4. Informatique
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Informatique')
BEGIN
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) 
    VALUES (N'Informatique', N'Ordinateurs, composants et périphériques.', N'computer', 4);
END
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Informatique');

-- Type (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type', 5, 1, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'PC portable') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'PC portable', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'PC bureau') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'PC bureau', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Écran') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Écran', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Clavier') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Clavier', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Souris') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Souris', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Imprimante') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Imprimante', 6);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Accessoire') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Accessoire', 7);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Composant') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Composant', 8);

-- Marque (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Marque')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Marque', 5, 0, 1, 2);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Marque');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'HP') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'HP', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Dell') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Dell', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Lenovo') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Lenovo', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Asus') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Asus', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Acer') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Acer', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Apple') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Apple', 6);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'MSI') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'MSI', 7);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Gigabyte') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Gigabyte', 8);

-- Processeur (TEXTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Processeur')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Processeur', 1, 0, 1, 3);

-- RAM (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'RAM')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'RAM', 5, 0, 1, 4);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'RAM');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'4 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'4 Go', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'8 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'8 Go', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'16 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'16 Go', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'32 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'32 Go', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'64 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'64 Go', 5);

-- Stockage (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Stockage')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Stockage', 5, 0, 1, 5);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Stockage');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'128 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'128 Go', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'256 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'256 Go', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'512 Go') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'512 Go', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'1 To') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'1 To', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'2 To') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'2 To', 5);

-- Carte graphique (TEXTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Carte graphique')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Carte graphique', 1, 0, 1, 6);

-- État (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'État', 5, 1, 1, 7);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Neuf', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Comme neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Comme neuf', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Bon état') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Bon état', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Utilisé') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Utilisé', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'À réparer') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'À réparer', 5);


-- ==========================================
-- 5. Mode
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Mode')
BEGIN
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) 
    VALUES (N'Mode', N'Vêtements, chaussures et accessoires.', N'checkroom', 5);
END
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Mode');

-- Genre (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Genre')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Genre', 5, 0, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Genre');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Homme') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Homme', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Femme') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Femme', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Enfant') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Enfant', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Unisexe') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Unisexe', 4);

-- Type (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type', 5, 1, 1, 2);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Vêtement') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Vêtement', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Chaussures') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Chaussures', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Sac') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Sac', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Montre') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Montre', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Bijoux') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Bijoux', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Accessoire') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Accessoire', 6);

-- Taille (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Taille')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Taille', 5, 0, 1, 3);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Taille');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'XS') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'XS', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'S') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'S', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'M') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'M', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'L') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'L', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'XL') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'XL', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'XXL') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'XXL', 6);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'36') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'36', 7);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'37') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'37', 8);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'38') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'38', 9);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'39') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'39', 10);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'40') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'40', 11);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'41') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'41', 12);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'42') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'42', 13);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'43') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'43', 14);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'44') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'44', 15);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'45') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'45', 16);

-- État (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'État', 5, 1, 1, 4);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Neuf', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Comme neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Comme neuf', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Bon état') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Bon état', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Utilisé') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Utilisé', 4);

-- Couleur (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Couleur')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Couleur', 5, 0, 1, 5);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Couleur');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Noir') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Noir', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Blanc') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Blanc', 2);


-- ==========================================
-- 6. Beauté
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Beauté')
BEGIN
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) 
    VALUES (N'Beauté', N'Parfums, maquillage et soins.', N'face', 6);
END
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Beauté');

-- Type (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type', 5, 1, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Parfum') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Parfum', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Maquillage') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Maquillage', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Soin visage') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Soin visage', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Soin cheveux') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Soin cheveux', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Accessoire beauté') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Accessoire beauté', 5);

-- Marque (TEXTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Marque')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Marque', 1, 0, 1, 2);

-- État (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'État', 5, 1, 1, 3);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Neuf', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Comme neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Comme neuf', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Utilisé') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Utilisé', 3);


-- ==========================================
-- 7. Maison
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Maison')
BEGIN
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) 
    VALUES (N'Maison', N'Meubles, décoration et électroménager.', N'weekend', 7);
END
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Maison');

-- Type (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type', 5, 1, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Meuble') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Meuble', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Décoration') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Décoration', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Électroménager') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Électroménager', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Cuisine') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Cuisine', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Literie') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Literie', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Rangement') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Rangement', 6);

-- Matière (TEXTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Matière')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Matière', 1, 0, 1, 2);

-- État (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'État', 5, 1, 1, 3);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Neuf', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Comme neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Comme neuf', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Bon état') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Bon état', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Utilisé') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Utilisé', 4);


-- ==========================================
-- 8. Jardin
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Jardin')
BEGIN
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) 
    VALUES (N'Jardin', N'Plantes, outils et mobilier de jardin.', N'park', 8);
END
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Jardin');

-- Type (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type', 5, 1, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Plante') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Plante', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Outil jardin') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Outil jardin', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Mobilier jardin') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Mobilier jardin', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Décoration jardin') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Décoration jardin', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Arrosage') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Arrosage', 5);

-- État (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'État', 5, 1, 1, 2);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'État');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Neuf', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Comme neuf') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Comme neuf', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Bon état') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Bon état', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Utilisé') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Utilisé', 4);


-- ==========================================
-- 9. Services
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Services')
BEGIN
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) 
    VALUES (N'Services', N'Plomberie, électricité, cours et autres services.', N'build', 9);
END
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Services');

-- Type de service (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type de service')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type de service', 5, 1, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type de service');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Plomberie') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Plomberie', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Électricité') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Électricité', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Peinture') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Peinture', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Ménage') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Ménage', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Transport') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Transport', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Réparation') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Réparation', 6);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Cours particuliers') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Cours particuliers', 7);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Design') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Design', 8);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Développement web') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Développement web', 9);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Photographie') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Photographie', 10);

-- Disponibilité (TEXTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Disponibilité')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Disponibilité', 1, 0, 0, 2);

-- Zone d’intervention (TEXTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Zone d’intervention')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Zone d’intervention', 1, 0, 1, 3);


-- ==========================================
-- 10. Emploi
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Nom = N'Emploi')
BEGIN
    INSERT INTO Categories (Nom, Description, IconKey, OrdreAffichage) 
    VALUES (N'Emploi', N'Offres d''emploi, stages et formations.', N'work', 10);
END
SET @IdCat = (SELECT IdCategorie FROM Categories WHERE Nom = N'Emploi');

-- Type de contrat (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type de contrat')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Type de contrat', 5, 1, 1, 1);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Type de contrat');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'CDI') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'CDI', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'CDD') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'CDD', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Stage') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Stage', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Freelance') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Freelance', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Temps partiel') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Temps partiel', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Temps plein') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Temps plein', 6);

-- Domaine (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Domaine')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Domaine', 5, 0, 1, 2);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Domaine');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Informatique') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Informatique', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Commerce') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Commerce', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Marketing') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Marketing', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Éducation') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Éducation', 4);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Santé') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Santé', 5);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Industrie') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Industrie', 6);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Transport') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Transport', 7);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Restauration') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Restauration', 8);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Administration') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Administration', 9);

-- Expérience requise (LISTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Expérience requise')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Expérience requise', 5, 0, 1, 3);
SET @IdAttr = (SELECT IdAttributCategorie FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Expérience requise');

IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'Débutant') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'Débutant', 1);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'1-2 ans') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'1-2 ans', 2);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'3-5 ans') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'3-5 ans', 3);
IF NOT EXISTS (SELECT 1 FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttr AND Valeur = N'5 ans et plus') INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage) VALUES (@IdAttr, N'5 ans et plus', 4);

-- Ville (TEXTE)
IF NOT EXISTS (SELECT 1 FROM AttributsCategorie WHERE IdCategorie = @IdCat AND Nom = N'Ville')
    INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage) VALUES (@IdCat, N'Ville', 1, 0, 1, 4);

GO
