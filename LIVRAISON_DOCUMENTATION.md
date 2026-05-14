# Documentation du Système de Livraison (Livraison Lifecycle)

Ce document explique le fonctionnement du cycle de vie de la livraison des produits sur la plateforme BigDeals.

## 1. Concept de base
Le système sépare strictement le **Statut de la Commande (Paiement)** du **Statut de la Livraison**.
- **StatutCommande** : Répond à "La commande est-elle payée ?" (Géré lors du checkout).
- **StatutLivraison** : Répond à "Où en est le colis ?" (Géré par l'annonceur ou l'admin après paiement).

---

## 2. États de Livraison (StatutLivraison)

Le système utilise les identifiants entiers suivants (Enum `StatutLivraison`) :

| ID | Label Français | Description |
|---|---|---|
| **1** | En attente de préparation | État initial après paiement réussi. |
| **2** | En préparation | L'annonceur prépare le colis. |
| **3** | Expédiée | Le colis a été remis au transporteur (`DateExpedition` fixée). |
| **4** | Livrée | Le client a reçu le colis (`DateLivraison` fixée). |
| **5** | Échec de livraison | Le transporteur n'a pas pu livrer. |
| **6** | Retournée | Le colis est revenu chez l'annonceur. |
| **7** | Annulée | Livraison annulée par l'annonceur ou le client. |

---

## 3. Machine à États (Transitions Valides)

Pour garantir l'intégrité des données, le backend (`OrdersService.cs`) impose des règles strictes sur les changements de statut. **Une transition non listée ici sera rejetée par l'API.**

- **En attente de préparation** (1) → `En préparation` (2) ou `Annulée` (7)
- **En préparation** (2) → `Expédiée` (3) ou `Annulée` (7)
- **Expédiée** (3) → `Livrée` (4) ou `Échec de livraison` (5)
- **Échec de livraison** (5) → `Retournée` (6) ou `Expédiée` (3) (nouvelle tentative)

> [!IMPORTANT]
> - Une livraison ne peut être mise à jour **QUE SI** `StatutCommande = 2` (PAYEE).
> - Une commande ne peut être **Annulée** (Statut 7) que si elle n'a pas encore été **Expédiée** (Statut 3).
> - **Le Client** peut annuler sa propre commande tant qu'elle est en statut 1 ou 2.

---

## 4. Implémentation Technique

### Backend
- **Repository** : `OrdersRepository.cs` gère la lecture/écriture des colonnes `StatutLivraison`, `AdresseLivraison`, `VilleLivraison`, `TelephoneLivraison`, `DateExpedition`, `DateLivraison`.
- **Service** : `OrdersService.cs` contient la logique de validation des transitions et la mise à jour automatique des dates.
- **Controller** : 
    - `PATCH /api/orders/client/{id}/cancel` (Client)
    - `PATCH /api/orders/announcer/{id}/delivery-status` (Annonceur)
    - `PATCH /api/orders/admin/{id}/delivery-status` (Admin)

### Frontend
- **Acheteur (`OrdersComponent`)** : Affiche une timeline visuelle et un bouton "Annuler" si la commande n'est pas encore expédiée.
- **Annonceur (`AnnouncerOrdersComponent`)** : Possède un bouton "Mettre à jour" qui affiche uniquement les transitions valides.
- **Admin (`AdminOrdersComponent`)** : Permet de filtrer par état de livraison et de forcer une mise à jour.

---

## 5. Comment tester / Debugger

### Base de données (SQL)
Pour vérifier l'état d'une commande en SQL :
```sql
SELECT IdCommande, StatutCommande, StatutLivraison, DateExpedition, DateLivraison 
FROM Commandes 
WHERE IdCommande = [VOTRE_ID];
```

### API (Postman)
**Mettre à jour le statut (Annonceur) :**
- **URL** : `PATCH /api/orders/announcer/{id}/delivery-status`
- **Body** :
```json
{
  "statutLivraison": 2
}
```

### Points de vigilance (Debugging)
1. **Paiement requis** : Si vous testez une commande en statut "1" (En attente de paiement), le bouton de livraison ne s'affichera pas.
2. **Transition bloquée** : Si l'API renvoie une erreur "Transition invalide", vérifiez le code source de `OrdersService.cs` dans le dictionnaire `_validTransitions`.
3. **Casing (JSON)** : Les propriétés sont mappées en camelCase (`statutLivraison`). Le frontend supporte aussi le PascalCase pour plus de robustesse.
