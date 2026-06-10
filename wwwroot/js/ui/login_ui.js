export const LoginUI = {
    elements: {
        loginForm: document.getElementById("loginForm"),
        emailInput: document.getElementById("email"),
        passwordInput: document.getElementById("password"),
        loginButton: document.getElementById("loginButton"),
        togglePasswordButton: document.getElementById("togglePassword"),
        loadingMessage: document.getElementById("loadingMessage"),
        errorMessage: document.getElementById("errorMessage"),
        successMessage: document.getElementById("successMessage")
    },

    setLoading: (isLoading) => {
        const { loginButton, loadingMessage } = LoginUI.elements;
        loginButton.disabled = isLoading;
        loadingMessage.classList.toggle("hidden", !isLoading);
    },

    showError: (message) => {
        const { errorMessage, successMessage } = LoginUI.elements;
        successMessage.classList.add("hidden");
        errorMessage.textContent = message;
        errorMessage.classList.remove("hidden");
    },

    showSuccess: (message) => {
        const { errorMessage, successMessage } = LoginUI.elements;
        errorMessage.classList.add("hidden");
        successMessage.textContent = message;
        successMessage.classList.remove("hidden");
    },

    togglePasswordVisibility: () => {
        const { passwordInput, togglePasswordButton } = LoginUI.elements;
        if (passwordInput.type === "password") {
            passwordInput.type = "text";
            togglePasswordButton.textContent = "Ocultar";
        } else {
            passwordInput.type = "password";
            togglePasswordButton.textContent = "Ver";
        }
    }
};