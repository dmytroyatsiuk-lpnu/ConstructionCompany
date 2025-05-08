async function showAddEmployeeForm() {
    const modal = document.getElementById("add_employee_modal");
    const select = document.getElementById("brigadeSelect");

    // Очистити попередні опції
    select.innerHTML = '<option value="">Select Brigade</option>';

    // Отримати бригади
    try {
        const response = await fetch("http://localhost:5001/api/manager/brigades", {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem("token")}`
            }
        });
        const brigades = await response.json();

        brigades.forEach(brigade => {
            const option = document.createElement("option");
            option.value = brigade.brigadeId;
            option.textContent = brigade.name + ' - ' + brigade.specialization || `Brigade #${brigade.brigadeId}`;
            select.appendChild(option);
        });
    } catch (error) {
        alert("Error loading brigades");
    }

    modal.style.display = "flex";
}

document.getElementById("add_employee_button").addEventListener("click", async () => {
    const inputs = document.querySelectorAll("#add_employee_modal_content input");
    const fullName = inputs[0].value;
    const hireDate = inputs[1].value;
    const salary = parseFloat(inputs[2].value);
    const position = inputs[3].value;
    const brigadeId = document.getElementById("brigadeSelect").value;

    const employeeDTO = {
        fullName,
        hireDate,
        salary,
        position,
        brigadeId: parseInt(brigadeId) || null // додано!
    };

    try {
        const response = await fetch("http://localhost:5001/api/manager/add-employee", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${localStorage.getItem("token")}`
            },
            body: JSON.stringify(employeeDTO)
        });

        const result = await response.json();

        if (response.ok) {
            alert(result.message);
            loadEmployees();
            loadEmployeeBrigadeTable();
            closeAddEmployeeModal();
        } else {
            alert(result.error || "Error adding employee");
        }
    } catch (error) {
        alert("Network error");
    }
});


function closeAddEmployeeModal(){
    document.getElementById("add_employee_modal").style.display = "none";
}


async function loadEmployees() {
    try {
        const response = await fetch("http://localhost:5001/api/manager/employees", {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem("token")}`
            }
        });

        if (!response.ok) {
            throw new Error("Failed to fetch employees");
        }

        const employees = await response.json();
        const tbody = document.querySelector("#employeesTable tbody");
        tbody.innerHTML = ""; // Очистити попередній вміст

        employees.forEach(emp => {
            const row = document.createElement("tr");

            const dateString = emp.hireDate.split('T')[0];
            const [year, month, day] = dateString.split("-");
            const formattedHireDate = `${day}.${month}.${year}`;

            row.innerHTML = `
                <td>${emp.fullName}</td>
                <td>${formattedHireDate}</td>
                <td>${emp.salary}</td>
                <td>${emp.position}</td>
            `;

            tbody.appendChild(row);
        });
    } catch (error) {
        alert("Error loading employees");
    }
}

async function loadEmployeeBrigadeTable() {
    try {
      const response = await fetch('http://localhost:5001/api/manager/employeebrigade', {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem("token")}`
        }
    });
      if (!response.ok) {
        throw new Error('Failed to fetch employee-brigade pairs');
      }

      const data = await response.json();
      const tableBody = document.querySelector('#employeesBrigadeTable tbody');
      tableBody.innerHTML = ''; // Очищення перед новим рендером

      data.forEach(pair => {
        const row = document.createElement('tr');
        row.innerHTML = `
          <td>${pair.employeeName}</td>
          <td>${pair.brigadeName}</td>
        `;
        tableBody.appendChild(row);
      });
    } catch (error) {
      console.error('Error loading employee-brigade table:', error);
    }
  }
window.addEventListener("DOMContentLoaded", async () => {
    await loadEmployeeBrigadeTable();
    await loadEmployees();
});



document.getElementById("export_employees_tojson").addEventListener("click", async () => {
    const token = localStorage.getItem("token");

    try {
        const response = await fetch("http://localhost:5001/api/manager/export/employees", {
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
        a.download = "Employees.json";
        a.click();

        URL.revokeObjectURL(url);
    } catch (error) {
        console.error("Export failed:", error);
        alert("Failed to export logs.");
    }
});




