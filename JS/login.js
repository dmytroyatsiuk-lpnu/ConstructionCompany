document.getElementById('log_in_button').addEventListener('click', async () => {
    const username = document.getElementById('log_in_username_field').value;
    const response = await fetch('http://localhost:5001/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            username: username,
            password: document.getElementById('log_in_password_field').value
        })
    });

    if (response.ok) {
        const data = await response.json();
        localStorage.setItem("token", data.token);
        localStorage.setItem("role", data.role);
        localStorage.setItem("username", username);
        console.log(data.token)
        console.log(data.role)
        switch(data.role) {
            case "admin":
                window.location.href = "admin.html";
                break;
            case "manager":
                window.location.href = "manager.html";
                break;
            case "brigadier":
                window.location.href = "brigadier.html";
                break;
        }
    } else {
        alert("Invalid email or password");
    }
})