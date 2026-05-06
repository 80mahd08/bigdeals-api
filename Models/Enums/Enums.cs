namespace api.Models.Enums;

public enum RoleUtilisateur
{
    CLIENT = 1,
    ANNONCEUR = 2,
    ADMIN = 3
}

public enum StatutCompte
{
    ACTIF = 1,
    INACTIF = 2,
    EN_ATTENTE = 3,
    BLOQUE = 4
}

public enum StatutDemandeAnnonceur
{
    EN_ATTENTE = 1,
    APPROUVEE = 2,
    REJETEE = 3
}

public enum TypeDonneeAttribut
{
    TEXTE = 1,
    NOMBRE = 2,
    DATE = 3,
    BOOLEAN = 4,
    LISTE = 5
}

public enum StatutAnnonce
{
    PUBLIEE = 1,
    SUSPENDUE = 2
}

public enum TypeContact
{
    TELEPHONE = 1,
    WHATSAPP = 2
}
