const API_BASE_URL = "http://localhost:5183/api/v1";
const LOGIN_PAGE_URL = "/login.html";
const HOME_PAGE_URL = "/index.html";

const token = localStorage.getItem("jwtToken");

if (!token || isTokenExpired(token)) {
    localStorage.removeItem("jwtToken");
    localStorage.removeItem("loggedUser");
    window.location.href = LOGIN_PAGE_URL;
}

const urlParams = new URLSearchParams(window.location.search);
const eventId = urlParams.get("eventId");

const eventTitle = document.getElementById("eventTitle");
const sectorsContainer = document.getElementById("sectorsContainer");
const seatsContainer = document.getElementById("seatsContainer");
const selectedSectorLabel = document.getElementById("selectedSectorLabel");

const loadingMessage = document.getElementById("loadingMessage");
const errorMessage = document.getElementById("errorMessage");
const successMessage = document.getElementById("successMessage");

const reserveButton = document.getElementById("reserveButton");
const payButton = document.getElementById("payButton");
const cancelButton = document.getElementById("cancelButton");
const logoutButton = document.getElementById("logoutButton");

const timerBox = document.getElementById("timerBox");
const timerValue = document.getElementById("timerValue");

let sectors = [];
let selectedSector = null;
let selectedSeat = null;
let activeReservation = null;
let timerInterval = null;
let remainingSeconds = 300;

document.addEventListener("DOMContentLoaded", async () => {
    if (!eventId) {
        showError("No se recibió el evento seleccionado.");
        return;
    }

    await loadEvent();
    await loadSectorsWithSeats();
});

logoutButton.addEventListener("click", () => {
    localStorage.removeItem("jwtToken");
    localStorage.removeItem("loggedUser");
    window.location.href = HOME_PAGE_URL;
});

reserveButton.addEventListener("click", async () => {
    await reserveSelectedSeat();
});

payButton.addEventListener("click", async () => {
    await payReservation();
});

cancelButton.addEventListener("click", () => {
    clearSelection();
});

async function loadEvent() {
    try {
        const response = await fetchWithAuth(`${API_BASE_URL}/events/${eventId}`, {
            method: "GET"
        });

        if (!response) {
            return;
        }

        const data = await parseJsonSafely(response);

        if (!response.ok) {
            throw new Error(data?.message || data?.detail || "No se pudo obtener el evento.");
        }

        eventTitle.textContent = `${data.name} - ${data.venue}`;
    } catch (error) {
        console.error("[CODE-ERROR] - Error al cargar evento:", error);
        showError(error.message || "No se pudo cargar el evento.");
    }
}

async function loadSectorsWithSeats() {
    setLoading(true);
    hideMessages();

    try {
        const response = await fetchWithAuth(`${API_BASE_URL}/sectors?eventId=${encodeURIComponent(eventId)}`, {
            method: "GET"
        });

        if (!response) {
            return;
        }

        const data = await parseJsonSafely(response);

        if (!response.ok) {
            throw new Error(data?.message || data?.detail || "No se pudieron obtener los sectores.");
        }

        sectors = data || [];

        for (const idx_tk of sectors) {
            const seatsResponse = await fetchWithAuth(`${API_BASE_URL}/seats?sectorId=${encodeURIComponent(idx_tk.id)}`, {
                method: "GET"
            });

            if (!seatsResponse) {
                return;
            }

            const seatsData = await parseJsonSafely(seatsResponse);

            if (!seatsResponse.ok) {
                throw new Error(seatsData?.message || seatsData?.detail || "No se pudieron obtener los asientos.");
            }

            idx_tk.seats = (seatsData || []).filter((seat) => Number(seat.sectorId) === Number(idx_tk.id));
        }

        renderSectorsList();
        renderSectorsMatrix();
    } catch (error) {
        console.error("[CODE-ERROR] - Error al cargar sectores y asientos:", error);
        showError(error.message || "No se pudieron cargar los sectores y asientos.");
    } finally {
        setLoading(false);
    }
}

function renderSectorsList() {
    sectorsContainer.innerHTML = "";

    if (!sectors.length) {
        sectorsContainer.innerHTML = "<p>No hay sectores disponibles para este evento.</p>";
        return;
    }

    const orderedSectors = [...sectors].sort((a, b) => {
        if (a.name.toLowerCase().includes("baja")) return -1;
        if (b.name.toLowerCase().includes("baja")) return 1;
        return 0;
    });
    
    for (const idx_tk of orderedSectors) {
        const card = document.createElement("article");
        card.className = "sector-card";
        card.dataset.sectorId = idx_tk.id;

        card.innerHTML = `
            <h3>${escapeHtml(idx_tk.name)}</h3>
            <p>Precio: $${idx_tk.price}</p>
            <p>Capacidad: ${idx_tk.capacity}</p>
        `;

        card.addEventListener("click", () => {
            selectedSector = idx_tk;
            selectedSeat = null;
            activeReservation = null;
        
            document.querySelectorAll(".sector-card").forEach((idx_tk) => {
                idx_tk.classList.remove("active");
            });
        
            card.classList.add("active");
        
            selectedSectorLabel.textContent = `Sector seleccionado: ${idx_tk.name}`;
            reserveButton.disabled = true;
            payButton.disabled = true;
            cancelButton.disabled = true;
        
            stopTimer();
        
            renderSectorsMatrix();
        
            const selectedMatrix = document.querySelector(`.sector-matrix[data-sector-id="${idx_tk.id}"]`);
        
            if (selectedMatrix) {
                selectedMatrix.classList.add("active");
                selectedMatrix.scrollIntoView({
                    behavior: "smooth",
                    block: "center"
                });
            }
        });

        sectorsContainer.appendChild(card);
    }
}

function renderSectorsMatrix(onlySectorId = null) {
    seatsContainer.innerHTML = "";

    if (!sectors.length) {
        seatsContainer.innerHTML = "<p>No hay sectores disponibles para este evento.</p>";
        return;
    }

    const sectorsToRender = onlySectorId
        ? sectors.filter((idx_tk) => Number(idx_tk.id) === Number(onlySectorId))
        : [...sectors].sort((a, b) => {
            if (a.name.toLowerCase().includes("baja")) return -1;
            if (b.name.toLowerCase().includes("baja")) return 1;
            return a.name.localeCompare(b.name);
        });

    for (const idx_tk of sectorsToRender) {
        const sectorBlock = document.createElement("section");
        sectorBlock.className = "sector-matrix";
        sectorBlock.dataset.sectorId = idx_tk.id;

        const title = document.createElement("h3");
        title.textContent = idx_tk.name;

        const grid = document.createElement("div");
        grid.className = "seats-grid";

        const orderedSeats = [...(idx_tk.seats || [])]
            .sort((a, b) => Number(a.seatNumber) - Number(b.seatNumber));

        const seatsPerRow = 10;

        for (let idx_tk = 0; idx_tk < orderedSeats.length; idx_tk += seatsPerRow) {
            const rowSeats = orderedSeats.slice(idx_tk, idx_tk + seatsPerRow);

            const rowDiv = document.createElement("div");
            rowDiv.className = "seat-row";

            for (const idx_tk of rowSeats) {
                const button = document.createElement("button");
                button.type = "button";

                const normalizedStatus = normalizeSeatStatus(idx_tk.status);

                button.className = `seat ${normalizedStatus}`;
                button.textContent = idx_tk.seatNumber;
                button.title = `Fila ${idx_tk.rowIdentifier} - Asiento ${idx_tk.seatNumber}`;

                if (normalizedStatus === "occupied") {
                    button.disabled = true;
                }

                button.addEventListener("click", () => {
                    if (normalizedStatus === "occupied") {
                        return;
                    }

                    selectedSeat = idx_tk;
                    selectedSector = sectors.find((sector) => Number(sector.id) === Number(idx_tk.sectorId)) || null;

                    document.querySelectorAll(".seat").forEach((seatButton) => {
                        if (!seatButton.classList.contains("occupied")) {
                            seatButton.classList.remove("selected");
                            seatButton.classList.add("available");
                        }
                    });
                    
                    button.classList.remove("available");
                    button.classList.remove("occupied");
                    button.classList.add("selected");

                    selectedSectorLabel.textContent =
                        `Sector seleccionado: ${selectedSector?.name || "Sector"} - Fila ${idx_tk.rowIdentifier} - Asiento ${idx_tk.seatNumber}`;

                    reserveButton.disabled = false;
                    cancelButton.disabled = false;
                    payButton.disabled = true;
                    hideMessages();
                });

                rowDiv.appendChild(button);
            }

            grid.appendChild(rowDiv);
        }

        sectorBlock.appendChild(title);
        sectorBlock.appendChild(grid);
        seatsContainer.appendChild(sectorBlock);
    }
}

async function reserveSelectedSeat() {
    if (!selectedSeat) {
        showError("Seleccioná un asiento disponible.");
        return;
    }

    setLoading(true);
    hideMessages();

    try {
        const user = getLoggedUser();

        const response = await fetchWithAuth(`${API_BASE_URL}/reservations`, {
            method: "POST",
            body: JSON.stringify({
                userId: user?.id || user?.Id || 1,
                seatId: selectedSeat.id
            })
        });

        if (!response) {
            return;
        }

        const data = await parseJsonSafely(response);

        if (!response.ok) {
            throw new Error(data?.message || data?.detail || "No se pudo generar la reserva.");
        }

        activeReservation = data;

        await patchSeatStatus(selectedSeat.id, "Reservado");

        showSuccess("Asiento reservado. Tenés 5 minutos para completar el pago.");
        startTimer();

        reserveButton.disabled = true;
        payButton.disabled = false;
        cancelButton.disabled = false;

        await loadSectorsWithSeats();

        if (selectedSector) {
            renderSectorsMatrix(selectedSector.id);
        }
    } catch (error) {
        console.error("[CODE-ERROR] - Error al reservar asiento:", error);
        showError(error.message || "No se pudo reservar el asiento.");
        await loadSectorsWithSeats();
    } finally {
        setLoading(false);
    }
}

async function patchSeatStatus(seatId, status) {
    const response = await fetchWithAuth(`${API_BASE_URL}/seats/${seatId}`, {
        method: "PATCH",
        body: JSON.stringify({
            status: status
        })
    });

    if (!response) {
        return null;
    }

    const data = await parseJsonSafely(response);

    if (!response.ok) {
        throw new Error(data?.message || data?.detail || "No se pudo actualizar el asiento.");
    }

    return data;
}

async function payReservation() {
    if (!activeReservation) {
        showError("No hay una reserva activa para pagar.");
        return;
    }

    setLoading(true);
    hideMessages();

    try {
        const reservationId =
            activeReservation.id ||
            activeReservation.reservationId ||
            activeReservation.Id;

        const response = await fetchWithAuth(`${API_BASE_URL}/payments`, {
            method: "POST",
            body: JSON.stringify({
                reservationId: reservationId
            })
        });

        if (!response) {
            return;
        }

        const data = await parseJsonSafely(response);

        if (!response.ok) {
            throw new Error(data?.message || data?.detail || "No se pudo procesar el pago.");
        }

        await patchSeatStatus(selectedSeat.id, "Vendido");

        stopTimer();
        showSuccess("Pago realizado correctamente. Asiento vendido.");

        activeReservation = null;
        selectedSeat = null;
        reserveButton.disabled = true;
        payButton.disabled = true;
        cancelButton.disabled = true;

        await loadSectorsWithSeats();

        if (selectedSector) {
            renderSectorsMatrix(selectedSector.id);
        }
    } catch (error) {
        console.error("[CODE-ERROR] - Error al pagar reserva:", error);
        showError(error.message || "No se pudo procesar el pago.");
    } finally {
        setLoading(false);
    }
}

function startTimer() {
    stopTimer();

    remainingSeconds = 300;
    timerBox.classList.remove("hidden");
    updateTimerText();

    timerInterval = setInterval(async () => {
        remainingSeconds -= 1;
        updateTimerText();

        if (remainingSeconds <= 0) {
            stopTimer();

            try {
                if (selectedSeat) {
                    await patchSeatStatus(selectedSeat.id, "Disponible");
                }

                showError("La reserva expiró. El asiento volvió a estar disponible.");

                activeReservation = null;
                selectedSeat = null;

                reserveButton.disabled = true;
                payButton.disabled = true;
                cancelButton.disabled = true;

                await loadSectorsWithSeats();

                if (selectedSector) {
                    renderSectorsMatrix(selectedSector.id);
                }
            } catch (error) {
                console.error("[CODE-ERROR] - Error al liberar asiento vencido:", error);
                showError("La reserva expiró, pero no se pudo liberar el asiento.");
            }
        }
    }, 1000);
}

function stopTimer() {
    if (timerInterval) {
        clearInterval(timerInterval);
        timerInterval = null;
    }

    timerBox.classList.add("hidden");
}

function updateTimerText() {
    const minutes = Math.floor(remainingSeconds / 60);
    const seconds = remainingSeconds % 60;

    timerValue.textContent =
        `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;
}

function clearSelection() {
    selectedSeat = null;
    activeReservation = null;

    reserveButton.disabled = true;
    payButton.disabled = true;
    cancelButton.disabled = true;

    stopTimer();

    if (selectedSector) {
        renderSectorsMatrix(selectedSector.id);
    } else {
        renderSectorsMatrix();
    }

    hideMessages();
}

function normalizeSeatStatus(status) {
    const value = String(status || "").toLowerCase();

    if (value.includes("vendido") || value.includes("ocupado") || value.includes("reservado")) {
        return "occupied";
    }

    return "available";
}

function buildAuthHeaders() {
    const currentToken = localStorage.getItem("jwtToken");

    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${currentToken}`
    };
}

async function fetchWithAuth(url, options = {}) {
    const currentToken = localStorage.getItem("jwtToken");

    if (!currentToken || isTokenExpired(currentToken)) {
        localStorage.removeItem("jwtToken");
        localStorage.removeItem("loggedUser");
        window.location.href = LOGIN_PAGE_URL;
        return null;
    }

    const response = await fetch(url, {
        ...options,
        headers: {
            "Content-Type": "application/json",
            ...options.headers,
            "Authorization": `Bearer ${currentToken}`
        }
    });

    if (response.status === 401) {
        localStorage.removeItem("jwtToken");
        localStorage.removeItem("loggedUser");
        window.location.href = LOGIN_PAGE_URL;
        return null;
    }

    return response;
}

function isTokenExpired(token) {
    try {
        const payload = JSON.parse(atob(token.split(".")[1]));
        const now = Math.floor(Date.now() / 1000);

        return payload.exp < now;
    } catch (error) {
        console.error("[CODE-ERROR] - Error al validar expiración del token:", error);
        return true;
    }
}

function getLoggedUser() {
    const rawUser = localStorage.getItem("loggedUser");

    if (!rawUser) {
        return null;
    }

    try {
        return JSON.parse(rawUser);
    } catch (error) {
        console.error("[CODE-ERROR] - Error al leer usuario logueado:", error);
        return null;
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
    successMessage.classList.add("hidden");
    errorMessage.textContent = "";
    successMessage.textContent = "";
}

function showError(message) {
    errorMessage.textContent = message;
    errorMessage.classList.remove("hidden");
}

function showSuccess(message) {
    successMessage.textContent = message;
    successMessage.classList.remove("hidden");
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