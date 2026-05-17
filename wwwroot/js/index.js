import { EventService } from './services/event_services.js';
import { IndexUI } from './ui/index_ui.js';
import { AuthManager } from './auth/auth_manager.js';

let state = {
    currentPage: 1,
    pageSize: 6,
    totalPages: 1
};

async function init() {
    setupEventListeners();
    renderAuthUI();
    await fetchAndRender();
}

async function fetchAndRender() {
    IndexUI.setLoading(true);
    try {
        const filters = {
            name: IndexUI.elements.searchName.value,
            date: IndexUI.elements.searchDate.value,
            time: IndexUI.elements.searchTime.value,
            venue: IndexUI.elements.searchVenue.value,
            pageNumber: state.currentPage,
            pageSize: state.pageSize
        };

        const response = await EventService.getEvents(filters);
        const data = await response.json();

        state.totalPages = data.totalPages || 1;
        
        IndexUI.renderEvents(data.items, data.totalRecords, (id) => {
            const token = AuthManager.getToken();

            console.log(token);
            console.log(id);    

            window.location.href = token ? `./reservations.html?eventId=${id}` : './login.html';
        });

        IndexUI.updatePagination(state.currentPage, state.totalPages);
    } catch (error) {
        console.error("Error al cargar eventos:", error);
    } finally {
        IndexUI.setLoading(false);
    }
}

function renderAuthUI() {
    const user = AuthManager.getLoggedUser();
    IndexUI.renderNavbar(user, () => {
        AuthManager.logout();
        location.reload();
    });
}

function setupEventListeners() {
    document.getElementById("searchButton").onclick = () => {
        state.currentPage = 1;
        fetchAndRender();
    };

    document.getElementById("clearButton").onclick = () => {
        IndexUI.elements.searchName.value = "";
        IndexUI.elements.searchDate.value = "";
        IndexUI.elements.searchTime.value = "";
        IndexUI.elements.searchVenue.value = "";
        state.currentPage = 1;
        fetchAndRender();
    };

    IndexUI.elements.prevPageButton.onclick = () => {
        if (state.currentPage > 1) {
            state.currentPage--;
            fetchAndRender();
        }
    };

    IndexUI.elements.nextPageButton.onclick = () => {
        if (state.currentPage < state.totalPages) {
            state.currentPage++;
            fetchAndRender();
        }
    };
}

document.addEventListener("DOMContentLoaded", init);