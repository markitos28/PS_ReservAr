export const SignupUI = {
    elements: {
        signupForm: document.getElementById("signupForm"),
        fullName: document.getElementById("fullName"),
        email: document.getElementById("email"),
        password: document.getElementById("password"),
        confirmPassword: document.getElementById("confirmPassword"),
        signupButton: document.getElementById("signupButton"),
        loadingMessage: document.getElementById("loadingMessage"),
        errorMessage: document.getElementById("errorMessage"),
        successMessage: document.getElementById("successMessage"),
        toggles: {
            password: document.getElementById("togglePassword"),
            confirm: document.getElementById("toggleConfirmPassword")
        }
    },

    setLoading: (isLoading) => {
        SignupUI.elements.signupButton.disabled = isLoading;
        SignupUI.elements.loadingMessage.classList.toggle("hidden", !isLoading);
    },

    showFeedback: (msg, isError = true) => {
        const { errorMessage, successMessage } = SignupUI.elements;
        if (isError) {
            errorMessage.textContent = msg;
            errorMessage.classList.remove("hidden");
            successMessage.classList.add("hidden");
        } else {
            successMessage.textContent = msg;
            successMessage.classList.remove("hidden");
            errorMessage.classList.add("hidden");
        }
    },

    toggleVisibility: (inputEl, btnEl) => {
        const isPassword = inputEl.type === "password";
        inputEl.type = isPassword ? "text" : "password";
        btnEl.textContent = isPassword ? "Ocultar" : "Ver";
    }
};