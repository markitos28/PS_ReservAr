const AUTH_API_BASE_URL = "http://localhost:5183/api/v1";
const USER_API_BASE_URL = "http://localhost:5183/api/v1";
const HOME_PAGE_URL = "/index.html";

const signupForm = document.getElementById("signupForm");
const fullNameInput = document.getElementById("fullName");
const emailInput = document.getElementById("email");
const passwordInput = document.getElementById("password");
const confirmPasswordInput = document.getElementById("confirmPassword");
const signupButton = document.getElementById("signupButton");

const togglePasswordButton = document.getElementById("togglePassword");
const toggleConfirmPasswordButton = document.getElementById("toggleConfirmPassword");

const loadingMessage = document.getElementById("loadingMessage");
const errorMessage = document.getElementById("errorMessage");
const successMessage = document.getElementById("successMessage");

togglePasswordButton.addEventListener("click", () => {
  togglePasswordVisibility(passwordInput, togglePasswordButton);
});

toggleConfirmPasswordButton.addEventListener("click", () => {
  togglePasswordVisibility(confirmPasswordInput, toggleConfirmPasswordButton);
});

signupForm.addEventListener("submit", async (event) => {
  event.preventDefault();
  hideMessages();

  const fullName = fullNameInput.value.trim();
  const email = emailInput.value.trim();
  const password = passwordInput.value.trim();
  const confirmPassword = confirmPasswordInput.value.trim();

  if (!fullName || !email || !password || !confirmPassword) {
    showError("Completá todos los campos.");
    return;
  }

  if (password !== confirmPassword) {
    showError("Las contraseñas no coinciden.");
    return;
  }

  if (password.length < 6) {
    showError("La contraseña debe tener al menos 6 caracteres.");
    return;
  }

  setLoadingState(true);

  try {
    const createdUser = await createUserInMsUser({
      name: fullName,
      email: email,
      password: password
    });

    const authResponse = await authenticateAgainstMsAuth(email, password);
    const token = authResponse.token || authResponse.jwt || authResponse.accessToken;

    if (!token) {
      throw new Error("El login se realizó pero Authentication no devolvió un token.");
    }

    localStorage.setItem("jwtToken", token);
    localStorage.setItem("loggedUser", JSON.stringify(createdUser));

    showSuccess("Usuario registrado correctamente. Redirigiendo...");

    setTimeout(() => {
      window.location.href = HOME_PAGE_URL;
    }, 900);
  } catch (error) {
    console.error("[CODE-ERROR] - Error durante el registro:", error);
    showError(error.message || "No se pudo registrar el usuario.");
  } finally {
    setLoadingState(false);
  }
});

async function createUserInMsUser(userPayload) {
  const response = await fetch(`${USER_API_BASE_URL}/users`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(userPayload)
  });

  const data = await parseJsonSafely(response);

  if (!response.ok) {
    throw new Error(data?.message || data?.detail || "No se pudo dar de alta el usuario.");
  }

  return data;
}

async function authenticateAgainstMsAuth(email, password) {
  const response = await fetch(`${AUTH_API_BASE_URL}/auth/login`, {
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
    throw new Error(data?.message || data?.detail || "No se pudo iniciar sesión luego del registro.");
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

function togglePasswordVisibility(input, button) {
  if (input.type === "password") {
    input.type = "text";
    button.textContent = "Ocultar";
    return;
  }

  input.type = "password";
  button.textContent = "Ver";
}

function setLoadingState(isLoading) {
  signupButton.disabled = isLoading;
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