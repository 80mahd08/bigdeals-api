USE BigDealsDb;
GO

-- Admin seed strategy (Option A)
-- Email: amari@gmail.com
-- Password: '123456789'
-- Hash generated via PBKDF2 with 100,000 iterations and a 16-byte salt, resulting in a 32-byte hash.

IF NOT EXISTS (SELECT 1 FROM Utilisateurs WHERE Email = 'amari@gmail.com')
BEGIN
    INSERT INTO Utilisateurs (
        Nom, Prenom, Email, MotDePasseHash, 
        Role, StatutCompte, DateCreation, EstActif
    )
    VALUES (
        'amari', 'mahdi', 'amari@gmail.com', 'PBKDF2$100000$xz7LSmwzbI20+Pyb+lBebQ==$vintzW0EzLXlQdtJe7yJZbZHUUbLK1nRlh9S3kAF/lg=',
        3, -- 3 = ADMIN
        1, -- 1 = ACTIF
        GETUTCDATE(),
        1
    );
    
    PRINT 'Initial admin user seeded successfully.';
END
ELSE
BEGIN
    PRINT 'Admin user amari@gmail.com already exists.';
END
GO
