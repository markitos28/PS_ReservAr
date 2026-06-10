import { API_BASE_URL } from '../config.js';
import { AuthManager } from '../auth/auth_manager.js';

export const EventService = {
    async getEvents(queryString) {
        const token = AuthManager.getToken();
        const headers = { "Content-Type": "application/json" };
        if (token) headers["Authorization"] = `Bearer ${token}`;

        const URI = `${API_BASE_URL}/events${queryString ? `?${new URLSearchParams(queryString)}` : ""}`;

        
        const response = await fetch(URI, { method: "GET", headers });
        if (response.status === 401) throw new Error("Sesión expirada");
        
        return response;
    }
};  