export const AuthManager = {
    getToken: () => localStorage.getItem("jwtToken"),
    getLoggedUser: () => JSON.parse(localStorage.getItem("loggedUser")),
    
    isTokenExpired: (token) => {
        try {
            const payload = JSON.parse(atob(token.split(".")[1]));
            return payload.exp < Math.floor(Date.now() / 1000);
        } catch (e) { return true; }
    },

    logout: () => {
        localStorage.removeItem("jwtToken");
        localStorage.removeItem("loggedUser");
    }
};