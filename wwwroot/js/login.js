const AUTH_API_BASE_URL = "http://localhost:5183/api/v1";
const USER_API_BASE_URL = "http://localhost:5183/api/v1";
const HOME_PAGE_URL = "/index.html";

const loginForm = document.getElementById("loginForm");
const emailInput = document.getElementById("email");
const passwordInput = document.getElementById("password");
const loginButton = document.getElementById("loginButton");
const togglePasswordButton = document.getElementById("togglePassword");
const loadingMessage = document.getElementById("loadingMessage");
const errorMessage = document.getElementById("errorMessage");
const successMessage = document.getElementById("successMessage");

togglePasswordButton.addEventListener("click", () => {
  if (passwordInput.type === "password") {
    passwordInput.type = "text";
    togglePasswordButton.textContent = "Ocultar";
    return;
  }

  passwordInput.type = "password";
  togglePasswordButton.textContent = "Ver";
});

loginForm.addEventListener("submit", async (event) => {
  event.preventDefault();

  hideMessages();

  const email = emailInput.value.trim();
  const password = passwordInput.value.trim();

  if (!email || !password) {
    showError("Completá correo y contraseña.");
    return;
  }

  setLoadingState(true);

  try {
    const authResponse = await authenticateAgainstMsAuth(email, password);

    const token = authResponse.access_token || authResponse.token || authResponse.Token || authResponse.jwt || authResponse.accessToken;

    if (!token) {
      throw new Error("El MS Authentication no devolvió un token JWT válido.");
    }

    //const userResponse = await fetchUserProfile(token, email);

    //localStorage.setItem("jwtToken", token);
    //localStorage.setItem("loggedUser", JSON.stringify(userResponse));
    localStorage.setItem("jwtToken", token);
    localStorage.setItem("loggedUser", JSON.stringify({ email: email }));

    showSuccess("Login exitoso. Redirigiendo...");

    setTimeout(() => {
      window.location.href = HOME_PAGE_URL;
    }, 800);
  } catch (error) {
    console.error("[CODE-ERROR] - Error durante el login:", error);
    showError(error.message || "No se pudo iniciar sesión.");
  } finally {
    setLoadingState(false);
  }
});

async function authenticateAgainstMsAuth(email, password) {
  const response = await fetch(`${AUTH_API_BASE_URL}/auth`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({
      email: email,
      password: password
    })
  });

  const data = await parseJsonSafely(response);

  if (!response.ok) {
    throw new Error(data?.message || data?.detail || "Credenciales inválidas.");
  }

  return data;
}

async function fetchUserProfile(token, email) {
  const response = await fetch(`${USER_API_BASE_URL}/users/by-email?email=${encodeURIComponent(email)}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`
    }
  });

  const data = await parseJsonSafely(response);

  if (!response.ok) {
    throw new Error(data?.message || data?.detail || "No se pudo obtener el usuario desde el MS User.");
  }

  return data;
}

async function parseJsonSafely(response) {
  const contentType = response.headers.get("content-type");

  if (!contentType || !contentType.includes("application/json")) {
    return null;
  }

  return await response.json();
}

function setLoadingState(isLoading) {
  loginButton.disabled = isLoading;
  loadingMessage.classList.toggle("hidden", !isLoading);
}

function hideMessages() {
  errorMessage.classList.add("hidden");
  successMessage.classList.add("hidden");
  errorMessage.textContent = "";
  successMessage.textContent = "";
}

function showError(message) {
  errorMessage.textContent = message;
  errorMessage.classList.remove("hidden");
}

function showSuccess(message) {
  successMessage.textContent = message;
  successMessage.classList.remove("hidden");
}