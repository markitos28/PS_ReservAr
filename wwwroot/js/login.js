import { AuthService } from './services/auth_services.js';
import { LoginUI } from './ui/login_ui.js';
import { AuthManager } from './auth/auth_manager.js';
import { PAGES } from './config.js';

document.addEventListener("DOMContentLoaded", () => {
    const { loginForm, togglePasswordButton } = LoginUI.elements;

    togglePasswordButton.onclick = LoginUI.togglePasswordVisibility;

    loginForm.onsubmit = async (e) => {
        e.preventDefault();
        const email = LoginUI.elements.emailInput.value.trim();
        const password = LoginUI.elements.passwordInput.value.trim();

        if (!email || !password) {
            LoginUI.showError("Completá correo y contraseña.");
            return;
        }

        LoginUI.setLoading(true);

        try {
            // 1. Autenticación
            const authData = await AuthService.login(email, password);

            if(!authData.access_token) {
                throw new Error("Respuesta inválida del servidor.");
            }

            localStorage.setItem("jwtToken", authData.access_token);

            // 2. Obtener perfil
            const userProfile = await AuthService.getUserProfile(authData.access_token, email);
            localStorage.setItem("loggedUser", JSON.stringify(userProfile));

            LoginUI.showSuccess("¡Ingreso exitoso! Redirigiendo...");
            
            setTimeout(() => {
                window.location.href = PAGES.HOME;
            }, 1500);

        } catch (error) {
            LoginUI.showError(error.message);
            localStorage.removeItem("jwtToken");
            localStorage.removeItem("loggedUser");
        } finally {
            LoginUI.setLoading(false);
        }
    };
});