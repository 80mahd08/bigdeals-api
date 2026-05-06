# Project Status Checklist Report

## 1. Backend Completed Modules
- **Auth Module**: Registration, Login, JWT issuing.
- **Password Reset**: Forgot password (email trigger) and Reset password (token-based).
- **Users Module**: Profile management (Me), Profile photo upload, Password change.
- **Categories Module**: List categories, get category schema (dynamic attributes).
- **Annonces Module**: Public listing, CRUD for announcers, Admin view.
- **Search Engine**: Dynamic filtering by keyword, category, price range, and custom attributes.
- **Interactions Foundation**:
    - **Favorites**: Add/Remove/List favorite ads.
    - **Follows**: Follow/Unfollow advertisers, list following.
    - **Contacts**: Track contact clicks (WhatsApp/Phone).
- **DemandeAnnonceur**: Submit and view advertiser status requests with document upload.

## 2. Backend Missing Modules
- **Cart**: No endpoints for cart persistence.
- **Orders**: No endpoints for order processing.
- **Payments**: No payment gateway integration.
- **Reviews**: No endpoints for submitting or listing reviews (only defined in frontend).
- **Reports**: No endpoints for reporting ads (only defined in frontend).

## 3. Frontend Completed Integrations
- **Environment**: Configured with `apiUrl` (http://localhost:5049/api) and `imageBaseUrl` (http://localhost:5049).
- **Authentication**:
    - Login/Register using **FormData**.
    - Forgot/Reset using **JSON**.
    - JWT Interceptor using **Bearer Token**.
    - Role naming normalized to **ANNONCEUR**.
- **Marketplace Pages**: Landing page, Ads listing with sidebar filters, Ad detail page.
- **Announcer Dashboard**: My ads listing, Ad creation/edit form (FormData).
- **Core Services**: `AuthenticationService`, `AnnoncesService`, `CategoriesService`, `FavoritesService`.
- **UI Utils**: `ImageUrlPipe` for dynamic image resolution.

## 4. Frontend Partial Integrations
- **Dynamic Filters**: Implemented in service but requires UI validation with real backend schema data.
- **Followers/Contacts**: UI exists but service endpoints have mismatches with current backend routes.

## 5. Frontend Missing Integrations
- **Cart/Checkout**: UI components exist but no backend integration.
- **Orders/Invoices**: UI exists but no backend integration.
- **Reviews/Reports**: UI exists but backend endpoints are missing.

## 6. Bugs Found
- **Route Mismatch (Follow)**: Frontend `ClientActionsService` calls `POST /interactions/follow/{id}`, Backend expects `POST /Annonceurs/{id}/follow`.
- **Route Mismatch (Contact)**: Frontend `ClientActionsService` calls `POST /interactions/contact`, Backend expects `POST /contacts-annonceur`.
- **Missing Route**: `AnnoncesService.getAnnoncesByUtilisateur` calls `annonces/user/{id}` which is not implemented in `AnnoncesController`.
- **Incomplete Logic**: Reviews and Reports services in frontend call `/interactions/review` and `/interactions/report`, but these routes/controllers do not exist on the backend.

## 7. Exact Next Step Recommendation
**Standardize Interactions API**:
1. Create a unified `InteractionsController` on the backend to handle Follows, Contacts, Reviews, and Reports to match the frontend `ClientActionsService` expectations.
2. Update `AnnoncesController` to include the `GetByUserId` endpoint.
3. Fix the route mismatches in `ClientActionsService` to point to the new unified routes.

## 8. Build Results
- **Backend (dotnet build)**: SUCCESS (0 Errors, 0 Warnings)
- **Frontend (npm run build)**: SUCCESS
