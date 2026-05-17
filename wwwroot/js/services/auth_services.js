import { API_BASE_URL } from '../config.js';

export const AuthService = {
    async login(email, password) {
        const response = await fetch(`${API_BASE_URL}/auth`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password })
        });
        
        if (!response.ok) {
            const data = await response.json().catch(() => ({}));
            throw new Error(data?.message || data?.detail || "Credenciales inválidas.");
        }
        return response.json();
    },

    async getUserProfile(token, email) {
        const response = await fetch(`${API_BASE_URL}/users/by-email?email=${encodeURIComponent(email)}`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error("No se pudo obtener el perfil del usuario.");
        }
        return response.json();
    },

    async register (userData) {
    const response = await fetch(`${API_BASE_URL}/identity/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(userData)
    });

    if (!response.ok) {
        const data = await response.json().catch(() => ({}));
        throw new Error(data?.message || data?.detail || "Error en el registro.");
    }
    return response.json();
    }
};


