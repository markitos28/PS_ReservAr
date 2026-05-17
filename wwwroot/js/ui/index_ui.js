/**
 * UI Manager para el Index - Manipulación del DOM y Renderizado de Eventos
 */
export const IndexUI = {
    elements: {
        eventsContainer: document.getElementById("eventsContainer"),
        eventsCount: document.getElementById("eventsCount"),
        navActions: document.getElementById("navActions"),
        welcomeUser: document.getElementById("welcomeUser"),
        loadingMessage: document.getElementById("loadingMessage"),
        errorMessage: document.getElementById("errorMessage"),
        // Inputs de filtros
        searchName: document.getElementById("searchName"),
        searchDate: document.getElementById("searchDate"),
        searchTime: document.getElementById("searchTime"),
        searchVenue: document.getElementById("searchVenue"),
        // Paginación
        prevPageButton: document.getElementById("prevPageButton"),
        nextPageButton: document.getElementById("nextPageButton"),
        currentPage: document.getElementById("currentPage"),
        totalPages: document.getElementById("totalPages"),
        paginationControls: document.getElementById("paginationControls")
    },

    setLoading: (isLoading) => {
        IndexUI.elements.loadingMessage.classList.toggle("hidden", !isLoading);
        IndexUI.elements.eventsContainer.style.opacity = isLoading ? "0.5" : "1";
    },

    renderEvents: (events, total, onCardClick) => {
        const { eventsContainer, eventsCount } = IndexUI.elements;
        eventsContainer.innerHTML = "";
        eventsCount.textContent = `${total} evento(s) encontrado(s)`;

        if (events.length === 0) {
            eventsContainer.innerHTML = `<div class="empty-message">No se encontraron eventos con esos filtros.</div>`;
            return;
        }

        events.forEach(event => {
            const date = new Date(event.eventDate);
            const card = document.createElement("article");
            card.className = "event-card";
            card.innerHTML = `
                <div class="event-image-placeholder">
                    <i class="icon-calendar"></i>
                </div>
                <div class="event-content">
                    <h4>${IndexUI.escapeHtml(event.name)}</h4>
                    <div class="event-info">
                        <p><strong>Fecha:</strong> ${date.toLocaleDateString()}</p>
                        <p><strong>Hora:</strong> ${event.eventTime || 'No definida'}</p>
                        <p><strong>Lugar:</strong> ${IndexUI.escapeHtml(event.venue)}</p>
                    </div>
                    <span class="status-badge">${event.status}</span>
                </div>
            `;
            card.onclick = () => onCardClick(event.id);
            eventsContainer.appendChild(card);
        });
    },

    updatePagination: (current, total) => {
        const { currentPage, totalPages, paginationControls, prevPageButton, nextPageButton } = IndexUI.elements;
        currentPage.textContent = current;
        totalPages.textContent = total;
        paginationControls.classList.toggle("hidden", total <= 1);
        prevPageButton.disabled = current === 1;
        nextPageButton.disabled = current === total;
    },

    renderNavbar: (user, onLogout) => {
        const { navActions, welcomeUser } = IndexUI.elements;
        if (user) {
            welcomeUser.textContent = `Bienvenido, ${user.name || 'Usuario'}`;
            navActions.innerHTML = `<button id="btnExit" class="btn btn-logout">Cerrar Sesión</button>`;
            document.getElementById("btnExit").onclick = onLogout;
        } else {
            welcomeUser.textContent = "Eventos en cartelera";
            navActions.innerHTML = `
                <a href="./login.html" class="btn btn-outline">Iniciar Sesión</a>
                <a href="./signup.html" class="btn btn-primary">Registrarse</a>
            `;
        }
    },

    escapeHtml: (str) => {
        const div = document.createElement('div');
        div.textContent = str;
        return div.innerHTML;
    }
};