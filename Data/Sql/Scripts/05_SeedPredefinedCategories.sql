USE BigDealsDb;
GO

/*
    BigDeals - Final Clean Predefined Category Seed

    Final category system columns:
    Categories: IdCategorie, Nom, Description, IconKey, OrdreAffichage, DateCreation
    AttributsCategorie: IdAttributCategorie, IdCategorie, Nom, TypeDonnee, OrdreAffichage, Placeholder, EstPlage
    OptionsAttributCategorie: IdOptionAttributCategorie, IdAttributCategorie, Valeur, OrdreAffichage

    Removed from category system:
    - Categories.EstActive
    - AttributsCategorie.Obligatoire
    - AttributsCategorie.Filtrable
    - AttributsCategorie.EstActive
    - OptionsAttributCategorie.EstActive

    TypeDonnee mapping:
    1 = TEXTE
    2 = NOMBRE
    3 = DATE
    4 = BOOLEAN
    5 = LISTE

    Rule:
    Only LISTE attributes can have options.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @TEXTE INT = 1;
DECLARE @NOMBRE INT = 2;
DECLARE @DATE INT = 3;
DECLARE @BOOLEAN INT = 4;
DECLARE @LISTE INT = 5;

------------------------------------------------------------
-- 0. Safety check
------------------------------------------------------------
IF OBJECT_ID(N'dbo.Annonces', N'U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM dbo.Annonces)
BEGIN
    THROW 51000, 'STOP: Annonces table contains data. Category reset is unsafe.', 1;
END;

IF OBJECT_ID(N'dbo.ValeursAttributAnnonce', N'U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM dbo.ValeursAttributAnnonce)
BEGIN
    THROW 51001, 'STOP: ValeursAttributAnnonce table contains data. Category reset is unsafe.', 1;
END;

------------------------------------------------------------
-- 1. Temp seed tables
------------------------------------------------------------
IF OBJECT_ID('tempdb..#CategorySeed') IS NOT NULL DROP TABLE #CategorySeed;
IF OBJECT_ID('tempdb..#AttributeSeed') IS NOT NULL DROP TABLE #AttributeSeed;
IF OBJECT_ID('tempdb..#OptionSeed') IS NOT NULL DROP TABLE #OptionSeed;

CREATE TABLE #CategorySeed
(
    Nom NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    IconKey NVARCHAR(100) NULL,
    OrdreAffichage INT NOT NULL
);

CREATE TABLE #AttributeSeed
(
    CategoryName NVARCHAR(100) NOT NULL,
    Nom NVARCHAR(100) NOT NULL,
    TypeDonnee INT NOT NULL,
    OrdreAffichage INT NOT NULL,
    Placeholder NVARCHAR(255) NULL,
    EstPlage BIT NOT NULL
);

CREATE TABLE #OptionSeed
(
    CategoryName NVARCHAR(100) NOT NULL,
    AttributeName NVARCHAR(100) NOT NULL,
    Valeur NVARCHAR(150) NOT NULL,
    OrdreAffichage INT NOT NULL
);

------------------------------------------------------------
-- 2. Categories
------------------------------------------------------------
INSERT INTO #CategorySeed (Nom, Description, IconKey, OrdreAffichage)
VALUES
(N'Véhicules', N'Voitures, motos, camions et autres véhicules.', N'ri-car-line', 1),
(N'Immobilier', N'Maisons, appartements, terrains et biens immobiliers.', N'ri-home-4-line', 2),
(N'Téléphones', N'Téléphones, smartphones et accessoires mobiles.', N'ri-smartphone-line', 3),
(N'Informatique', N'Ordinateurs, composants, écrans et matériel informatique.', N'ri-computer-line', 4),
(N'Mode', N'Veêtements, chaussures, accessoires et articles de mode.', N'ri-shirt-line', 5),
(N'Beauté', N'Produits de beauté, soin, parfums et cosmétique.', N'ri-magic-line', 6),
(N'Maison', N'Meubles, décoration, électroménager et articles maison.', N'ri-home-heart-line', 7),
(N'Jardin', N'Plantes, outils, mobilier et articles de jardin.', N'ri-leaf-line', 8),
(N'Services', N'Services professionnels, réparation, transport et assistance.', N'ri-tools-line', 9),
(N'Emploi', N'Offres d’emploi, stages, missions et opportunités de travail.', N'ri-briefcase-line', 10);

------------------------------------------------------------
-- 3. Attributes
------------------------------------------------------------
INSERT INTO #AttributeSeed (CategoryName, Nom, TypeDonnee, OrdreAffichage, Placeholder, EstPlage)
VALUES
(N'Véhicules', N'Type véhicule', 5, 1, NULL, 0),
(N'Véhicules', N'Marque', 1, 2, N'Ex: BMW', 0),
(N'Véhicules', N'Modèle', 1, 3, N'Ex: Série 3', 0),
(N'Véhicules', N'Année', 2, 4, N'Ex: 2020', 1),
(N'Véhicules', N'Kilométrage', 2, 5, N'km', 1),
(N'Véhicules', N'Carburant', 5, 6, NULL, 0),
(N'Véhicules', N'Boîte vitesse', 5, 7, NULL, 0),
(N'Véhicules', N'Puissance fiscale', 2, 8, N'CV', 1),
(N'Véhicules', N'Couleur', 1, 9, N'Ex: Noir', 0),
(N'Véhicules', N'État', 5, 10, NULL, 0),
(N'Véhicules', N'Première main', 4, 11, NULL, 0),
(N'Véhicules', N'Climatisation', 4, 12, NULL, 0),
(N'Vehicules', N'Nombre portes', 2, 13, NULL, 0),

(N'Immobilier', N'Type bien', 5, 1, NULL, 0),
(N'Immobilier', N'Transaction', 5, 2, NULL, 0),
(N'Immobilier', N'Surface', 2, 3, N'm²', 1),
(N'Immobilier', N'Nombre pièces', 2, 4, NULL, 1),
(N'Immobilier', N'Nombre chambres', 2, 5, NULL, 1),
(N'Immobilier', N'Nombre salles de bain', 2, 6, NULL, 1),
(N'Immobilier', N'Étage', 2, 7, NULL, 0),
(N'Immobilier', N'Meublé', 4, 8, NULL, 0),
(N'Immobilier', N'Ascenseur', 4, 9, NULL, 0),
(N'Immobilier', N'Garage', 4, 10, NULL, 0),
(N'Immobilier', N'Jardin', 4, 11, NULL, 0),
(N'Immobilier', N'Terrasse / Balcon', 4, 12, NULL, 0),
(N'Immobilier', N'Chauffage', 5, 13, NULL, 0),
(N'Immobilier', N'État du bien', 5, 14, NULL, 0),

(N'Téléphones', N'Marque', 1, 1, N'Ex: Samsung', 0),
(N'Téléphones', N'Modèle', 1, 2, N'Ex: Galaxy S23', 0),
(N'Téléphones', N'État', 5, 3, NULL, 0),
(N'Téléphones', N'Stockage', 2, 4, N'GB', 1),
(N'Téléphones', N'RAM', 2, 5, N'GB', 1),
(N'Téléphones', N'Couleur', 1, 6, N'Ex: Noir', 0),
(N'Téléphones', N'Double SIM', 4, 7, NULL, 0),
(N'Téléphones', N'5G', 4, 8, NULL, 0),
(N'Téléphones', N'Batterie changée', 4, 9, NULL, 0),
(N'Téléphones', N'Avec chargeur', 4, 10, NULL, 0),
(N'Téléphones', N'Avec boîte', 4, 11, NULL, 0),
(N'Téléphones', N'Garantie', 4, 12, NULL, 0),
(N'Téléphones', N'Durée garantie', 2, 13, N'mois', 1),

(N'Informatique', N'Type produit', 5, 1, NULL, 0),
(N'Informatique', N'Marque', 1, 2, N'Ex: HP', 0),
(N'Informatique', N'Modèle', 1, 3, NULL, 0),
(N'Informatique', N'Processeur', 1, 4, N'Ex: i5, Ryzen 5', 0),
(N'Informatique', N'RAM', 2, 5, N'GB', 1),
(N'Informatique', N'Stockage', 2, 6, N'GB', 1),
(N'Informatique', N'Carte graphique', 1, 7, NULL, 0),
(N'Informatique', N'Taille écran', 2, 8, N'pouces', 1),
(N'Informatique', N'Résolution', 1, 9, N'Ex: 1920x1080', 0),
(N'Informatique', N'Système', 1, 10, N'Ex: Windows 11', 0),
(N'Informatique', N'État', 5, 11, NULL, 0),
(N'Informatique', N'Garantie', 4, 12, NULL, 0),

-- UPDATED: Mode / Type article is now LISTE, not TEXTE
(N'Mode', N'Type article', 5, 1, NULL, 0),
(N'Mode', N'Genre', 5, 2, NULL, 0),
(N'Mode', N'Marque', 1, 3, NULL, 0),
(N'Mode', N'Taille', 1, 4, N'Ex: M, L, 42...', 0),
(N'Mode', N'Couleur', 1, 5, NULL, 0),
(N'Mode', N'Matière', 1, 6, N'Ex: Coton', 0),
(N'Mode', N'État', 5, 7, NULL, 0),
(N'Mode', N'Original', 4, 8, NULL, 0),
(N'Mode', N'Saison', 5, 9, NULL, 0),

(N'Beauté', N'Type produit', 5, 1, NULL, 0),
(N'Beauté', N'Marque', 1, 2, NULL, 0),
(N'Beauté', N'Genre', 5, 3, NULL, 0),
(N'Beauté', N'État', 5, 4, NULL, 0),
(N'Beauté', N'Volume', 2, 5, N'ml', 1),
(N'Beauté', N'Type peau', 5, 6, NULL, 0),
(N'Beauté', N'Type cheveux', 5, 7, NULL, 0),
(N'Beauté', N'Bio / Naturel', 4, 8, NULL, 0),
(N'Beauté', N'Date expiration', 3, 9, NULL, 1),
(N'Beauté', N'Original', 4, 10, NULL, 0),

(N'Maison', N'Type article', 5, 1, NULL, 0),
(N'Maison', N'Marque', 1, 2, NULL, 0),
(N'Maison', N'Matière', 5, 3, NULL, 0),
(N'Maison', N'Couleur', 1, 4, NULL, 0),
(N'Maison', N'État', 5, 5, NULL, 0),
(N'Maison', N'Longueur', 2, 6, N'cm', 1),
(N'Maison', N'Largeur', 2, 7, N'cm', 1),
(N'Maison', N'Hauteur', 2, 8, N'cm', 1),
(N'Maison', N'Surface', 2, 9, N'm²', 1),
(N'Maison', N'Poids', 2, 10, N'kg', 1),
(N'Maison', N'Démontable', 4, 11, NULL, 0),
(N'Maison', N'Livraison possible', 4, 12, NULL, 0),

(N'Jardin', N'Type article', 5, 1, NULL, 0),
(N'Jardin', N'État', 5, 2, NULL, 0),
(N'Jardin', N'Matière', 5, 3, NULL, 0),
(N'Jardin', N'Taille', 2, 4, N'cm', 1),
(N'Jardin', N'Type plante', 5, 5, NULL, 0),
(N'Jardin', N'Exposition soleil', 5, 6, NULL, 0),
(N'Jardin', N'Arrosage', 5, 7, NULL, 0),
(N'Jardin', N'Avec pot', 4, 8, NULL, 0),
(N'Jardin', N'Livraison possible', 4, 9, NULL, 0),

(N'Services', N'Type service', 5, 1, NULL, 0),
(N'Services', N'Expérience', 2, 2, N'années', 1),
(N'Services', N'Disponibilité', 5, 3, NULL, 0),
(N'Services', N'Zone service', 1, 4, N'Ex: Tunis, Sfax...', 0),
(N'Services', N'Déplacement possible', 4, 5, NULL, 0),
(N'Services', N'Service à distance', 4, 6, NULL, 0),
(N'Services', N'Tarif par', 5, 7, NULL, 0),
(N'Services', N'Durée estimée', 2, 8, N'heures', 1),
(N'Services', N'Certification', 4, 9, NULL, 0),
(N'Services', N'Urgence acceptée', 4, 10, NULL, 0),

(N'Emploi', N'Type annonce', 5, 1, NULL, 0),
(N'Emploi', N'Domaine', 1, 2, N'Ex: Informatique', 0),
(N'Emploi', N'Type contrat', 5, 3, NULL, 0),
(N'Emploi', N'Niveau étude', 5, 4, NULL, 0),
(N'Emploi', N'Expérience requise', 2, 5, N'années', 1),
(N'Emploi', N'Salaire min', 2, 6, N'TND', 1),
(N'Emploi', N'Salaire max', 2, 7, N'TND', 1),
(N'Emploi', N'Télétravail', 4, 8, NULL, 0),
(N'Emploi', N'Compétences', 1, 9, NULL, 0),
(N'Emploi', N'Date début', 3, 10, NULL, 1),
(N'Emploi', N'Langues', 1, 11, N'Ex: Français, Anglais', 0);

------------------------------------------------------------
-- 4. Options - Only for LISTE attributes
------------------------------------------------------------
INSERT INTO #OptionSeed (CategoryName, AttributeName, Valeur, OrdreAffichage)
VALUES
(N'Véhicules', N'Type véhicule', N'Voiture', 1),
(N'Véhicules', N'Type véhicule', N'Moto', 2),
(N'Véhicules', N'Type véhicule', N'Camion', 3),
(N'Véhicules', N'Type véhicule', N'Bus', 4),
(N'Véhicules', N'Type véhicule', N'Utilitaire', 5),
(N'Véhicules', N'Type véhicule', N'Tracteur', 6),
(N'Véhicules', N'Type véhicule', N'Remorque', 7),

(N'Véhicules', N'Carburant', N'Essence', 1),
(N'Véhicules', N'Carburant', N'Diesel', 2),
(N'Véhicules', N'Carburant', N'Hybride', 3),
(N'Véhicules', N'Carburant', N'Électrique', 4),
(N'Vehicules', N'Carburant', N'GPL', 5),

(N'Véhicules', N'Boîte vitesse', N'Manuelle', 1),
(N'Véhicules', N'Boîte vitesse', N'Automatique', 2),

(N'Véhicules', N'État', N'Neuf', 1),
(N'Véhicules', N'État', N'Très bon état', 2),
(N'Véhicules', N'État', N'Bon état', 3),
(N'Véhicules', N'État', N'État moyen', 4),
(N'Véhicules', N'État', N'À réparer', 5),

(N'Immobilier', N'Type bien', N'Appartement', 1),
(N'Immobilier', N'Type bien', N'Maison', 2),
(N'Immobilier', N'Type bien', N'Villa', 3),
(N'Immobilier', N'Type bien', N'Studio', 4),
(N'Immobilier', N'Type bien', N'Terrain', 5),
(N'Immobilier', N'Type bien', N'Bureau', 6),
(N'Immobilier', N'Type bien', N'Local commercial', 7),
(N'Immobilier', N'Type bien', N'Garage', 8),
(N'Immobilier', N'Type bien', N'Immeuble', 9),
(N'Immobilier', N'Type bien', N'Ferme', 10),

(N'Immobilier', N'Transaction', N'Vente', 1),
(N'Immobilier', N'Transaction', N'Location', 2),

(N'Immobilier', N'Chauffage', N'Aucun', 1),
(N'Immobilier', N'Chauffage', N'Gaz', 2),
(N'Immobilier', N'Chauffage', N'Électrique', 3),
(N'Immobilier', N'Chauffage', N'Central', 4),
(N'Immobilier', N'Chauffage', N'Climatisation réversible', 5),
(N'Immobilier', N'Chauffage', N'Solaire', 6),

(N'Immobilier', N'État du bien', N'Neuf', 1),
(N'Immobilier', N'État du bien', N'Très bon état', 2),
(N'Immobilier', N'État du bien', N'Bon état', 3),
(N'Immobilier', N'État du bien', N'À rénover', 4),
(N'Immobilier', N'État du bien', N'En construction', 5),

(N'Téléphones', N'État', N'Neuf', 1),
(N'Téléphones', N'État', N'Très bon état', 2),
(N'Téléphones', N'État', N'Bon état', 3),
(N'Téléphones', N'État', N'Écran cassé', 4),
(N'Téléphones', N'État', N'Batterie faible', 5),
(N'Téléphones', N'État', N'À réparer', 6),

(N'Informatique', N'Type produit', N'PC portable', 1),
(N'Informatique', N'Type produit', N'PC bureau', 2),
(N'Informatique', N'Type produit', N'Écran', 3),
(N'Informatique', N'Type produit', N'Clavier', 4),
(N'Informatique', N'Type produit', N'Souris', 5),
(N'Informatique', N'Type produit', N'Imprimante', 6),
(N'Informatique', N'Type produit', N'Scanner', 7),
(N'Informatique', N'Type produit', N'Composant PC', 8),
(N'Informatique', N'Type produit', N'Carte graphique', 9),
(N'Informatique', N'Type produit', N'Disque dur / SSD', 10),
(N'Informatique', N'Type produit', N'Accessoire informatique', 11),
(N'Informatique', N'Type produit', N'Réseau', 12),
(N'Informatique', N'Type produit', N'Logiciel', 13),

(N'Informatique', N'État', N'Neuf', 1),
(N'Informatique', N'État', N'Très bon état', 2),
(N'Informatique', N'État', N'Bon état', 3),
(N'Informatique', N'État', N'État moyen', 4),
(N'Informatique', N'État', N'À réparer', 5),

-- UPDATED: Options for Mode / Type article
(N'Mode', N'Type article', N'T-shirt', 1),
(N'Mode', N'Type article', N'Chemise', 2),
(N'Mode', N'Type article', N'Pantalon', 3),
(N'Mode', N'Type article', N'Jean', 4),
(N'Mode', N'Type article', N'Robe', 5),
(N'Mode', N'Type article', N'Jupe', 6),
(N'Mode', N'Type article', N'Veste', 7),
(N'Mode', N'Type article', N'Manteau', 8),
(N'Mode', N'Type article', N'Pull', 9),
(N'Mode', N'Type article', N'Chaussures', 10),
(N'Mode', N'Type article', N'Sac', 11),
(N'Mode', N'Type article', N'Montre', 12),
(N'Mode', N'Type article', N'Bijoux', 13),
(N'Mode', N'Type article', N'Lunettes', 14),
(N'Mode', N'Type article', N'Ceinture', 15),
(N'Mode', N'Type article', N'Casquette', 16),
(N'Mode', N'Type article', N'Hijab / Foulard', 17),
(N'Mode', N'Type article', N'Accessoire', 18),

(N'Mode', N'Genre', N'Homme', 1),
(N'Mode', N'Genre', N'Femme', 2),
(N'Mode', N'Genre', N'Enfant', 3),
(N'Mode', N'Genre', N'Bébé', 4),
(N'Mode', N'Genre', N'Unisexe', 5),

(N'Mode', N'État', N'Neuf', 1),
(N'Mode', N'État', N'Très bon état', 2),
(N'Mode', N'État', N'Bon état', 3),
(N'Mode', N'État', N'État moyen', 4),

(N'Mode', N'Saison', N'Printemps', 1),
(N'Mode', N'Saison', N'Été', 2),
(N'Mode', N'Saison', N'Automne', 3),
(N'Mode', N'Saison', N'Hiver', 4),
(N'Mode', N'Saison', N'Toutes saisons', 5),

(N'Beauté', N'Type produit', N'Parfum', 1),
(N'Beauté', N'Type produit', N'Maquillage', 2),
(N'Beauté', N'Type produit', N'Soin visage', 3),
(N'Beauté', N'Type produit', N'Soin corps', 4),
(N'Beauté', N'Type produit', N'Soin cheveux', 5),
(N'Beauté', N'Type produit', N'Hygiène', 6),
(N'Beauté', N'Type produit', N'Accessoire beauté', 7),
(N'Beauté', N'Type produit', N'Produit naturel', 8),
(N'Beauté', N'Type produit', N'Crème', 9),
(N'Beauté', N'Type produit', N'Shampoing', 10),
(N'Beauté', N'Type produit', N'Gel douche', 11),
(N'Beauté', N'Type produit', N'Déodorant', 12),

(N'Beauté', N'Genre', N'Homme', 1),
(N'Beauté', N'Genre', N'Femme', 2),
(N'Beauté', N'Genre', N'Unisexe', 3),

(N'Beauté', N'État', N'Neuf', 1),
(N'Beauté', N'État', N'Utilisé une fois', 2),
(N'Beauté', N'État', N'Ouvert', 3),
(N'Beauté', N'État', N'Presque vide', 4),

(N'Beauté', N'Type peau', N'Peau normale', 1),
(N'Beauté', N'Type peau', N'Peau sèche', 2),
(N'Beauté', N'Type peau', N'Peau grasse', 3),
(N'Beauté', N'Type peau', N'Peau mixte', 4),
(N'Beauté', N'Type peau', N'Peau sensible', 5),
(N'Beauté', N'Type peau', N'Peau acnéique', 6),
(N'Beauté', N'Type peau', N'Tous types de peau', 7),

(N'Beauté', N'Type cheveux', N'Cheveux normaux', 1),
(N'Beauté', N'Type cheveux', N'Cheveux secs', 2),
(N'Beauté', N'Type cheveux', N'Cheveux gras', 3),
(N'Beauté', N'Type cheveux', N'Cheveux bouclés', 4),
(N'Beauté', N'Type cheveux', N'Cheveux frisés', 5),
(N'Beauté', N'Type cheveux', N'Cheveux colorés', 6),
(N'Beauté', N'Type cheveux', N'Cheveux abîmés', 7),
(N'Beauté', N'Type cheveux', N'Tous types de cheveux', 8),

(N'Maison', N'Type article', N'Meuble', 1),
(N'Maison', N'Type article', N'Canapé', 2),
(N'Maison', N'Type article', N'Lit', 3),
(N'Maison', N'Type article', N'Table', 4),
(N'Maison', N'Type article', N'Chaise', 5),
(N'Maison', N'Type article', N'Armoire', 6),
(N'Maison', N'Type article', N'Décoration', 7),
(N'Maison', N'Type article', N'Tapis', 8),
(N'Maison', N'Type article', N'Rideau', 9),
(N'Maison', N'Type article', N'Cuisine', 10),
(N'Maison', N'Type article', N'Vaisselle', 11),
(N'Maison', N'Type article', N'Électroménager', 12),
(N'Maison', N'Type article', N'Literie', 13),
(N'Maison', N'Type article', N'Rangement', 14),
(N'Maison', N'Type article', N'Luminaire', 15),

(N'Maison', N'Matière', N'Bois', 1),
(N'Maison', N'Matière', N'Métal', 2),
(N'Maison', N'Matière', N'Plastique', 3),
(N'Maison', N'Matière', N'Verre', 4),
(N'Maison', N'Matière', N'Tissu', 5),
(N'Maison', N'Matière', N'Cuir', 6),
(N'Maison', N'Matière', N'Simili cuir', 7),
(N'Maison', N'Matière', N'Céramique', 8),
(N'Maison', N'Matière', N'Marbre', 9),
(N'Maison', N'Matière', N'Rotin', 10),
(N'Maison', N'Matière', N'Bambou', 11),

(N'Maison', N'État', N'Neuf', 1),
(N'Maison', N'État', N'Très bon état', 2),
(N'Maison', N'État', N'Bon état', 3),
(N'Maison', N'État', N'État moyen', 4),
(N'Maison', N'État', N'À réparer', 5),

(N'Jardin', N'Type article', N'Plante', 1),
(N'Jardin', N'Type article', N'Arbre', 2),
(N'Jardin', N'Type article', N'Fleur', 3),
(N'Jardin', N'Type article', N'Pot', 4),
(N'Jardin', N'Type article', N'Terreau', 5),
(N'Jardin', N'Type article', N'Outil jardin', 6),
(N'Jardin', N'Type article', N'Mobilier jardin', 7),
(N'Jardin', N'Type article', N'Décoration extérieure', 8),
(N'Jardin', N'Type article', N'Arrosage', 9),
(N'Jardin', N'Type article', N'Gazon', 10),
(N'Jardin', N'Type article', N'Graines', 11),
(N'Jardin', N'Type article', N'Engrais', 12),

(N'Jardin', N'État', N'Neuf', 1),
(N'Jardin', N'État', N'Très bon état', 2),
(N'Jardin', N'État', N'Bon état', 3),
(N'Jardin', N'État', N'État moyen', 4),

(N'Jardin', N'Matière', N'Bois', 1),
(N'Jardin', N'Matière', N'Métal', 2),
(N'Jardin', N'Matière', N'Plastique', 3),
(N'Jardin', N'Matière', N'Terre cuite', 4),
(N'Jardin', N'Matière', N'Céramique', 5),
(N'Jardin', N'Matière', N'Rotin', 6),
(N'Jardin', N'Matière', N'Bambou', 7),
(N'Jardin', N'Matière', N'Tissu', 8),
(N'Jardin', N'Matière', N'Pierre', 9),

(N'Jardin', N'Type plante', N'Plante intérieure', 1),
(N'Jardin', N'Type plante', N'Plante extérieure', 2),
(N'Jardin', N'Type plante', N'Fleur', 3),
(N'Jardin', N'Type plante', N'Arbre fruitier', 4),
(N'Jardin', N'Type plante', N'Arbre décoratif', 5),
(N'Jardin', N'Type plante', N'Cactus', 6),
(N'Jardin', N'Type plante', N'Succulente', 7),
(N'Jardin', N'Type plante', N'Plante aromatique', 8),
(N'Jardin', N'Type plante', N'Plante grimpante', 9),

(N'Jardin', N'Exposition soleil', N'Plein soleil', 1),
(N'Jardin', N'Exposition soleil', N'Mi-ombre', 2),
(N'Jardin', N'Exposition soleil', N'Ombre', 3),
(N'Jardin', N'Exposition soleil', N'Intérieur lumineux', 4),
(N'Jardin', N'Exposition soleil', N'Faible lumière', 5),

(N'Jardin', N'Arrosage', N'Faible', 1),
(N'Jardin', N'Arrosage', N'Moyen', 2),
(N'Jardin', N'Arrosage', N'Fréquent', 3),
(N'Jardin', N'Arrosage', N'Quotidien', 4),
(N'Jardin', N'Arrosage', N'Rare', 5),

(N'Services', N'Type service', N'Réparation', 1),
(N'Services', N'Type service', N'Transport', 2),
(N'Services', N'Type service', N'Nettoyage', 3),
(N'Services', N'Type service', N'Formation', 4),
(N'Services', N'Type service', N'Design', 5),
(N'Services', N'Type service', N'Développement web', 6),
(N'Services', N'Type service', N'Marketing', 7),
(N'Services', N'Type service', N'Photographie', 8),
(N'Services', N'Type service', N'Vidéo', 9),
(N'Services', N'Type service', N'Plomberie', 10),
(N'Services', N'Type service', N'Électricité', 11),
(N'Services', N'Type service', N'Peinture', 12),
(N'Services', N'Type service', N'Menuiserie', 13),
(N'Services', N'Type service', N'Maçonnerie', 14),
(N'Services', N'Type service', N'Jardinage', 15),
(N'Services', N'Type service', N'Mécanique', 16),
(N'Services', N'Type service', N'Livraison', 17),
(N'Services', N'Type service', N'Déménagement', 18),
(N'Services', N'Type service', N'Assistance informatique', 19),

(N'Services', N'Disponibilité', N'Temps plein', 1),
(N'Services', N'Disponibilité', N'Temps partiel', 2),
(N'Services', N'Disponibilité', N'Week-end', 3),
(N'Services', N'Disponibilité', N'Soir', 4),
(N'Services', N'Disponibilité', N'Matin', 5),
(N'Services', N'Disponibilité', N'Après-midi', 6),
(N'Services', N'Disponibilité', N'Sur demande', 7),
(N'Services', N'Disponibilité', N'Urgence', 8),

(N'Services', N'Tarif par', N'Heure', 1),
(N'Services', N'Tarif par', N'Jour', 2),
(N'Services', N'Tarif par', N'Projet', 3),
(N'Services', N'Tarif par', N'Intervention', 4),
(N'Services', N'Tarif par', N'Mois', 5),

(N'Emploi', N'Type annonce', N'Offre d’emploi', 1),
(N'Emploi', N'Type annonce', N'Stage', 2),
(N'Emploi', N'Type annonce', N'Freelance', 3),
(N'Emploi', N'Type annonce', N'Alternance', 4),
(N'Emploi', N'Type annonce', N'Mission temporaire', 5),

(N'Emploi', N'Type contrat', N'CDI', 1),
(N'Emploi', N'Type contrat', N'CDD', 2),
(N'Emploi', N'Type contrat', N'Stage', 3),
(N'Emploi', N'Type contrat', N'Freelance', 4),
(N'Emploi', N'Type contrat', N'Temps partiel', 5),
(N'Emploi', N'Type contrat', N'Temps plein', 6),
(N'Emploi', N'Type contrat', N'Saisonnier', 7),
(N'Emploi', N'Type contrat', N'Contrat temporaire', 8),

(N'Emploi', N'Niveau étude', N'Aucun diplôme', 1),
(N'Emploi', N'Niveau étude', N'Primaire', 2),
(N'Emploi', N'Niveau étude', N'Collège', 3),
(N'Emploi', N'Niveau étude', N'Bac', 4),
(N'Emploi', N'Niveau étude', N'Formation professionnelle', 5),
(N'Emploi', N'Niveau étude', N'BTP', 6),
(N'Emploi', N'Niveau étude', N'BTS', 7),
(N'Emploi', N'Niveau étude', N'Bac+2', 8),
(N'Emploi', N'Niveau étude', N'Licence', 9),
(N'Emploi', N'Niveau étude', N'Bac+3', 10),
(N'Emploi', N'Niveau étude', N'Master', 11),
(N'Emploi', N'Niveau étude', N'Bac+5', 12),
(N'Emploi', N'Niveau étude', N'Ingénieur', 13),
(N'Emploi', N'Niveau étude', N'Doctorat', 14);

------------------------------------------------------------
-- 5. Validate seed before touching real tables
------------------------------------------------------------
IF EXISTS
(
    SELECT 1
    FROM #OptionSeed o
    LEFT JOIN #AttributeSeed a
        ON a.CategoryName = o.CategoryName
       AND a.Nom = o.AttributeName
    WHERE a.Nom IS NULL
)
BEGIN
    THROW 51002, 'Seed error: Some options reference attributes that do not exist.', 1;
END;

IF EXISTS
(
    SELECT 1
    FROM #OptionSeed o
    JOIN #AttributeSeed a
        ON a.CategoryName = o.CategoryName
       AND a.Nom = o.AttributeName
    WHERE a.TypeDonnee <> 5
)
BEGIN
    THROW 51003, 'Seed error: Some options are attached to non-LISTE attributes.', 1;
END;

IF EXISTS
(
    SELECT 1
    FROM #AttributeSeed a
    WHERE a.TypeDonnee = 5
      AND NOT EXISTS
      (
          SELECT 1
          FROM #OptionSeed o
          WHERE o.CategoryName = a.CategoryName
            AND o.AttributeName = a.Nom
      )
)
BEGIN
    THROW 51004, 'Seed error: Some LISTE attributes have no options.', 1;
END;

IF EXISTS
(
    SELECT 1
    FROM #AttributeSeed
    WHERE Nom LIKE N'%Transmission%'
)
BEGIN
    THROW 51011, 'Seed error: Transmission attribute must not exist. Use Boîte vitesse instead.', 1;
END;

------------------------------------------------------------
-- 6. Clean category schema, reset category data, seed real tables
------------------------------------------------------------
BEGIN TRY
    BEGIN TRANSACTION;

    DELETE FROM dbo.OptionsAttributCategorie;
    DELETE FROM dbo.AttributsCategorie;
    DELETE FROM dbo.Categories;

    ------------------------------------------------------------
    -- Ensure kept columns exist for existing databases
    ------------------------------------------------------------
    IF COL_LENGTH('dbo.Categories', 'DateCreation') IS NULL
    BEGIN
        ALTER TABLE dbo.Categories
        ADD DateCreation DATETIME2 NOT NULL
            CONSTRAINT DF_Categories_DateCreation DEFAULT SYSUTCDATETIME();
    END;

    IF COL_LENGTH('dbo.AttributsCategorie', 'Placeholder') IS NULL
    BEGIN
        ALTER TABLE dbo.AttributsCategorie ADD Placeholder NVARCHAR(255) NULL;
    END;

    IF COL_LENGTH('dbo.AttributsCategorie', 'EstPlage') IS NULL
    BEGIN
        ALTER TABLE dbo.AttributsCategorie
        ADD EstPlage BIT NOT NULL
            CONSTRAINT DF_AttributsCategorie_EstPlage DEFAULT 0;
    END;

    ------------------------------------------------------------
    -- Drop indexes depending on removed category columns
    ------------------------------------------------------------
    DECLARE @sql NVARCHAR(MAX) = N'';

    ;WITH IndexesToDrop AS
    (
        SELECT DISTINCT
            s.name AS SchemaName,
            t.name AS TableName,
            i.name AS IndexName
        FROM sys.indexes i
        JOIN sys.index_columns ic
            ON ic.object_id = i.object_id
           AND ic.index_id = i.index_id
        JOIN sys.columns c
            ON c.object_id = ic.object_id
           AND c.column_id = ic.column_id
        JOIN sys.tables t
            ON t.object_id = i.object_id
        JOIN sys.schemas s
            ON s.schema_id = t.schema_id
        WHERE t.name IN (N'Categories', N'AttributsCategorie', N'OptionsAttributCategorie')
          AND c.name IN (N'EstActive', N'Obligatoire', N'Filtrable')
          AND i.is_primary_key = 0
          AND i.is_unique_constraint = 0
          AND i.name IS NOT NULL
    )
    SELECT @sql +=
        N'DROP INDEX ' + QUOTENAME(IndexName) +
        N' ON ' + QUOTENAME(SchemaName) + N'.' + QUOTENAME(TableName) + N';' + CHAR(13)
    FROM IndexesToDrop;

    IF @sql <> N'' EXEC sp_executesql @sql;

    ------------------------------------------------------------
    -- Drop default constraints on removed category columns
    ------------------------------------------------------------
    SET @sql = N'';

    SELECT @sql +=
        N'ALTER TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) +
        N' DROP CONSTRAINT ' + QUOTENAME(dc.name) + N';' + CHAR(13)
    FROM sys.default_constraints dc
    JOIN sys.columns c
        ON c.default_object_id = dc.object_id
    JOIN sys.tables t
        ON t.object_id = c.object_id
    JOIN sys.schemas s
        ON s.schema_id = t.schema_id
    WHERE t.name IN (N'Categories', N'AttributsCategorie', N'OptionsAttributCategorie')
      AND c.name IN (N'EstActive', N'Obligatoire', N'Filtrable');

    IF @sql <> N'' EXEC sp_executesql @sql;

    ------------------------------------------------------------
    -- Drop check constraints mentioning removed category columns
    ------------------------------------------------------------
    SET @sql = N'';

    SELECT @sql +=
        N'ALTER TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) +
        N' DROP CONSTRAINT ' + QUOTENAME(cc.name) + N';' + CHAR(13)
    FROM sys.check_constraints cc
    JOIN sys.tables t
        ON t.object_id = cc.parent_object_id
    JOIN sys.schemas s
        ON s.schema_id = t.schema_id
    WHERE t.name IN (N'Categories', N'AttributsCategorie', N'OptionsAttributCategorie')
      AND (
            cc.definition LIKE N'%EstActive%'
         OR cc.definition LIKE N'%Obligatoire%'
         OR cc.definition LIKE N'%Filtrable%'
      );

    IF @sql <> N'' EXEC sp_executesql @sql;

    ------------------------------------------------------------
    -- Drop unnecessary category columns
    ------------------------------------------------------------
    IF COL_LENGTH('dbo.Categories', 'EstActive') IS NOT NULL
        ALTER TABLE dbo.Categories DROP COLUMN EstActive;

    IF COL_LENGTH('dbo.AttributsCategorie', 'Obligatoire') IS NOT NULL
        ALTER TABLE dbo.AttributsCategorie DROP COLUMN Obligatoire;

    IF COL_LENGTH('dbo.AttributsCategorie', 'Filtrable') IS NOT NULL
        ALTER TABLE dbo.AttributsCategorie DROP COLUMN Filtrable;

    IF COL_LENGTH('dbo.AttributsCategorie', 'EstActive') IS NOT NULL
        ALTER TABLE dbo.AttributsCategorie DROP COLUMN EstActive;

    IF COL_LENGTH('dbo.OptionsAttributCategorie', 'EstActive') IS NOT NULL
        ALTER TABLE dbo.OptionsAttributCategorie DROP COLUMN EstActive;

    ------------------------------------------------------------
    -- Reset identities
    ------------------------------------------------------------
    IF COLUMNPROPERTY(OBJECT_ID('dbo.OptionsAttributCategorie'), 'IdOptionAttributCategorie', 'IsIdentity') = 1
        DBCC CHECKIDENT ('dbo.OptionsAttributCategorie', RESEED, 0);

    IF COLUMNPROPERTY(OBJECT_ID('dbo.AttributsCategorie'), 'IdAttributCategorie', 'IsIdentity') = 1
        DBCC CHECKIDENT ('dbo.AttributsCategorie', RESEED, 0);

    IF COLUMNPROPERTY(OBJECT_ID('dbo.Categories'), 'IdCategorie', 'IsIdentity') = 1
        DBCC CHECKIDENT ('dbo.Categories', RESEED, 0);

    ------------------------------------------------------------
    -- Insert categories
    ------------------------------------------------------------
    INSERT INTO dbo.Categories (Nom, Description, IconKey, OrdreAffichage, DateCreation)
    SELECT Nom, Description, IconKey, OrdreAffichage, SYSUTCDATETIME()
    FROM #CategorySeed;

    ------------------------------------------------------------
    -- Insert attributes
    ------------------------------------------------------------
    DECLARE @InsertedAttributes TABLE
    (
        IdAttributCategorie INT NOT NULL,
        IdCategorie INT NOT NULL,
        Nom NVARCHAR(100) NOT NULL
    );

    INSERT INTO dbo.AttributsCategorie
    (
        IdCategorie,
        Nom,
        TypeDonnee,
        OrdreAffichage,
        Placeholder,
        EstPlage
    )
    OUTPUT inserted.IdAttributCategorie, inserted.IdCategorie, inserted.Nom
    INTO @InsertedAttributes (IdAttributCategorie, IdCategorie, Nom)
    SELECT
        c.IdCategorie,
        a.Nom,
        a.TypeDonnee,
        a.OrdreAffichage,
        a.Placeholder,
        a.EstPlage
    FROM #AttributeSeed a
    JOIN dbo.Categories c
        ON c.Nom = a.CategoryName;

    ------------------------------------------------------------
    -- Insert options
    ------------------------------------------------------------
    INSERT INTO dbo.OptionsAttributCategorie
    (
        IdAttributCategorie,
        Valeur,
        OrdreAffichage
    )
    SELECT
        ia.IdAttributCategorie,
        o.Valeur,
        o.OrdreAffichage
    FROM #OptionSeed o
    JOIN dbo.Categories c
        ON c.Nom = o.CategoryName
    JOIN @InsertedAttributes ia
        ON ia.IdCategorie = c.IdCategorie
       AND ia.Nom = o.AttributeName;

    ------------------------------------------------------------
    -- Final validation
    ------------------------------------------------------------
    IF EXISTS
    (
        SELECT 1
        FROM dbo.OptionsAttributCategorie o
        JOIN dbo.AttributsCategorie a
            ON a.IdAttributCategorie = o.IdAttributCategorie
        WHERE a.TypeDonnee <> 5
    )
    BEGIN
        THROW 51005, 'Final validation failed: Non-LISTE attribute has options.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.AttributsCategorie
        WHERE Nom LIKE N'%Transmission%'
    )
    BEGIN
        THROW 51012, 'Final validation failed: Transmission attribute exists. Use Boîte vitesse instead.', 1;
    END;

    IF COL_LENGTH('dbo.Categories', 'EstActive') IS NOT NULL
        THROW 51006, 'Final validation failed: Categories.EstActive still exists.', 1;

    IF COL_LENGTH('dbo.AttributsCategorie', 'Obligatoire') IS NOT NULL
        THROW 51007, 'Final validation failed: AttributsCategorie.Obligatoire still exists.', 1;

    IF COL_LENGTH('dbo.AttributsCategorie', 'Filtrable') IS NOT NULL
        THROW 51008, 'Final validation failed: AttributsCategorie.Filtrable still exists.', 1;

    IF COL_LENGTH('dbo.AttributsCategorie', 'EstActive') IS NOT NULL
        THROW 51009, 'Final validation failed: AttributsCategorie.EstActive still exists.', 1;

    IF COL_LENGTH('dbo.OptionsAttributCategorie', 'EstActive') IS NOT NULL
        THROW 51010, 'Final validation failed: OptionsAttributCategorie.EstActive still exists.', 1;

    COMMIT TRANSACTION;

    PRINT 'Predefined category system cleaned and seeded successfully.';

    SELECT 'Categories' AS TableName, COUNT(*) AS Total FROM dbo.Categories
    UNION ALL
    SELECT 'AttributsCategorie', COUNT(*) FROM dbo.AttributsCategorie
    UNION ALL
    SELECT 'OptionsAttributCategorie', COUNT(*) FROM dbo.OptionsAttributCategorie;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

    PRINT 'Predefined category seed failed.';
    PRINT ERROR_MESSAGE();

    THROW;
END CATCH;

DROP TABLE #OptionSeed;
DROP TABLE #AttributeSeed;
DROP TABLE #CategorySeed;
GO