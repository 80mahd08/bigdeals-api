# 🗄️ BigDeals Database Configuration

This directory contains the database setup and configuration files for pair-programming and collaboration.

## 🚀 Getting Started

To get your local development database up and running quickly:

1. **Open SQL Server Management Studio (SSMS)** or connect via your preferred CLI tool.
2. Open and run [setup_all.sql](file:///e:/my%20work/api/Data/Sql/setup_all.sql).
   * This unified script will:
     * Create the `BigDealsDb` database.
     * Build all tables in the correct dependency order.
     * Establish all check, primary, foreign key, and unique constraints.
     * Set up all performance-optimizing indexes.
     * Seed a default administrator account (`amari@gmail.com` with password `123456789`).
     * Seed all predefined marketplace categories, dynamic attributes, and options.

## 🗑️ Resetting / Teardown

If you want to clear all platform data and drop all tables for a clean slate, open and run [teardown.sql](file:///e:/my%20work/api/Data/Sql/teardown.sql).

---

## 📋 Platform Schema Overview

The database is built on **SQL Server** and consists of the following key layers:

### 1. Identity & Verification Flow
* **`Utilisateurs`**: Storage for Clients, Announcers, and Admins.
* **`DemandesAnnonceur`**: Verification requests submitted by users to upgrade to Announcer status.
* **`PaiementsAnnonceur`**: Payment logs for upgrading to advertiser accounts (via Mock, Flouci, Sobflous, Konnect).
* **`PasswordResetTokens`**: Hashes for secure password recovery.

### 2. Marketplace Core
* **`Categories`**: Premium platform categories (e.g. Véhicules, Téléphones, Mode).
* **`AttributsCategorie`**: Dynamic attributes representing specific categories (e.g. "Marque", "Surface", "RAM").
* **`OptionsAttributCategorie`**: Selectable options for list-type attributes.
* **`Annonces`**: Product listings posted by users.
* **`ValeursAttributAnnonce`**: Concrete values mapped dynamically to an ad's attributes.
* **`ImagesAnnonce`**: Image attachment galleries for listings.

### 3. Interactions & Feedback
* **`Favoris`**: Users' bookmarked listings.
* **`AbonnementsAnnonceur`**: Subscriptions to specific announcers.
* **`ContactsAnnonceur`**: Direct WhatsApp and Telephone contact analytics.
* **`Avis`**: Star ratings (1-5) and user commentary on listings.
* **`Signalements`**: User reports flagging suspicious listings for admin review.

### 4. Product Checkout Flow
* **`Commandes`**: Order requests with full delivery lifecycle details (`MontantAnnonce`, `FraisLivraison`, `StatutLivraison`).
* **`PaiementsCommandes`**: Transaction logs validating order payment states.

---

### 🔑 Seed Admin Credentials
* **Email:** `admin@admin.com`
* **Password:** `123456789`
