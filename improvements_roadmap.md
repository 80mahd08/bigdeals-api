# BigDeals: Technical & UI/UX Improvement Roadmap

This document outlines strategic suggestions to enhance the scalability, maintainability, and user experience of the BigDeals marketplace.

---

## 🚀 Backend & Architecture Improvements

### 1. Data Access Layer Optimization
*   **Transition to Dapper**: While raw SQL is performant, using [Dapper](https://github.com/DapperLib/Dapper) would significantly reduce boilerplate code in repositories while maintaining the "Raw SQL" performance and control you prefer.
*   **Repository Generic Interface**: Implement a `IBaseRepository<T>` to standardize common CRUD operations (GetById, Delete, etc.) and reduce code duplication across the 15+ repositories.

### 2. Security & Identity
*   **Refresh Token Implementation**: Currently, users must log in again when their JWT expires. Implementing a Refresh Token flow (stored in a secure HttpOnly cookie) would provide a seamless session experience.
*   **Rate Limiting**: Add middleware (like `AspNetCoreRateLimit`) to protect public endpoints (like Login, Register, and Search) from brute-force and DDoS attacks.

### 3. API Consistency
*   **Fluent Validation**: Use [FluentValidation](https://docs.fluentvalidation.net/) instead of manual checks in Services. This separates validation logic from business logic and automatically integrates with ASP.NET Core's ModelState.
*   **AutoMapper**: Simplify the mapping between Domain Models and DTOs. Manual mapping is prone to errors as the schema grows.

---

## 🎨 UI/UX Design Enhancements

### 1. Dashboard Interactivity
*   **Real-time Notifications**: Use **SignalR** to push notifications to the Announcer Dashboard when a new review is received or an ad is approved.
*   **Review Replies**: Allow advertisers to reply to customer reviews. This builds trust and improves the "social" aspect of the marketplace.

### 2. Search & Discovery
*   **Advanced Filtering Sidebar**: Replace simple dropdowns with a faceted search (e.g., checkboxes for categories, price sliders, and "Clear All" buttons).
*   **Search Autocomplete**: Implement a debounced search input that suggests categories or ad titles as the user types.

### 3. Mobile Experience
*   **PWA Support**: Convert the Angular app into a Progressive Web App (PWA). This allows users to "install" BigDeals on their home screen and enables offline caching for faster load times on mobile networks.
*   **Skeleton Screens**: Replace the circular loading spinners with "Skeleton" placeholders that mimic the layout of the content being loaded. This feels much faster to the user.

---

## 🛠️ Performance & Scalability

### 1. Frontend Performance
*   **Angular Standalone Components**: Migrate existing modules to Standalone Components to reduce bundle size and simplify the dependency tree.
*   **Image Optimization**: Implement an image proxy (or use a library like `ngx-image-compress`) to serve WebP versions of uploaded photos, drastically reducing page weight.

### 2. Database Efficiency
*   **Full-Text Search**: Instead of `LIKE '%query%'`, implement SQL Server Full-Text Indexing on the `Titre` and `Description` columns of the `Annonces` table for significantly faster search results.
*   **Read-Only Projections**: For heavy pages like the home page, create SQL Views or use non-tracking queries to avoid overhead.

---

## 📈 Future Features to Consider
*   **Verified Badge**: A "Verified Seller" system based on identity document checks (already partially implemented).
*   **Internal Messaging**: A real-time chat between buyers and sellers to replace the current "Contact" form.
*   **Ad Boosting**: A payment integration (Stripe/Flouci) to allow sellers to pay for "Premium" placement of their ads.
