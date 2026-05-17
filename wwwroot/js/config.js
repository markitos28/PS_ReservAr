const path = window.location.pathname;
export const PATH_PROJECT = path.substring(0, path.lastIndexOf('/'));
export const API_BASE_URL = "http://localhost:5183/api/v1";

export const PAGES = {
    RESERVATIONS: `${PATH_PROJECT}/reservations.html`,
    LOGIN: `${PATH_PROJECT}/login.html`,
    HOME: `${PATH_PROJECT}/index.html`,
    SIGNUP: `${PATH_PROJECT}/signup.html`
};