USE BigDealsDb;
GO

-- Initial admin seed
-- Email: amari@gmail.com
-- Password: 123456789
-- Hash generated via PBKDF2 with 100,000 iterations.

IF NOT EXISTS (SELECT 1 FROM dbo.Utilisateurs WHERE Email = N'amari@gmail.com')
BEGIN
    INSERT INTO dbo.Utilisateurs
    (
        Nom,
        Prenom,
        Email,
        MotDePasseHash,
        Role,
        StatutCompte,
        DateCreation,
        EstActif
    )
    VALUES
    (
        N'amari',
        N'mahdi',
        N'amari@gmail.com',
        N'PBKDF2$100000$xz7LSmwzbI20+Pyb+lBebQ==$vintzW0EzLXlQdtJe7yJZbZHUUbLK1nRlh9S3kAF/lg=',
        3, -- ADMIN
        1, -- ACTIF
        SYSUTCDATETIME(),
        1
    );

    PRINT 'Initial admin user seeded successfully.';
END
ELSE
BEGIN
    PRINT 'Admin user amari@gmail.com already exists.';
END
GO
