document.addEventListener('DOMContentLoaded', async () => {
    const container = document.getElementById('tiles-container');
    const token = localStorage.getItem("token");

    try {
        const response = await fetch('http://localhost:5001/api/admin/tables', {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });

        if (!response.ok) {
            throw new Error("Unable to fetch tables");
        }

        const tables = await response.json();

        tables.forEach(tableName => {
            const tile = document.createElement('div');
            tile.className = 'tile';
            tile.textContent = tableName;
            tile.addEventListener('click', () => openModal(tableName));
            container.appendChild(tile);
        });

    } catch (error) {
        console.error(error);
        alert("Failed to load tables.");
    }
});

async function openModal(tableName) {
    const modal = document.getElementById('modal');
    const title = document.getElementById('modal-title');
    const editor = document.getElementById('table-editor');
    editor.innerHTML = ''; // Очистити попередній вміст
    modal.style.display = 'flex';
    title.textContent = `Editing table: ${tableName}`;

    const token = localStorage.getItem("token");

    try {
        const response = await fetch(`http://localhost:5001/api/admin/tables/${tableName}`, {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });

        if (!response.ok) {
            throw new Error(`Unable to fetch data for table ${tableName}`);
        }

        const data = await response.json();

        if (data.length === 0) {
            editor.innerHTML = '<p>Table is empty.</p>';
            return;
        }

        // Створення таблиці
        const table = document.createElement('table');
        table.classList.add('data-table');

        // Заголовок
        const thead = document.createElement('thead');
        const headerRow = document.createElement('tr');
        Object.keys(data[0]).forEach(key => {
            const th = document.createElement('th');
            th.textContent = key;
            headerRow.appendChild(th);
        });
        thead.appendChild(headerRow);
        table.appendChild(thead);

        // Дані
        const tbody = document.createElement('tbody');
        data.forEach(row => {
            const tr = document.createElement('tr');
            Object.values(row).forEach(value => {
                const td = document.createElement('td');
                td.textContent = value;
                tr.appendChild(td);
            });
            tbody.appendChild(tr);
        });
        table.appendChild(tbody);

        // Обгортаємо таблицю у div з прокруткою, якщо >10 рядків
        if (data.length > 10) {
            const wrapper = document.createElement('div');
            wrapper.classList.add('table-wrapper');
            wrapper.appendChild(table);
            editor.appendChild(wrapper);
        } else {
            editor.appendChild(table);
        }


    } catch (error) {
        console.error(error);
        editor.innerHTML = `<p style="color:red">Failed to load data: ${error.message}</p>`;
    }
}

function closeModal() {
    document.getElementById('modal').style.display = 'none';
    document.getElementById('table-editor').innerHTML = '';
}

function openAddUserModal() {
    document.getElementById('add_user_modal').style.display = 'flex';
}

document.getElementById('add_user_button').addEventListener('click', () => {
    openAddUserModal();
});

document.querySelector('#add_user_modal button:first-of-type').addEventListener('click', async () => {
    const inputs = document.querySelectorAll('#add_user_modal input');
    const username = inputs[0].value.trim();
    const password = inputs[1].value.trim();
    const role = inputs[2].value.trim();
    const fullName = inputs[3].value.trim();
    const employeeId = inputs[4].value.trim();

    if (!username || !password || !role || !fullName) {
        alert("All fields except employee ID are required.");
        return;
    }

    const user = {
        username,
        passwordHash: password,
        role,
        fullName,
        employeeId: employeeId ? parseInt(employeeId) : null
    };

    const token = localStorage.getItem("token");

    try {
        const response = await fetch("http://localhost:5001/api/admin/add-user", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            },
            body: JSON.stringify(user)
        });

        if (!response.ok) {
            const error = await response.json();
            alert("Error: " + error.error || response.statusText);
            return;
        }

        closeAddUserModal();
    } catch (error) {
        console.error(error);
        alert("Failed to add user.");
    }
});


function closeAddUserModal(){
    document.getElementById('add_user_modal').style.display = 'none';
}



async function openDeleteModal() {
    document.getElementById("deleteModal").style.display = "flex";
    await loadUsers();
}

function closeDeleteModal() {
    document.getElementById("deleteModal").style.display = "none";
    document.getElementById("userList").innerHTML = "";
}

async function loadUsers() {
    const token = localStorage.getItem("token");
    try {
        const response = await fetch("http://localhost:5001/api/admin/users", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        const users = await response.json();

        const userList = document.getElementById("userList");
        userList.innerHTML = "";

        users.forEach(user => {
            const userDiv = document.createElement("div");
            userDiv.style.display = "flex";
            userDiv.style.justifyContent = "space-between";
            userDiv.style.marginBottom = "5px";

            userDiv.innerHTML = `
                <span>${user.username}</span>
                <button onclick="deleteUser('${user.username}')">Delete</button>
            `;

            userList.appendChild(userDiv);
        });

    } catch (err) {
        console.error("Помилка при завантаженні користувачів:", err);
        alert("Не вдалося завантажити список.");
    }
}

async function deleteUser(username) {
    const token = localStorage.getItem("token");

    if (!confirm(`Ви впевнені, що хочете видалити ${username}?`)) return;

    try {
        const response = await fetch(`http://localhost:5001/api/admin/delete-user/${username}`, {
            method: "DELETE",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        const data = await response.json();

        if (response.ok) {
            await loadUsers(); // оновити список після видалення
        } else {
            alert("Помилка: " + data.message || data.error);
        }
    } catch (err) {
        console.error("Помилка при видаленні:", err);
        alert("Не вдалося з'єднатись із сервером.");
    }
}

document.getElementById('edit_user_button').addEventListener('click', openEditUserModal);

async function openEditUserModal() {
    document.getElementById("edit_user_modal").style.display = "flex";

    // Показати список користувачів для вибору
    const token = localStorage.getItem("token");
    try {
        const response = await fetch("http://localhost:5001/api/admin/users", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        const users = await response.json();

        const username = prompt("Enter username to edit:");
        const user = users.find(u => u.username === username);

        if (!user) {
            alert("User not found.");
            closeEditUserModal();
            return;
        }

        document.getElementById("edit_username").value = user.username;
        // решту полів доведеться ввести вручну або витягнути з додаткового API, якщо доступний

    } catch (err) {
        console.error("Failed to load users:", err);
        alert("Failed to load users.");
    }
}

function closeEditUserModal() {
    document.getElementById("edit_user_modal").style.display = "none";
}

async function submitEditUser() {
    const username = document.getElementById("edit_username").value.trim();
    const password = document.getElementById("edit_password").value.trim();
    const role = document.getElementById("edit_role").value.trim();
    const fullName = document.getElementById("edit_fullname").value.trim();
    const employeeId = document.getElementById("edit_employeeId").value.trim();

    const user = {
        username,
        passwordHash: password || null,
        role,
        fullName,
        employeeId: employeeId ? parseInt(employeeId) : null
    };

    const token = localStorage.getItem("token");

    try {
        const response = await fetch("http://localhost:5001/api/admin/edit-user", {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            },
            body: JSON.stringify(user)
        });

        const result = await response.json();

        if (!response.ok) {
            alert("Error: " + result.error || result.message);
            return;
        }

        alert("User updated successfully.");
        closeEditUserModal();

    } catch (err) {
        console.error(err);
        alert("Failed to update user.");
    }
}

document.getElementById("export_UserActionLogs_tojson").addEventListener("click", async () => {
    const token = localStorage.getItem("token");

    try {
        const response = await fetch("http://localhost:5001/api/admin/export/UserActionLogs", {
            headers: {
                "Authorization": "Bearer " + token
            }
        });

        if (!response.ok) {
            throw new Error("Failed to fetch logs");
        }

        const logs = await response.json();

        const blob = new Blob([JSON.stringify(logs, null, 2)], { type: "application/json" });
        const url = URL.createObjectURL(blob);

        const a = document.createElement("a");
        a.href = url;
        a.download = "UserActionLogs.json";
        a.click();

        URL.revokeObjectURL(url);
    } catch (error) {
        console.error("Export failed:", error);
        alert("Failed to export logs.");
    }
});
