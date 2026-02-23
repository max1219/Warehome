// -----------------------------
// ГЛОБАЛЬНОЕ СОСТОЯНИЕ
// -----------------------------
let root = null;
let path = []; // массив имён категорий от корня до текущей

// -----------------------------
// УТИЛИТЫ
// -----------------------------
function getCurrentCategory() {
    let node = root;
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

function showError(message) {
    alert(message);
}

// -----------------------------
// ЗАГРУЗКА ДЕРЕВА
// -----------------------------
async function loadTree() {
    const res = await fetch("/api/storage-categories/tree");

    if (!res.ok) {
        showError("Ошибка загрузки дерева категорий");
        return;
    }

    root = await res.json();
    render();
}

// -----------------------------
// СОЗДАНИЕ ПОДКАТЕГОРИИ
// -----------------------------
async function createCategory() {
    const name = prompt("Введите имя подкатегории:");
    if (!name) return;

    const parentPath = buildPathString();

    const res = await fetch("/api/storage-categories", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            name,
            parentPath
        })
    });

    if (res.ok) {
        await loadTree();
        return;
    }

    switch (res.status) {
        case 404:
            showError("Родительская категория не найдена");
            break;
        case 409:
            showError("Категория с таким именем уже существует");
            break;
        default:
            showError("Ошибка создания категории");
    }
}

// -----------------------------
// СОЗДАНИЕ СКЛАДА
// -----------------------------
async function createStorage() {
    const name = prompt("Введите имя склада:");
    if (!name) return;

    const categoryPath = buildPathString();

    const res = await fetch("/api/storages", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            name,
            categoryPath
        })
    });

    if (res.ok) {
        await loadTree();
        return;
    }

    switch (res.status) {
        case 404:
            showError("Категория не найдена");
            break;
        case 409:
            showError("Склад с таким именем уже существует");
            break;
        default:
            showError("Ошибка создания склада");
    }
}

// -----------------------------
// УДАЛЕНИЕ КАТЕГОРИИ
// -----------------------------
async function deleteCategory(fullPath) {
    const res = await fetch("/api/storage-categories", {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ path: fullPath })
    });

    if (res.ok) {
        await loadTree();
        return;
    }

    switch (res.status) {
        case 404:
            showError("Категория не найдена");
            break;
        case 409:
            showError("Категория не пуста");
            break;
        default:
            showError("Ошибка удаления категории");
    }
}

// -----------------------------
// УДАЛЕНИЕ СКЛАДА
// -----------------------------
async function deleteStorage(name) {
    const categoryPath = buildPathString();

    const res = await fetch("/api/storages", {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            name,
            categoryPath
        })
    });

    if (res.ok) {
        await loadTree();
        return;
    }

    switch (res.status) {
        case 404:
            showError("Склад не найден");
            break;
        case 409:
            showError("Склад не пуст");
            break;
        default:
            showError("Ошибка удаления склада");
    }
}

// -----------------------------
// РЕНДЕР
// -----------------------------
function render() {
    const current = getCurrentCategory();

    // путь
    const pathElem = document.getElementById("path");
    const fullPath = "/" + path.join("/");
    pathElem.textContent = fullPath === "/" ? "/ (корень)" : fullPath;

    // кнопка вверх
    document.getElementById("btnUp").disabled = path.length === 0;

    // категории
    const catList = document.getElementById("categoryList");
    catList.innerHTML = "";

    current.children.forEach(cat => {
        const li = document.createElement("li");

        // переход по клику на плашку
        li.onclick = () => {
            path.push(cat.name);
            render();
        };

        const nameSpan = document.createElement("span");
        nameSpan.textContent = cat.name;

        const delBtn = document.createElement("button");
        delBtn.textContent = "Удалить";
        delBtn.onclick = async (e) => {
            e.stopPropagation();
            const fullPath = buildChildPath(cat.name);
            if (confirm(`Удалить категорию "${cat.name}"?`)) {
                await deleteCategory(fullPath);
            }
        };

        li.appendChild(nameSpan);
        li.appendChild(delBtn);
        catList.appendChild(li);
    });

    // склады
    const storageList = document.getElementById("storageList");
    storageList.innerHTML = "";

    current.storageNames.forEach(storage => {
        const li = document.createElement("li");

        const nameSpan = document.createElement("span");
        nameSpan.textContent = storage;

        const delBtn = document.createElement("button");
        delBtn.textContent = "Удалить";
        delBtn.onclick = async (e) => {
            e.stopPropagation();
            if (confirm(`Удалить склад "${storage}"?`)) {
                await deleteStorage(storage);
            }
        };

        li.appendChild(nameSpan);
        li.appendChild(delBtn);
        storageList.appendChild(li);
    });
}

// -----------------------------
// КНОПКИ
// -----------------------------
document.getElementById("btnCreateCategory").onclick = createCategory;
document.getElementById("btnCreateStorage").onclick = createStorage;

document.getElementById("btnUp").onclick = () => {
    if (path.length > 0) {
        path.pop();
        render();
    }
};

// -----------------------------
// СТАРТ
// -----------------------------
loadTree();
