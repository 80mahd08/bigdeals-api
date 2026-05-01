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
