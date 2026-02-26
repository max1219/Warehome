// -----------------------------
// ГЛОБАЛЬНОЕ СОСТОЯНИЕ
// -----------------------------
let mode = "storages"; // "storages" | "items"

let storageRoot = null;
let itemRoot = null;

let path = []; // путь внутри активного дерева

// -----------------------------
// УТИЛИТЫ
// -----------------------------
function getActiveRoot() {
    return mode === "storages" ? storageRoot : itemRoot;
}

function getCurrentCategory() {
    let node = getActiveRoot();
    for (let name of path) {
        node = node.children.find(c => c.name === name);
    }
    return node;
}

function buildPathString() {
    return path.length === 0 ? null : path.join("/");
}

function buildChildPath(name) {
    return path.length === 0 ? name : path.join("/") + "/" + name;
}

function showError(msg) {
    alert(msg);
}

// -----------------------------
// ЗАГРУЗКА ДЕРЕВЬЕВ
// -----------------------------
async function loadStorageTree() {
    const res = await fetch("/api/storage-categories/tree");
    if (!res.ok) return showError("Ошибка загрузки дерева складов");
    storageRoot = await res.json();
}

async function loadItemTree() {
    const res = await fetch("/api/item-type-categories/tree");
    if (!res.ok) return showError("Ошибка загрузки дерева предметов");
    itemRoot = await res.json();
}

async function reloadActiveTree() {
    if (mode === "storages") await loadStorageTree();
    else await loadItemTree();
    render();
}

// -----------------------------
// CRUD ДЛЯ КАТЕГОРИЙ
// -----------------------------
async function createCategory() {
    const name = prompt("Введите имя подкатегории:");
    if (!name) return;

    const parentPath = buildPathString();

    const url = mode === "storages"
        ? "/api/storage-categories"
        : "/api/item-type-categories";

    const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, parentPath })
    });

    if (res.ok) return reloadActiveTree();

    if (res.status === 404) showError("Родительская категория не найдена");
    else if (res.status === 409) showError("Категория уже существует");
    else showError("Ошибка создания категории");
}

async function deleteCategory(fullPath) {
    const url = mode === "storages"
        ? "/api/storage-categories"
        : "/api/item-type-categories";

    const body = mode === "storages"
        ? { path: fullPath }
        : { path: fullPath };

    const res = await fetch(url, {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
    });

    if (res.ok) return reloadActiveTree();

    if (res.status === 404) showError("Категория не найдена");
    else if (res.status === 409) showError("Категория не пуста");
    else showError("Ошибка удаления категории");
}

// -----------------------------
// CRUD ДЛЯ ЛИСТЬЕВ (склады / предметы)
// -----------------------------
async function createLeaf() {
    const name = prompt(mode === "storages" ? "Имя склада:" : "Имя предмета:");
    if (!name) return;

    const categoryPath = buildPathString();

    const url = mode === "storages"
        ? "/api/storages"
        : "/api/item-types";

    const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, categoryPath })
    });

    if (res.ok) return reloadActiveTree();

    if (res.status === 404) showError("Категория не найдена");
    else if (res.status === 409) showError("Элемент уже существует");
    else showError("Ошибка создания");
}

async function deleteLeaf(name) {
    const categoryPath = buildPathString();

    const url = mode === "storages"
        ? "/api/storages"
        : "/api/item-types";

    const res = await fetch(url, {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, categoryPath })
    });

    if (res.ok) return reloadActiveTree();

    if (res.status === 404) showError("Элемент не найден");
    else if (res.status === 409) showError("Элемент не пуст");
    else showError("Ошибка удаления");
}

// -----------------------------
// РЕНДЕР
// -----------------------------
function render() {
    const root = getActiveRoot();
    if (!root) return;

    const current = getCurrentCategory();

    // путь
    const pathElem = document.getElementById("path");
    const fullPath = "/" + path.join("/");
    pathElem.textContent = fullPath === "/" ? "/ (корень)" : fullPath;

    // кнопка вверх
    document.getElementById("btnUp").disabled = path.length === 0;

    // заголовки
    document.getElementById("categoryHeader").textContent =
        mode === "storages" ? "Категории складов" : "Категории предметов";

    document.getElementById("leafHeader").textContent =
        mode === "storages" ? "Склады" : "Предметы";

    // списки
    const catList = document.getElementById("categoryList");
    const leafList = document.getElementById("leafList");

    catList.innerHTML = "";
    leafList.innerHTML = "";

    // категории
    current.children.forEach(cat => {
        const li = document.createElement("li");

        li.onclick = () => {
            path.push(cat.name);
            render();
        };

        const nameSpan = document.createElement("span");
        nameSpan.textContent = cat.name;

        const delBtn = document.createElement("button");
        delBtn.textContent = "Удалить";
        delBtn.onclick = (e) => {
            e.stopPropagation();
            const full = buildChildPath(cat.name);
            if (confirm(`Удалить категорию "${cat.name}"?`)) {
                deleteCategory(full);
            }
        };

        li.appendChild(nameSpan);
        li.appendChild(delBtn);
        catList.appendChild(li);
    });

    // листья
    const leafNames = mode === "storages"
        ? current.storageNames
        : current.itemNames;

    leafNames.forEach(name => {
        const li = document.createElement("li");

        const nameSpan = document.createElement("span");
        nameSpan.textContent = name;

        const delBtn = document.createElement("button");
        delBtn.textContent = "Удалить";
        delBtn.onclick = (e) => {
            e.stopPropagation();
            if (confirm(`Удалить "${name}"?`)) {
                deleteLeaf(name);
            }
        };

        li.appendChild(nameSpan);
        li.appendChild(delBtn);
        leafList.appendChild(li);
    });
}

// -----------------------------
// ПЕРЕКЛЮЧЕНИЕ РЕЖИМОВ
// -----------------------------
document.getElementById("btnStoragesMode").onclick = () => {
    mode = "storages";
    path = [];
    document.getElementById("btnStoragesMode").classList.add("active");
    document.getElementById("btnItemsMode").classList.remove("active");
    reloadActiveTree();
};

document.getElementById("btnItemsMode").onclick = () => {
    mode = "items";
    path = [];
    document.getElementById("btnItemsMode").classList.add("active");
    document.getElementById("btnStoragesMode").classList.remove("active");
    reloadActiveTree();
};

// -----------------------------
// КНОПКИ
// -----------------------------
document.getElementById("btnCreateCategory").onclick = createCategory;
document.getElementById("btnCreateLeaf").onclick = createLeaf;

document.getElementById("btnUp").onclick = () => {
    if (path.length > 0) {
        path.pop();
        render();
    }
};

// -----------------------------
// СТАРТ
// -----------------------------
(async () => {
    await loadStorageTree();
    await loadItemTree();
    render();
})();
