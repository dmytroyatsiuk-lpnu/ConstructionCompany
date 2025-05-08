document.addEventListener("DOMContentLoaded", async () => {
    const infoContainer = document.getElementById("company_information");

    try {
        const response = await fetch("http://localhost:5001/api/manager/company-info", {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });

        if (!response.ok) throw new Error("Не вдалося отримати інформацію");

        const data = await response.json();

        infoContainer.innerHTML = `
            <h2>Перелік проектів:</h2>
            <table border="1" cellpadding="5" cellspacing="0">
                <thead>
                    <tr><th>Назва</th><th>Статус</th><th>Бюджет (грн)</th></tr>
                </thead>
                <tbody>
                ${data.projects.map(p => `
                    <tr>
                        <td>${p.name}</td>
                        <td>${
                            p.status === "todo" ? "Очікує виконання" :
                            p.status === "planning" ? "Планується" :
                            p.status === "in_progress" ? "Виконується" :
                            p.status === "completed" ? "Виконаний" :
                            p.status
                        }</td>
                        <td>${p.budget}</td>
                    </tr>
                `).join("")}
                
                </tbody>
            </table>
            <p><strong>Сума бюджетів проектів:</strong> ${data.totalBudget} грн</p>

            <h3>Активні бригади:</h3>
            <table border="1" cellpadding="5" cellspacing="0">
                <tr><th>Назва бригади</th></tr>
                ${data.activeBrigades.map(b => `<tr><td>${b}</td></tr>`).join("")}
            </table>

            <h3>Неактивні бригади:</h3>
            <table border="1" cellpadding="5" cellspacing="0">
                <tr><th>Назва бригади</th></tr>
                ${data.inactiveBrigades.map(b => `<tr><td>${b}</td></tr>`).join("")}
            </table>

            <h3>Працівники:</h3>
            <table border="1" cellpadding="5" cellspacing="0">
                <thead>
                    <tr><th>ПІБ</th><th>Посада</th><th>Зарплата (грн)</th></tr>
                </thead>
                <tbody>
                    ${data.employees.map(e => `
                        <tr><td>${e.fullName}</td><td>${e.position}</td><td>${e.salary}</td></tr>
                    `).join("")}
                </tbody>
            </table>
            <p><strong>Сума зарплат:</strong> ${data.totalSalaries} грн</p>

            <h3>Приналежність працівників до бригад:</h3>
            <table border="1" cellpadding="5" cellspacing="0">
                <thead>
                    <tr><th>Працівник</th><th>Бригада</th></tr>
                </thead>
                <tbody>
                    ${data.employeeBrigades.map(pair => `
                        <tr><td>${pair.employeeName}</td><td>${pair.brigadeName}</td></tr>
                    `).join("")}
                </tbody>
            </table>

            <h3>Приналежність бригад до проектів:</h3>
            <table border="1" cellpadding="5" cellspacing="0">
                <thead>
                    <tr><th>Бригада</th><th>Проект</th></tr>
                </thead>
                <tbody>
                    ${data.brigadeProjects.map(bp => `
                        <tr><td>${bp.brigadeName}</td><td>${bp.projectName}</td></tr>
                    `).join("")}
                </tbody>
            </table>

            <p><strong>Загальна вартість матеріалів:</strong> ${data.totalMaterialCost} грн</p>
        `;
        const exportButton = document.createElement('button');
        exportButton.id = 'export_to_jpeg';
        exportButton.textContent = 'Export to jpeg';
        infoContainer.appendChild(exportButton);

        // Тепер можна підв’язати обробник подій
        exportButton.addEventListener('click', () => {
            const navbar = document.getElementById('navbar');
            exportButton.style.display = 'none';

            html2canvas(infoContainer, {
                scale: 2,
                useCORS: true
            }).then(canvas => {
                const link = document.createElement('a');
                link.download = 'company_information.jpeg';
                link.href = canvas.toDataURL('image/jpeg', 0.95);
                link.click();

                navbar.style.display = '';
                exportButton.style.display = '';
            });
        });

    } catch (error) {
        infoContainer.innerHTML = `<p style="color:red">Помилка завантаження: ${error.message}</p>`;
    }
});

document.getElementById('export_to_jpeg').addEventListener('click', () => {
    const navbar = document.getElementById('navbar');
    const buttonJpeg = document.getElementById('export_to_jpeg');
    const content = document.getElementById('company_information');

    // Тимчасово ховаємо елементи
    navbar.style.display = 'none';
    buttonJpeg.style.display = 'none';

    html2canvas(content, {
        scale: 2,
        useCORS: true
    }).then(canvas => {
        const link = document.createElement('a');
        link.download = 'company_information.jpeg';
        link.href = canvas.toDataURL('image/jpeg', 0.95);
        link.click();

        // Повертаємо все назад
        navbar.style.display = '';
        buttonJpeg.style.display = '';
    });
});

