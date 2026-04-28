const EVENT_API_BASE_URL = "http://localhost:5183/api/v1/event";
const RESERVATIONS_PAGE_URL = "/reservations.html";

const token = localStorage.getItem("jwtToken");

if (!token) {
    window.location.href = "/login.html";
}

const searchNameInput = document.getElementById("searchName");
const searchDateInput = document.getElementById("searchDate");
const searchTimeInput = document.getElementById("searchTime");
const searchVenueInput = document.getElementById("searchVenue");

const searchButton = document.getElementById("searchButton");
const clearButton = document.getElementById("clearButton");
const logoutButton = document.getElementById("logoutButton");

const eventsContainer = document.getElementById("eventsContainer");
const loadingMessage = document.getElementById("loadingMessage");
const errorMessage = document.getElementById("errorMessage");
const eventsCount = document.getElementById("eventsCount");

document.addEventListener("DOMContentLoaded", async () => {
    setLoggedUserLabel();
    await loadEvents();
});

searchButton.addEventListener("click", async () => {
    await loadEvents();
});

clearButton.addEventListener("click", async () => {
    searchNameInput.value = "";
    searchDateInput.value = "";
    searchTimeInput.value = "";
    searchVenueInput.value = "";
    await loadEvents();
});

logoutButton.addEventListener("click", () => {
    localStorage.removeItem("jwtToken");
    localStorage.removeItem("loggedUser");

    window.location.href = "/login.html";
});

async function loadEvents() {
    hideMessages();
    setLoading(true);

    try {
        const queryString = buildQueryString();

        const response = await fetch(`${EVENT_API_BASE_URL}/events${queryString}`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });

        const data = await parseJsonSafely(response);

        if (!response.ok) {
            throw new Error(data?.message || data?.detail || "No se pudieron obtener los eventos.");
        }

        const filteredEvents = applyClientSideFilters(data || []);
        renderEvents(filteredEvents);
    } catch (error) {
        console.error("[CODE-ERROR] - Error al cargar eventos:", error);
        showError(error.message || "No se pudieron cargar los eventos.");
        renderEvents([]);
    } finally {
        setLoading(false);
    }
}

function buildQueryString() {
    const params = new URLSearchParams();

    const name = searchNameInput.value.trim();
    const eventDate = searchDateInput.value;
    const venue = searchVenueInput.value.trim();

    if (name) {
        params.append("name", name);
    }

    if (eventDate) {
        params.append("eventDate", eventDate);
    }

    if (venue) {
        params.append("venue", venue);
    }

    const query = params.toString();

    return query ? `?${query}` : "";
}

function applyClientSideFilters(events) {
    const selectedTime = searchTimeInput.value;

    if (!selectedTime) {
        return events;
    }

    return events.filter((idx_tk) => {
        const eventDate = new Date(idx_tk.eventDate);
        const eventTime = eventDate.toTimeString().slice(0, 5);

        return eventTime === selectedTime;
    });
}

function renderEvents(events) {
    eventsContainer.innerHTML = "";
    eventsCount.textContent = `${events.length} evento(s)`;

    if (!events.length) {
        eventsContainer.innerHTML = `
            <div class="empty-message">
                No se encontraron eventos con los filtros seleccionados.
            </div>
        `;
        return;
    }

    for (const idx_tk of events) {
        const card = document.createElement("article");
        card.className = "event-card";

        const eventDate = new Date(idx_tk.eventDate);

        const formattedDate = eventDate.toLocaleDateString("es-AR");
        const formattedTime = eventDate.toLocaleTimeString("es-AR", {
            hour: "2-digit",
            minute: "2-digit"
        });

        card.innerHTML = `
            <h4>${escapeHtml(idx_tk.name)}</h4>
            <div class="event-info">
                <span><strong>Fecha:</strong> ${formattedDate}</span>
                <span><strong>Horario:</strong> ${formattedTime}</span>
                <span><strong>Lugar:</strong> ${escapeHtml(idx_tk.venue)}</span>
            </div>
            <span class="status-badge">${escapeHtml(idx_tk.status)}</span>
        `;

        card.addEventListener("click", () => {
            window.location.href = `${RESERVATIONS_PAGE_URL}?eventId=${idx_tk.id}`;
        });

        eventsContainer.appendChild(card);
    }
}

function setLoggedUserLabel() {
    const userRaw = localStorage.getItem("loggedUser");

    if (!userRaw) {
        return;
    }

    try {
        const user = JSON.parse(userRaw);
        const subtitle = document.querySelector(".brand span");

        if (subtitle && user?.name) {
            subtitle.textContent = `Bienvenido, ${user.name}`;
        }
    } catch (error) {
        console.error("[CODE-ERROR] - Error al leer usuario logueado:", error);
    }
}

async function parseJsonSafely(response) {
    const contentType = response.headers.get("content-type");

    if (!contentType || !contentType.includes("application/json")) {
        return null;
    }

    return await response.json();
}

function setLoading(isLoading) {
    loadingMessage.classList.toggle("hidden", !isLoading);
}

function hideMessages() {
    errorMessage.classList.add("hidden");
    errorMessage.textContent = "";
}

function showError(message) {
    errorMessage.textContent = message;
    errorMessage.classList.remove("hidden");
}

function escapeHtml(value) {
    if (value === null || value === undefined) {
        return "";
    }

    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}