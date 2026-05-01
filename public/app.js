// Configuration
const API_BASE_URL = 'http://localhost:5049/api';

// Initialize Lucide Icons
lucide.createIcons();

// State
let selectedFiles = [];
let categorySchema = [];

// DOM Elements
const form = document.getElementById('annonce-form');
const categorySelect = document.getElementById('IdCategorie');
const dynamicSection = document.getElementById('dynamic-section');
const dynamicAttributes = document.getElementById('dynamic-attributes');
const uploadZone = document.getElementById('upload-zone');
const fileInput = document.getElementById('Images');
const previewGrid = document.getElementById('preview-grid');
const submitBtn = document.getElementById('submit-btn');

// --- Initialization ---

document.addEventListener('DOMContentLoaded', () => {
    fetchCategories();
    setupImageUpload();
});

// --- API Calls ---

async function fetchCategories() {
    try {
        const response = await fetch(`${API_BASE_URL}/categories`);
        const result = await response.json();
        if (result.success) {
            result.data.forEach(cat => {
                const option = document.createElement('option');
                option.value = cat.idCategorie;
                option.textContent = cat.nom;
                categorySelect.appendChild(option);
            });
        }
    } catch (err) {
        showToast('Error loading categories', 'error');
    }
}

categorySelect.addEventListener('change', async (e) => {
    const catId = e.target.value;
    if (!catId) {
        dynamicSection.style.display = 'none';
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/categories/${catId}/attributes`);
        const result = await response.json();
        if (result.success) {
            categorySchema = result.data;
            renderDynamicFields(result.data);
            dynamicSection.style.display = 'block';
        }
    } catch (err) {
        showToast('Error loading category attributes', 'error');
    }
});

// --- Dynamic Rendering ---

function renderDynamicFields(attributes) {
    dynamicAttributes.innerHTML = '';
    
    attributes.forEach(attr => {
        const group = document.createElement('div');
        group.className = 'form-group';
        
        const label = document.createElement('label');
        label.textContent = attr.nom + (attr.obligatoire ? ' *' : '');
        group.appendChild(label);

        let input;
        
        const type = attr.typeDonnee.toUpperCase();

        if (type === 'LISTE' && attr.options) {
            input = document.createElement('select');
            input.className = 'form-control';
            input.dataset.attrId = attr.idAttributCategorie;
            input.dataset.type = 'option';
            
            const defaultOpt = document.createElement('option');
            defaultOpt.value = '';
            defaultOpt.textContent = `Select ${attr.nom}...`;
            input.appendChild(defaultOpt);

            attr.options.forEach(opt => {
                const o = document.createElement('option');
                o.value = opt.idOptionAttributCategorie;
                o.textContent = opt.valeur;
                input.appendChild(o);
            });
        } else if (type === 'NOMBRE') {
            input = document.createElement('input');
            input.type = 'number';
            input.className = 'form-control';
            input.dataset.attrId = attr.idAttributCategorie;
            input.dataset.type = 'number';
            input.placeholder = `Enter ${attr.nom}...`;
        } else {
            input = document.createElement('input');
            input.type = 'text';
            input.className = 'form-control';
            input.dataset.attrId = attr.idAttributCategorie;
            input.dataset.type = 'text';
            input.placeholder = `Enter ${attr.nom}...`;
        }

        if (attr.obligatoire) input.required = true;
        group.appendChild(input);
        dynamicAttributes.appendChild(group);
    });
}

// --- Image Handling ---

function setupImageUpload() {
    uploadZone.addEventListener('click', () => fileInput.click());
    
    uploadZone.addEventListener('dragover', (e) => {
        e.preventDefault();
        uploadZone.classList.add('dragover');
    });

    uploadZone.addEventListener('dragleave', () => {
        uploadZone.classList.remove('dragover');
    });

    uploadZone.addEventListener('drop', (e) => {
        e.preventDefault();
        uploadZone.classList.remove('dragover');
        handleFiles(e.dataTransfer.files);
    });

    fileInput.addEventListener('change', (e) => {
        handleFiles(e.target.files);
    });
}

function handleFiles(files) {
    const newFiles = Array.from(files);
    if (selectedFiles.length + newFiles.length > 8) {
        showToast('Maximum 8 images allowed', 'error');
        return;
    }

    newFiles.forEach(file => {
        if (!file.type.startsWith('image/')) return;
        selectedFiles.push(file);
        
        const reader = new FileReader();
        reader.onload = (e) => {
            const div = document.createElement('div');
            div.className = 'preview-item';
            div.innerHTML = `
                <img src="${e.target.result}">
                <button type="button" class="remove-img" onclick="removeImage(${selectedFiles.length - 1})">×</button>
            `;
            previewGrid.appendChild(div);
        };
        reader.readAsDataURL(file);
    });
}

window.removeImage = (index) => {
    selectedFiles.splice(index, 1);
    renderPreviews();
};

function renderPreviews() {
    previewGrid.innerHTML = '';
    selectedFiles.forEach((file, index) => {
        const reader = new FileReader();
        reader.onload = (e) => {
            const div = document.createElement('div');
            div.className = 'preview-item';
            div.innerHTML = `
                <img src="${e.target.result}">
                <button type="button" class="remove-img" onclick="removeImage(${index})">×</button>
            `;
            previewGrid.appendChild(div);
        };
        reader.readAsDataURL(file);
    });
}

// --- Form Submission ---

form.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    if (selectedFiles.length === 0) {
        showToast('Please upload at least one image', 'error');
        return;
    }

    submitBtn.disabled = true;
    submitBtn.textContent = 'Publishing...';

    const formData = new FormData();
    formData.append('IdCategorie', document.getElementById('IdCategorie').value);
    formData.append('Titre', document.getElementById('Titre').value);
    formData.append('Description', document.getElementById('Description').value);
    formData.append('Prix', document.getElementById('Prix').value);
    formData.append('Localisation', document.getElementById('Localisation').value);

    // Dynamic Attributes Formatting
    const values = [];
    const dynInputs = dynamicAttributes.querySelectorAll('.form-control');
    dynInputs.forEach(input => {
        if (!input.value) return;

        const obj = { idAttributCategorie: parseInt(input.dataset.attrId) };
        const type = input.dataset.type;

        if (type === 'option') {
            obj.idOptionAttributCategorie = parseInt(input.value);
        } else if (type === 'number') {
            obj.valeurNombre = parseFloat(input.value);
        } else {
            obj.valeurTexte = input.value;
        }
        
        // Use the fix we implemented: stringified JSON for ValeursJson
        formData.append('ValeursJson', JSON.stringify(obj));
    });

    // Images
    selectedFiles.forEach(file => {
        formData.append('Images', file);
    });

    try {
        const tokenInput = document.getElementById('jwt-token').value.trim();
        if (!tokenInput) {
            showToast('Please provide an authentication token', 'error');
            submitBtn.disabled = false;
            submitBtn.textContent = 'Publish Announcement';
            return;
        }

        const token = tokenInput.startsWith('Bearer ') ? tokenInput : `Bearer ${tokenInput}`;

        const response = await fetch(`${API_BASE_URL}/annonces`, {
            method: 'POST',
            body: formData,
            headers: {
                'Authorization': token
            }
        });

        const result = await response.json();
        if (result.success) {
            showToast('Announcement published successfully!', 'success');
            setTimeout(() => window.location.reload(), 2000);
        } else {
            showToast(result.message || 'Submission failed', 'error');
        }
    } catch (err) {
        showToast('Network error, please try again', 'error');
    } finally {
        submitBtn.disabled = false;
        submitBtn.textContent = 'Publish Announcement';
    }
});

// --- Helpers ---

function showToast(msg, type = 'success') {
    const toast = document.getElementById('toast');
    const toastMsg = document.getElementById('toast-msg');
    
    toast.style.display = 'block';
    toastMsg.textContent = msg;
    toast.style.borderColor = type === 'success' ? '#10b981' : '#ef4444';
    
    setTimeout(() => {
        toast.style.transform = 'translateY(0)';
        setTimeout(() => {
            toast.style.transform = 'translateY(100px)';
            setTimeout(() => toast.style.display = 'none', 400);
        }, 3000);
    }, 10);
}
