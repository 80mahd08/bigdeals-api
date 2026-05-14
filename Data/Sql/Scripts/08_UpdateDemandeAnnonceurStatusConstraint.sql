USE BigDealsDb;
GO

-- 1. Drop old constraint if it exists
IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CHK_DemandesAnnonceur_Statut' AND type = 'C')
BEGIN
    EXEC('ALTER TABLE dbo.DemandesAnnonceur DROP CONSTRAINT CHK_DemandesAnnonceur_Statut');
END
GO

-- 2. Add new constraint allowing Statut IN (1, 2, 3, 4)
-- 1 = EN_ATTENTE_VERIFICATION
-- 2 = APPROUVEE
-- 3 = REJETEE
-- 4 = EN_ATTENTE_PAIEMENT
ALTER TABLE dbo.DemandesAnnonceur ADD CONSTRAINT CHK_DemandesAnnonceur_Statut CHECK (Statut IN (1, 2, 3, 4));
GO

-- 3. Update the unique index for pending requests to also exclude those waiting for payment if needed
-- However, the business rule says "Each demande annonceur can have only one annonceur fee payment record"
-- and "User uploads document -> status becomes EN_ATTENTE_VERIFICATION".
-- If we want to allow ONLY ONE active request per user (either in verification or in payment), we update the index.
-- Existing index name: UQ_DemandesAnnonceur_EnAttente_PerUser

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_DemandesAnnonceur_EnAttente_PerUser' AND object_id = OBJECT_ID('dbo.DemandesAnnonceur'))
BEGIN
    DROP INDEX UQ_DemandesAnnonceur_EnAttente_PerUser ON dbo.DemandesAnnonceur;
END
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_DemandesAnnonceur_EnAttente_PerUser
ON dbo.DemandesAnnonceur(IdUtilisateur)
WHERE Statut IN (1, 4);
GO
