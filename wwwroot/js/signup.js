import { AuthService } from './services/auth_services.js';
import { SignupUI } from './ui/signup_ui.js';
import { PAGES } from './config.js';

document.addEventListener("DOMContentLoaded", () => {
    const { signupForm, toggles, password, confirmPassword } = SignupUI.elements;

    toggles.password.onclick = () => SignupUI.toggleVisibility(password, toggles.password);
    toggles.confirm.onclick = () => SignupUI.toggleVisibility(confirmPassword, toggles.confirm);

    signupForm.onsubmit = async (e) => {
        e.preventDefault();
        
        const data = {
            fullName: SignupUI.elements.fullName.value.trim(),
            email: SignupUI.elements.email.value.trim(),
            password: password.value,
            confirmPassword: confirmPassword.value
        };

        if (data.password !== data.confirmPassword) {
            SignupUI.showFeedback("Las contraseñas no coinciden.");
            return;
        }

        SignupUI.setLoading(true);

        try {
            await AuthService.register({
                name: data.fullName,
                email: data.email,
                password: data.password
            });

            SignupUI.showFeedback("¡Cuenta creada! Redirigiendo al login...", false);
            setTimeout(() => window.location.href = PAGES.LOGIN, 2000);
            
        } catch (err) {
            SignupUI.showFeedback(err.message);
        } finally {
            SignupUI.setLoading(false);
        }
    };
});