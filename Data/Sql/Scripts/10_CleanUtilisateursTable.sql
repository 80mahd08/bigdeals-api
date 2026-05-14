USE BigDealsDb;
GO

--------------------------------------------------------------------------------
-- 10_CleanUtilisateursTable.sql
-- Goal: Cleanup Utilisateurs table by merging status flags into StatutCompte
-- and removing deprecated columns.
--------------------------------------------------------------------------------

PRINT 'Starting cleanup of Utilisateurs table...';

-- 1. Migration of data to new StatutCompte logic
-- Logic: 1=ACTIF, 2=BLOQUE
-- If EstActif was 0 OR old StatutCompte was 4 (BLOQUE) or 2 (INACTIF/BLOQUE in old enum) 
-- we map them to 2 (BLOQUE).
-- Everything else becomes 1 (ACTIF).

PRINT 'Migrating user status data...';

-- Update to 2 (BLOQUE) if they were inactive or blocked
IF COL_LENGTH('dbo.Utilisateurs', 'EstActif') IS NOT NULL
BEGIN
    UPDATE dbo.Utilisateurs 
    SET StatutCompte = 2 
    WHERE EstActif = 0 OR StatutCompte IN (2, 4);

    -- Update the rest to 1 (ACTIF)
    UPDATE dbo.Utilisateurs 
    SET StatutCompte = 1 
    WHERE StatutCompte NOT IN (2);
END
ELSE
BEGIN
    -- If EstActif is already gone, just ensure StatutCompte is valid
    UPDATE dbo.Utilisateurs SET StatutCompte = 2 WHERE StatutCompte IN (2, 4);
    UPDATE dbo.Utilisateurs SET StatutCompte = 1 WHERE StatutCompte NOT IN (2);
END
GO

-- 2. Handle constraints
PRINT 'Updating constraints...';

IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_Utilisateurs_StatutCompte' AND type = 'C')
BEGIN
    ALTER TABLE dbo.Utilisateurs DROP CONSTRAINT CHK_Utilisateurs_StatutCompte;
END
GO

ALTER TABLE dbo.Utilisateurs ADD CONSTRAINT CHK_Utilisateurs_StatutCompte CHECK (StatutCompte IN (1, 2));
GO

-- 3. Drop deprecated columns
PRINT 'Dropping deprecated columns...';

-- Drop default constraints first
DECLARE @ConstraintName nvarchar(200)

-- EstActif default
SELECT @ConstraintName = Name FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID('dbo.Utilisateurs') 
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE name = 'EstActif' AND object_id = OBJECT_ID('dbo.Utilisateurs'))
IF @ConstraintName IS NOT NULL EXEC('ALTER TABLE dbo.Utilisateurs DROP CONSTRAINT ' + @ConstraintName)

-- EstVerifie default
SET @ConstraintName = NULL
SELECT @ConstraintName = Name FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID('dbo.Utilisateurs') 
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE name = 'EstVerifie' AND object_id = OBJECT_ID('dbo.Utilisateurs'))
IF @ConstraintName IS NOT NULL EXEC('ALTER TABLE dbo.Utilisateurs DROP CONSTRAINT ' + @ConstraintName)

IF COL_LENGTH('dbo.Utilisateurs', 'EstActif') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Utilisateurs DROP COLUMN EstActif;
END
GO

IF COL_LENGTH('dbo.Utilisateurs', 'EstVerifie') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Utilisateurs DROP COLUMN EstVerifie;
END
GO

IF COL_LENGTH('dbo.Utilisateurs', 'DerniereConnexion') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Utilisateurs DROP COLUMN DerniereConnexion;
END
GO

PRINT 'Ensuring RefreshToken columns exist...';

IF COL_LENGTH('dbo.Utilisateurs', 'RefreshToken') IS NULL
BEGIN
    ALTER TABLE dbo.Utilisateurs ADD RefreshToken NVARCHAR(500) NULL;
END
GO

IF COL_LENGTH('dbo.Utilisateurs', 'RefreshTokenExpiry') IS NULL
BEGIN
    ALTER TABLE dbo.Utilisateurs ADD RefreshTokenExpiry DATETIME2 NULL;
END
GO

PRINT 'Cleanup complete.';
GO
