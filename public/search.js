let API_BASE = "http://localhost:5049/api";
let categories = [];
let currentCategorySchema = [];

console.log("🚀 Search.js Loaded.");

async function init() {
    console.log("🎬 Initializing...");
    try {
        const testConn = await fetch(`${API_BASE}/categories`);
        if (!testConn.ok) throw new Error();
        console.log("✅ API Connected at " + API_BASE);
    } catch (e) {
        console.warn("⚠️ API fallback to 7096");
        API_BASE = "https://localhost:7096/api";
    }

    try {
        await loadCategories();
        await performSearch();
    } catch (e) {
        console.error("❌ Init Error:", e);
    }

    window.addEventListener('scroll', () => {
        const nav = document.querySelector('nav');
        if (nav && window.scrollY > 50) nav.classList.add('scrolled');
        else if (nav) nav.classList.remove('scrolled');
    });
}

async function loadCategories() {
    const response = await fetch(`${API_BASE}/categories`);
    const result = await response.json();
    categories = result.data || [];
    const select = document.getElementById('categorySelect');
    if (select) {
        select.innerHTML = '<option value="">Toutes les catégories</option>';
        categories.forEach(cat => {
            const opt = document.createElement('option');
            opt.value = cat.idCategorie;
            opt.textContent = cat.nom;
            select.appendChild(opt);
        });
    }
}

async function onCategoryChange() {
    const catId = document.getElementById('categorySelect').value;
    console.log("📂 Category Changed to:", catId);
    
    const container = document.getElementById('dynamicFilters');
    if (!container) return;
    
    container.innerHTML = "";
    currentCategorySchema = [];

    if (!catId) {
        console.log("ℹ️ No category selected, clearing filters.");
        return;
    }

    try {
        console.log(`📡 Fetching schema for cat ${catId}...`);
        const response = await fetch(`${API_BASE}/categories/${catId}/schema`);
        const result = await response.json();
        console.log("📥 Schema received:", result);

        // Case-insensitive property lookup
        const data = result.data || {};
        const attributesKey = Object.keys(data).find(k => k.toLowerCase() === 'attributs');
        const attributes = data[attributesKey] || [];
        
        console.log(`✅ Found ${attributes.length} attributes in schema.`);

        currentCategorySchema = attributes.filter(a => a.filtrable && (a.estActive || a.estActive === undefined));

        if (currentCategorySchema.length === 0) {
            console.log("ℹ️ No filtrable attributes for this category.");
            return;
        }

        currentCategorySchema.forEach(attr => {
            console.log(`✨ Generating UI for: ${attr.nom} (${attr.typeDonnee})`);
            const group = document.createElement('div');
            group.className = 'filter-group';
            
            // Generate unique IDs for the dynamic inputs
            const safeNom = attr.nom.replace(/[^a-z0-9]/gi, '_').toLowerCase();
            
            group.innerHTML = `<h3>🔹 ${attr.nom}</h3>`;

            const typeKey = Object.keys(attr).find(k => k.toLowerCase() === 'typedonnee');
            const typeValue = attr[typeKey];

            if (typeValue === "LISTE") {
                const select = document.createElement('select');
                select.className = 'form-control dynamic-input';
                select.dataset.attrId = attr.idAttributCategorie || attr.idAttributCategorie;
                select.dataset.type = "LISTE";
                select.innerHTML = `<option value="">Choisir ${attr.nom}...</option>`;
                
                const optionsKey = Object.keys(attr).find(k => k.toLowerCase() === 'options');
                const options = attr[optionsKey] || [];
                
                options.forEach(opt => {
                    select.innerHTML += `<option value="${opt.idOptionAttributCategorie}">${opt.valeur}</option>`;
                });
                group.appendChild(select);
            } 
            else if (typeValue === "NOMBRE") {
                const range = document.createElement('div');
                range.className = 'price-range';
                range.innerHTML = `
                    <input type="number" class="form-control dynamic-input" data-attr-id="${attr.idAttributCategorie}" data-type="NOMBRE_MIN" placeholder="Min">
                    <input type="number" class="form-control dynamic-input" data-attr-id="${attr.idAttributCategorie}" data-type="NOMBRE_MAX" placeholder="Max">
                `;
                group.appendChild(range);
            }
            else if (typeValue === "BOOLEAN") {
                const select = document.createElement('select');
                select.className = 'form-control dynamic-input';
                select.dataset.attrId = attr.idAttributCategorie;
                select.dataset.type = "BOOLEAN";
                select.innerHTML = `<option value="">Indifférent</option><option value="true">Oui</option><option value="false">Non</option>`;
                group.appendChild(select);
            }
            else {
                const input = document.createElement('input');
                input.className = 'form-control dynamic-input';
                input.dataset.attrId = attr.idAttributCategorie;
                input.dataset.type = "TEXTE";
                input.placeholder = `Rechercher ${attr.nom}...`;
                group.appendChild(input);
            }

            container.appendChild(group);
        });
    } catch (err) {
        console.error("❌ Failed to load schema:", err);
    }
}

async function performSearch(page = 1) {
    const grid = document.getElementById('annonceGrid');
    const countLabel = document.getElementById('resultsCount');
    if (grid) grid.innerHTML = '<div class="empty-state"><h3>Chargement...</h3></div>';

    const dynamicFilters = [];
    const inputs = document.querySelectorAll('.dynamic-input');
    const grouped = {};

    inputs.forEach(input => {
        const id = input.dataset.attrId;
        if (!grouped[id]) grouped[id] = { idAttributCategorie: parseInt(id) };
        const val = input.value;
        if (!val) return;

        switch(input.dataset.type) {
            case "LISTE": grouped[id].idOptionAttributCategorie = parseInt(val); break;
            case "TEXTE": grouped[id].valeurTexte = val; break;
            case "BOOLEAN": grouped[id].valeurBooleen = val === "true"; break;
            case "NOMBRE_MIN": grouped[id].valeurNombreMin = parseFloat(val); break;
            case "NOMBRE_MAX": grouped[id].valeurNombreMax = parseFloat(val); break;
        }
    });

    Object.values(grouped).forEach(f => {
        if (f.idOptionAttributCategorie || f.valeurTexte || f.valeurBooleen !== undefined || f.valeurNombreMin || f.valeurNombreMax) {
            dynamicFilters.push(f);
        }
    });

    const requestBody = {
        keyword: document.getElementById('keywordInput')?.value || null,
        idCategorie: document.getElementById('categorySelect')?.value ? parseInt(document.getElementById('categorySelect').value) : null,
        prixMin: document.getElementById('minPrice')?.value ? parseFloat(document.getElementById('minPrice').value) : null,
        prixMax: document.getElementById('maxPrice')?.value ? parseFloat(document.getElementById('maxPrice').value) : null,
        localisation: document.getElementById('locationInput')?.value || null,
        filtresDynamiques: dynamicFilters,
        sortBy: document.getElementById('sortSelect')?.value.split(':')[0] || "newest",
        sortDirection: document.getElementById('sortSelect')?.value.split(':')[1] || "desc",
        pageNumber: page,
        pageSize: 12
    };

    try {
        const response = await fetch(`${API_BASE}/annonces/search`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestBody)
        });
        const result = await response.json();
        const { items, totalCount } = result.data;

        if (countLabel) countLabel.textContent = `${totalCount} annonce${totalCount > 1 ? 's' : ''} trouvée${totalCount > 1 ? 's' : ''}`;

        if (!items || items.length === 0) {
            grid.innerHTML = '<div class="empty-state"><h3>Aucune annonce trouvée</h3></div>';
            return;
        }

        grid.innerHTML = items.map(item => `
            <div class="annonce-card">
                <div class="annonce-badge">${item.categorieNom}</div>
                <img src="${item.mainImageUrl ? (item.mainImageUrl.startsWith('http') ? item.mainImageUrl : 'http://localhost:5049/' + item.mainImageUrl) : 'https://via.placeholder.com/400x300?text=Pas+d-image'}" class="annonce-image">
                <div class="annonce-content">
                    <div class="annonce-price">${formatCurrency(item.prix)}</div>
                    <h3 class="annonce-title">${item.titre}</h3>
                    <div class="annonce-meta">
                        <span>📍 ${item.localisation || 'Tunisie'}</span>
                        <span>📅 ${new Date(item.dateCreation).toLocaleDateString()}</span>
                    </div>
                </div>
            </div>
        `).join('');
    } catch (err) {
        console.error("Search failed", err);
    }
}

function formatCurrency(val) {
    return new Intl.NumberFormat('fr-TN', { style: 'currency', currency: 'TND' }).format(val);
}

window.performSearch = performSearch;
window.onCategoryChange = onCategoryChange;
document.addEventListener('DOMContentLoaded', init);
