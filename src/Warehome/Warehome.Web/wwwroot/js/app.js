// ------------------------------------------------------------
// ГЛОБАЛЬНОЕ СОСТОЯНИЕ
// ------------------------------------------------------------

let mode = "storages";              // "storages" | "items"
let path = [];                      // путь в левой панели
let selectedLeaf = null;            // { type: "storage"|"item", name, categoryPath }

let storageTree = null;
let itemTree = null;

// состояние мини‑проводника в модалке добавления остатка
let stockSelectPath = [];
let stockSelectTree = null;         // дерево складов или типов
let stockSelectMode = null;         // "selectItemType" | "selectStorage"
let stockSelectChosenLeaf = null;   // { name, categoryPath }


// ------------------------------------------------------------
// ПОМОЩНИКИ
// ------------------------------------------------------------

async function safeFetch(url, options = {}, timeoutMs = 8000) {
    const controller = new AbortController();
    const id = setTimeout(() => controller.abort(), timeoutMs);

    try {
        const response = await fetch(url, {
            ...options,
            signal: controller.signal
        });

        clearTimeout(id);

        if (!response.ok) {
            const text = await response.text().catch(() => "");
            throw new Error(`HTTP ${response.status}: ${text}`);
        }

        return response;
    } catch (err) {
        clearTimeout(id);
        alert("Ошибка запроса: " + err.message);
        throw err;
    }
}


function getNodeByPath(tree, pathArr) {
    let node = tree;
    for (const p of pathArr) {
        const next = node.children.find(c => c.name === p);
        if (!next) return null;
        node = next;
    }
    return node;
}

function currentTree() {
    return mode === "storages" ? storageTree : itemTree;
}

function isRootPath() {
    return path.length === 0;
}

function apiCategoryUrl() {
    return mode === "storages"
        ? "/api/storage-categories"
        : "/api/item-type-categories";
}

function apiLeafUrl() {
    return mode === "storages"
        ? "/api/storages"
        : "/api/item-types";
}


// ------------------------------------------------------------
// ЗАГРУЗКА ДЕРЕВЬЕВ
// ------------------------------------------------------------

async function loadTrees() {
    const [storagesRes, itemsRes] = await Promise.all([
        safeFetch("/api/storage-categories/tree"),
        safeFetch("/api/item-type-categories/tree")
    ]);

    storageTree = await storagesRes.json();
    itemTree = await itemsRes.json();

    renderLeftPanel();
    renderRightPanel();
}


// ------------------------------------------------------------
// РЕНДЕР ЛЕВОЙ ПАНЕЛИ
// ------------------------------------------------------------

function renderLeftPanel() {
    const tree = currentTree();
    const node = getNodeByPath(tree, path);

    const pathEl = document.getElementById("path");
    pathEl.textContent = "/" + path.join("/");

    const categoryHeader = document.getElementById("categoryHeader");
    const leafHeader = document.getElementById("leafHeader");

    if (mode === "storages") {
        categoryHeader.textContent = "Категории складов";
        leafHeader.textContent = "Склады";
    } else {
        categoryHeader.textContent = "Категории предметов";
        leafHeader.textContent = "Типы предметов";
    }

    const categoryList = document.getElementById("categoryList");
    const leafList = document.getElementById("leafList");

    categoryList.innerHTML = "";
    leafList.innerHTML = "";

    // категории
    node.children.forEach(cat => {
        const li = document.createElement("li");
        li.className = "category-item";

        // КЛИК ПО ВСЕЙ ПЛАШКЕ
        li.onclick = () => {
            path.push(cat.name);
            selectedLeaf = null;
            renderLeftPanel();
            renderRightPanel();
        };

        // ТЕКСТ (не перехватывает клики)
        const span = document.createElement("span");
        span.textContent = cat.name;

        // КНОПКА УДАЛЕНИЯ
        const del = document.createElement("button");
        del.textContent = "×";
        del.className = "delete-btn";
        del.onclick = async (e) => {
            e.stopPropagation(); // не даём клику уйти на li

            if (!confirm("Удалить категорию?")) return;

            await safeFetch(apiCategoryUrl(), {
                method: "DELETE",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    categoryPath: isRootPath() ? null : path.join("/"),
                    name: cat.name
                })
            });

            await loadTrees();
            renderRightPanel();
        };

        li.appendChild(span);
        li.appendChild(del);
        categoryList.appendChild(li);
    });


    // листья (склады или типы)
    const leaves = mode === "storages" ? node.storageNames : node.itemNames;

    leaves.forEach(name => {
        const li = document.createElement("li");
        li.className = "leaf-item";

        // КЛИК ПО ВСЕЙ ПЛАШКЕ
        li.onclick = () => {

            selectedLeaf = {
                type: mode === "storages" ? "storage" : "item",
                name,
                categoryPath: path.join("/")
            };

            renderRightPanel();
        };

        // ТЕКСТ (не перехватывает клики)
        const span = document.createElement("span");
        span.textContent = name;

        // КНОПКА УДАЛЕНИЯ
        const del = document.createElement("button");
        del.textContent = "×";
        del.className = "delete-btn";
        del.onclick = async (e) => {
            e.stopPropagation(); // не даём клику уйти на li

            if (!confirm("Удалить?")) return;

            await safeFetch(apiLeafUrl(), {
                method: "DELETE",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    categoryPath: isRootPath() ? null : path.join("/"),
                    name
                })
            });

            selectedLeaf = null;
            await loadTrees();
            renderRightPanel();
        };

        li.appendChild(span);
        li.appendChild(del);
        leafList.appendChild(li);
    });

}


// ------------------------------------------------------------
// СОЗДАНИЕ КАТЕГОРИЙ И ЛИСТЬЕВ
// ------------------------------------------------------------

async function createCategory() {
    const name = prompt("Название новой категории:");
    if (!name) return;

    const body = {
        categoryPath: isRootPath() ? null : path.join("/"),
        name
    };

    await safeFetch(apiCategoryUrl(), {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
    });

    await loadTrees();
}

async function createLeaf() {
    const name = prompt(
        mode === "storages"
            ? "Название склада:"
            : "Название типа предмета:"
    );
    if (!name) return;

    const body = {
        categoryPath: isRootPath() ? null : path.join("/"),
        name
    };

    await safeFetch(apiLeafUrl(), {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
    });

    await loadTrees();
}


// ------------------------------------------------------------
// ЗАГРУЗКА ОСТАТКОВ
// ------------------------------------------------------------

async function loadStocks() {
    if (!selectedLeaf) return [];

    if (selectedLeaf.type === "storage") {
        const res = await safeFetch("/api/item-stocks/by-storage", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                storageName: selectedLeaf.name,
                storageCategoryPath: selectedLeaf.categoryPath || null
            })
        });
        const data = await res.json();
        return data.result || [];
    }

    if (selectedLeaf.type === "item") {
        const res = await safeFetch("/api/item-stocks/by-type", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                itemTypeName: selectedLeaf.name,
                itemTypeCategoryPath: selectedLeaf.categoryPath || null
            })
        });
        const data = await res.json();
        return data.result || [];
    }

    return [];
}


// ------------------------------------------------------------
// РЕНДЕР ПРАВОЙ ПАНЕЛИ
// ------------------------------------------------------------

async function renderRightPanel() {
    const header = document.getElementById("selectedHeader");
    const stockHeader = document.getElementById("stockHeader");
    const stockList = document.getElementById("stockList");
    const btnCreateStock = document.getElementById("btnCreateStock");

    stockList.innerHTML = "";

    if (!selectedLeaf) {
        header.textContent = "Ничего не выбрано";
        stockHeader.style.display = "none";
        btnCreateStock.style.display = "none";
        return;
    }

    const prefix = selectedLeaf.categoryPath
        ? "/" + selectedLeaf.categoryPath + "/"
        : "/";

    header.textContent =
        (selectedLeaf.type === "storage" ? "Склад: " : "Предмет: ") +
        prefix +
        selectedLeaf.name;

    btnCreateStock.style.display = "block";
    stockHeader.style.display = "block";

    const stocks = await loadStocks();

    stocks.forEach(stock => {
        const li = document.createElement("li");
        li.className = "stock-item";

        const title =
            selectedLeaf.type === "storage"
                ? stock.itemTypeName
                : stock.storageName;

        li.textContent = `${title} — ${stock.quantity}`;
        li.onclick = () => openStockDetailsModal(stock);

        stockList.appendChild(li);
    });
}


// ------------------------------------------------------------
// МОДАЛ ПРОСМОТРА / ИЗМЕНЕНИЯ / УДАЛЕНИЯ ОСТАТКА
// ------------------------------------------------------------

function openStockDetailsModal(stock) {
    const modal = document.getElementById("stockDetailsModal");

    document.getElementById("stockItemTypePath").textContent =
        "Тип: /" + stock.itemTypeCategoryPath + "/" + stock.itemTypeName;

    document.getElementById("stockStoragePath").textContent =
        "Склад: /" + stock.storageCategoryPath + "/" + stock.storageName;

    document.getElementById("stockCurrentQuantity").textContent =
        stock.quantity;

    document.getElementById("stockNewQuantity").value = stock.quantity;

    document.getElementById("btnApplyStockQuantity").onclick = async () => {
        const value = document.getElementById("stockNewQuantity").value.trim();
        const newQ = Number(value);

        if (value === "" || Number.isNaN(newQ) || newQ < 0) {
            alert("Введите корректное количество (целое неотрицательное)");
            return;
        }

        if (stock.id == null) {
            alert("У остатка нет id. Проверь, что backend реально возвращает поле id в списке остатков.");
            return;
        }

        const response = await safeFetch("/api/item-stocks/quantity", {
            method: "PATCH",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                id: stock.id,
                newQuantity: newQ
            })
        });

        if (!response.ok) {
            const text = await response.text().catch(() => "");
            alert("Ошибка изменения количества: " + response.status + " " + text);
            return;
        }

        closeStockDetailsModal();
        renderRightPanel();
    };

    document.getElementById("btnDeleteStock").onclick = async () => {
        if (!confirm("Удалить остаток?")) return;
        await safeFetch("/api/item-stocks", {
            method: "DELETE",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(stock.id)
        });
        closeStockDetailsModal();
        renderRightPanel();
    };

    modal.style.display = "block";
}

function closeStockDetailsModal() {
    document.getElementById("stockDetailsModal").style.display = "none";
}


// ------------------------------------------------------------
// МОДАЛ ДОБАВЛЕНИЯ ОСТАТКА (с мини‑проводником)
// ------------------------------------------------------------

function openStockCreateModal() {
    if (!selectedLeaf) return;

    const modal = document.getElementById("stockCreateModal");
    stockSelectPath = [];
    stockSelectChosenLeaf = null;

    if (selectedLeaf.type === "storage") {
        stockSelectMode = "selectItemType";
        stockSelectTree = itemTree;
        document.getElementById("stockSelectTargetHeader").textContent =
            "Выберите тип предмета";
        document.getElementById("stockSelectLeafHeader").textContent =
            "Типы предметов";
    } else {
        stockSelectMode = "selectStorage";
        stockSelectTree = storageTree;
        document.getElementById("stockSelectTargetHeader").textContent =
            "Выберите склад";
        document.getElementById("stockSelectLeafHeader").textContent =
            "Склады";
    }

    document.getElementById("stockCreateQuantityBlock").style.display = "none";
    document.getElementById("stockCreateQuantity").value = "";
    document.getElementById("stockCreateSelectedInfo").textContent = "";

    renderStockSelectTree();
    modal.style.display = "block";
}

function closeStockCreateModal() {
    document.getElementById("stockCreateModal").style.display = "none";
}


// ------------------------------------------------------------
// РЕНДЕР МИНИ‑ПРОВОДНИКА В МОДАЛКЕ
// ------------------------------------------------------------

function renderStockSelectTree() {
    const node = getNodeByPath(stockSelectTree, stockSelectPath);

    document.getElementById("stockSelectPath").textContent =
        "/" + stockSelectPath.join("/");

    const catList = document.getElementById("stockSelectCategoryList");
    const leafList = document.getElementById("stockSelectLeafList");

    catList.innerHTML = "";
    leafList.innerHTML = "";

    // категории
    node.children.forEach(cat => {
        const li = document.createElement("li");
        li.textContent = cat.name;
        li.onclick = () => {
            stockSelectPath.push(cat.name);
            renderStockSelectTree();
        };
        catList.appendChild(li);
    });

    // листья
    const leaves =
        stockSelectMode === "selectItemType"
            ? node.itemNames
            : node.storageNames;

    leaves.forEach(name => {
        const li = document.createElement("li");
        li.textContent = name;
        li.onclick = () => {
            stockSelectChosenLeaf = {
                name,
                categoryPath: stockSelectPath.join("/")
            };
            document.getElementById("stockCreateQuantityBlock").style.display =
                "block";
            document.getElementById("stockCreateSelectedInfo").textContent =
                "Выбрано: /" +
                stockSelectChosenLeaf.categoryPath +
                "/" +
                stockSelectChosenLeaf.name;
        };
        leafList.appendChild(li);
    });
}


// ------------------------------------------------------------
// СОЗДАНИЕ ОСТАТКА
// ------------------------------------------------------------

async function createStock() {
    if (!stockSelectChosenLeaf) return;

    const q = Number(document.getElementById("stockCreateQuantity").value);
    if (Number.isNaN(q)) return;

    let body = { quantity: q };

    if (selectedLeaf.type === "storage") {
        body.storageName = selectedLeaf.name;
        body.storageCategoryPath = selectedLeaf.categoryPath;
        body.itemTypeName = stockSelectChosenLeaf.name;
        body.itemTypeCategoryPath = stockSelectChosenLeaf.categoryPath;
    } else {
        body.itemTypeName = selectedLeaf.name;
        body.itemTypeCategoryPath = selectedLeaf.categoryPath;
        body.storageName = stockSelectChosenLeaf.name;
        body.storageCategoryPath = stockSelectChosenLeaf.categoryPath;
    }

    await safeFetch("/api/item-stocks", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
    });

    closeStockCreateModal();
    renderRightPanel();
}


// ------------------------------------------------------------
// ИНИЦИАЛИЗАЦИЯ
// ------------------------------------------------------------

document.getElementById("btnStoragesMode").onclick = () => {
    mode = "storages";
    path = [];
    selectedLeaf = null;
    document.getElementById("btnStoragesMode").classList.add("active");
    document.getElementById("btnItemsMode").classList.remove("active");
    renderLeftPanel();
    renderRightPanel();
};

document.getElementById("btnItemsMode").onclick = () => {
    mode = "items";
    path = [];
    selectedLeaf = null;
    document.getElementById("btnItemsMode").classList.add("active");
    document.getElementById("btnStoragesMode").classList.remove("active");
    renderLeftPanel();
    renderRightPanel();
};

document.getElementById("btnUp").onclick = () => {
    if (path.length === 0) return;
    path.pop();
    selectedLeaf = null;
    renderLeftPanel();
    renderRightPanel();
};

document.getElementById("btnCreateCategory").onclick = createCategory;
document.getElementById("btnCreateLeaf").onclick = createLeaf;

document.getElementById("btnCreateStock").onclick = openStockCreateModal;
document.getElementById("btnCloseStockCreate").onclick = closeStockCreateModal;
document.getElementById("btnCloseStockDetails").onclick = closeStockDetailsModal;
document.getElementById("btnCreateStockConfirm").onclick = createStock;

loadTrees();
