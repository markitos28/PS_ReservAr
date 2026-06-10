import { AuthManager } from '../auth/auth_manager.js';
import { PAGES, API_BASE_URL} from '../config.js';

export async function fetchWithAuth(endpoint, options = {}) {
    const token = AuthManager.getToken();
    if (!token || AuthManager.isTokenExpired(token)) {
        AuthManager.logout();
        window.location.href = PAGES.LOGIN;
        return null;
    }

    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        ...options,
        headers: {
            "Content-Type": "application/json",
            ...options.headers,
            "Authorization": `Bearer ${token}`
        }
    });

    if (response.status === 401) {
        AuthManager.logout();
        window.location.href = PAGES.LOGIN;
    }
    return response;
}