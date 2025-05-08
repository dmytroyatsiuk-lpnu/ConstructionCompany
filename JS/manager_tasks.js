window.addEventListener("DOMContentLoaded", async () => {
    const select = document.getElementById("brigadeSelect");
    const tableBody = document.querySelector("#tasksTable tbody");

    select.innerHTML = '<option value="">Select Brigade</option>';

    // Завантаження бригад
    try {
        const response = await fetch("http://localhost:5001/api/manager/brigades", {
            headers: { 'Authorization': `Bearer ${localStorage.getItem("token")}` }
        });

        const brigades = await response.json();
        brigades.forEach(brigade => {
            const option = document.createElement("option");
            option.value = brigade.brigadeId;
            option.textContent = brigade.name + ' - ' + (brigade.specialization || `Brigade #${brigade.brigadeId}`);
            select.appendChild(option);
        });

    } catch (error) {
        alert("Error loading brigades");
    }

    // При виборі бригади — завантажити завдання
    select.addEventListener("change", async () => {
        const brigadeId = select.value;
        if (!brigadeId) return;

        try {
            const response = await fetch(`http://localhost:5001/api/manager/tasks/${brigadeId}`, {
                headers: { 'Authorization': `Bearer ${localStorage.getItem("token")}` }
            });

            const tasks = await response.json();
            tableBody.innerHTML = ""; // Очистити попередні

            tasks.forEach(task => {
                const row = document.createElement("tr");

                const dateString = task.createdAt;
                const [year, month, day] = dateString.split("-");
                const formattedCreatesAtDate = `${day}.${month}.${year}`;
                
                row.innerHTML = `
                    <td>${formattedCreatesAtDate}</td>
                    <td><input type="date" value="${task.deadline || ''}"></td>
                    <td><input class="description_table_field" type="text" value="${task.description || ''}"></td>
                    <td>
                        <select>
                            <option value="todo" ${task.status === 'todo' ? 'selected' : ''}>Очікує виконання</option>
                            <option value="in_progress" ${task.status === 'in_progress' ? 'selected' : ''}>Виконується</option>
                            <option value="done" ${task.status === 'done' ? 'selected' : ''}>Готово</option>
                        </select>
                    </td>
                    <td><button class="saveBtn">Зберегти зміни</button></td>
                `;

                // Додати обробник кнопки "Зберегти"
                row.querySelector(".saveBtn").addEventListener("click", async () => {
                    const deadline = row.querySelector('input[type="date"]').value;
                    const description = row.querySelector('input[type="text"]').value;
                    const status = row.querySelector('select').value;

                    try {
                        const updateResponse = await fetch(`http://localhost:5001/api/manager/tasks/${task.taskId}`, {
                            method: "PUT",
                            headers: {
                                'Content-Type': 'application/json',
                                'Authorization': `Bearer ${localStorage.getItem("token")}`
                            },
                            body: JSON.stringify({ deadline, description, status })
                        });

                        if (updateResponse.ok) {
                            alert("Завдання оновлено");
                        } else {
                            alert("Помилка оновлення завдання");
                        }

                    } catch (err) {
                        alert("Помилка при збереженні");
                    }
                });

                tableBody.appendChild(row);
            });

        } catch (err) {
            alert("Помилка при завантаженні завдань");
        }
    });
});



function showAddTaskForm() {
    const brigadeId = document.getElementById("brigadeSelect").value;
    if (!brigadeId) {
        alert("Оберіть бригаду перед додаванням завдання.");
        return;
    }

    const modal = document.getElementById("add_task_modal");
    modal.style.display = "flex";

    // Зберігаємо brigadeId у дата-атрибут модального вікна
    modal.setAttribute("data-brigade-id", brigadeId);
}

function closeAddTaskModal() {
    const modal = document.getElementById("add_task_modal");
    modal.style.display = "none";
}

// Обробка натискання "Add Task" в модальному вікні
document.addEventListener("DOMContentLoaded", () => {
    const addButton = document.getElementById("add_task_button_modal");

    addButton.addEventListener("click", async () => {
        const modal = document.getElementById("add_task_modal");
        const brigadeId = modal.getAttribute("data-brigade-id");

        const deadline = modal.querySelector('input[type="date"]').value;
        const description = modal.querySelector('input[type="text"]').value;
        const status = modal.querySelector('select').value;

        if (!description || !status || !deadline) {
            alert("Заповніть усі поля.");
            return;
        }

        try {
            const response = await fetch("http://localhost:5001/api/manager/add-task", {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem("token")}`
                },
                body: JSON.stringify({
                    brigadeId: parseInt(brigadeId),
                    deadline: deadline,
                    description: description,
                    status: status
                })
            });

            if (response.ok) {
                alert("Завдання додано.");
                closeAddTaskModal();

                // Оновити список завдань
                document.getElementById("brigadeSelect").dispatchEvent(new Event("change"));
            } else {
                const errorText = await response.text();
                alert("Помилка додавання завдання: " + errorText);
            }

        } catch (err) {
            alert("Помилка під час запиту: " + err.message);
        }
    });
});
