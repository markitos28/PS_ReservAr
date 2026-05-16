const path = window.location.pathname;
const PATH_PROJECT= path.substring(0, path.lastIndexOf('/'));
const API_BASE_URL = "http://localhost:5183/api/v1";
const LOGIN_PAGE_URL = `${PATH_PROJECT}/login.html`;
const HOME_PAGE_URL = `${PATH_PROJECT}/index.html`;

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

const seatQuantityInput = document.getElementById("seatQuantityInput");
const increaseQuantityBtn = document.getElementById("increaseQuantityBtn");
const decreaseQuantityBtn = document.getElementById("decreaseQuantityBtn");
const quantityHelperText = document.getElementById("quantityHelperText");

const payButton = document.getElementById("payButton");
const cancelButton = document.getElementById("cancelButton");
const logoutButton = document.getElementById("logoutButton");

const timerBox = document.getElementById("timerBox");
const timerValue = document.getElementById("timerValue");

let sectors = [];
let selectedSector = null;
let selectedSeats = [];
let desiredSeatCount = 1;
let seatsCount = 0;
let activeReservation = null;
let timerInterval = null;
let postPaymentCountdownInterval = null;
let remainingSeconds = 300;

document.addEventListener("DOMContentLoaded", async () => {
    if (!eventId) {
        showError("No se recibió el evento seleccionado.");
        return;
    }

    await loadEvent();
    await loadSectorsWithSeats();
    setupQuantitySelector();
});

function setupQuantitySelector() {
    // Listener para cambios en el input
    seatQuantityInput.addEventListener("change", () => {
        updateDesiredSeatCount();
    });

    seatQuantityInput.addEventListener("input", () => {
        updateDesiredSeatCount();
    });

    // Listener para botones + y -
    increaseQuantityBtn.addEventListener("click", () => {
        const currentValue = parseInt(seatQuantityInput.value) || 1;
        if (currentValue < 20) {
            seatQuantityInput.value = currentValue + 1;
            updateDesiredSeatCount();
        }
    });

    decreaseQuantityBtn.addEventListener("click", () => {
        const currentValue = parseInt(seatQuantityInput.value) || 1;
        if (currentValue > 1) {
            seatQuantityInput.value = currentValue - 1;
            updateDesiredSeatCount();
        }
    });
}

function updateDesiredSeatCount() {
    const value = parseInt(seatQuantityInput.value);
    
    if (isNaN(value) || value < 1) {
        desiredSeatCount = 1;
        seatQuantityInput.value = 1;
    } else if (value > 20) {
        desiredSeatCount = 20;
        seatQuantityInput.value = 20;
    } else {
        desiredSeatCount = value;
    }

    // Limpiar selección anterior si el usuario cambia la cantidad
    if (selectedSeats.length > 0) {
        clearSelection();
    }

    updateQuantityHelperText();
    hideMessages();
}

function updateQuantityHelperText() {
    if (desiredSeatCount === 1) {
        quantityHelperText.textContent = "Seleccioná 1 asiento en el mapa";
    } else {
        quantityHelperText.textContent = `Seleccioná exactamente ${desiredSeatCount} asientos en el mapa`;
    }
}

logoutButton.addEventListener("click", async () => {

    let textMessage = "¿Desea cerrar sesión?";

    if (activeReservation) {
        textMessage = "Tenés una reserva activa. Si cerrás sesión, se perderá la selección de asientos.";
    }

    const result = await Swal.fire({
        title: "Cerrar sesión",
        text: textMessage,
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí, cerrar sesión",
        cancelButtonText: "Cancelar",
        confirmButtonColor: "#ef4444",
        cancelButtonColor: "#22c55e",
        background: "#0f172a",
        color: "#f8fafc"
    });

    if (!result.isConfirmed) {
        return;
    }

    // Si hay reserva activa la libera antes
    if (activeReservation) {
        await releaseCurrentReservation();
    }

    localStorage.removeItem("jwtToken");
    localStorage.removeItem("loggedUser");

    window.location.href = HOME_PAGE_URL;
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
        await fetchWithAuth(`${API_BASE_URL}/reservations/expire-pending`, {
            method: "PATCH"
        });

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
        highlightSelectedSectorMatrix();
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

        if (selectedSector && Number(selectedSector.id) === Number(idx_tk.id)) {
            card.classList.add("active");
        }

        card.innerHTML = `
            <h3>${escapeHtml(idx_tk.name)}</h3>
            <p>Precio: $${idx_tk.price}</p>
            <p>Capacidad: ${idx_tk.capacity}</p>
        `;

        card.addEventListener("click", () => {
            selectedSector = idx_tk;
            selectedSeats = [];
            seatsCount = 0;
            activeReservation = null;

            document.querySelectorAll(".sector-card").forEach((idx_tk) => {
                idx_tk.classList.remove("active");
            });

            card.classList.add("active");

            selectedSectorLabel.textContent = `Sector seleccionado: ${idx_tk.name}`;
            payButton.disabled = true;
            cancelButton.disabled = true;

            stopTimer();

            renderSectorsMatrix();
            highlightSelectedSectorMatrix();

            const selectedMatrix = document.querySelector(`.sector-matrix[data-sector-id="${idx_tk.id}"]`);

            if (selectedMatrix) {
                selectedMatrix.scrollIntoView({
                    behavior: "smooth",
                    block: "center"
                });
            }

            updateQuantityHelperText();
            hideMessages();
        });

        sectorsContainer.appendChild(card);
    }
}

function renderSectorsMatrix() {
    seatsContainer.innerHTML = "";

    if (!sectors.length) {
        seatsContainer.innerHTML = "<p>No hay sectores disponibles para este evento.</p>";
        return;
    }

    const sectorsToRender = [...sectors].sort((a, b) => {
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

                if (selectedSeats.some((seat) => seat.id === idx_tk.id) && normalizedStatus !== "occupied") {
                    button.classList.remove("available");
                    button.classList.add("selected");
                }

                if (normalizedStatus === "occupied") {
                    button.disabled = true;
                }

                button.addEventListener("click", () => {
                    if (normalizedStatus === "occupied") {
                        return;
                    }

                    const isAlreadySelected = selectedSeats.some((seat) => seat.id === idx_tk.id);

                    if (isAlreadySelected) {
                        // Deseleccionar asiento
                        selectedSeats = selectedSeats.filter((seat) => seat.id !== idx_tk.id);
                        seatsCount--;
                        button.classList.remove("selected");
                        button.classList.add("available");
                    } else {
                        // Verificar si ya hemos llegado al límite deseado
                        if (seatsCount >= desiredSeatCount) {
                            showError(`Ya tienes ${desiredSeatCount} asiento(s) seleccionado(s). Máximo permitido.`);
                            return;
                        }

                        // Seleccionar asiento
                        selectedSeats.push(idx_tk);
                        seatsCount++;
                        selectedSector = sectors.find((sector) => Number(sector.id) === Number(idx_tk.sectorId)) || null;
                        button.classList.remove("available");
                        button.classList.add("selected");
                    }

                    // Actualizar estado de botones y etiqueta
                    updateUIAfterSeatSelection();

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

function updateUIAfterSeatSelection() {
    if (selectedSeats.length === 0) {
        selectedSectorLabel.textContent = `Elegí un sector para ver sus butacas.`;
        payButton.disabled = true;
        cancelButton.disabled = true;
    } else if (seatsCount === desiredSeatCount) {
        selectedSectorLabel.textContent =
            `${seatsCount} asiento(s) seleccionado(s) - Listo para pagar`;
        payButton.disabled = false;
        cancelButton.disabled = false;
    } else {
        selectedSectorLabel.textContent =
            `${seatsCount} de ${desiredSeatCount} asiento(s) seleccionado(s)`;
        payButton.disabled = true;
        cancelButton.disabled = false;
    }
}

function highlightSelectedSectorMatrix() {
    document.querySelectorAll(".sector-matrix").forEach((matrix) => {
        matrix.classList.remove("active");
    });

    if (!selectedSector) {
        return;
    }

    const selectedMatrix = document.querySelector(`.sector-matrix[data-sector-id="${selectedSector.id}"]`);

    if (selectedMatrix) {
        selectedMatrix.classList.add("active");
    }
}

async function releaseCurrentReservation() {
    if (!activeReservation || selectedSeats.length === 0) {
        return;
    }

    const reservationId =
        activeReservation.id ||
        activeReservation.reservationId ||
        activeReservation.Id;

    try {
        if (reservationId) {
            await fetchWithAuth(`${API_BASE_URL}/reservations/${reservationId}`, {
                method: "PATCH",
                body: JSON.stringify({
                    status: "EXPIRADO"
                })
            });
        }

        for (const seat of selectedSeats) {
            await patchSeatStatus(seat.id, "Disponible");
        }

        activeReservation = null;
        selectedSeats = [];

        stopTimer();
    } catch (error) {
        console.error("[CODE-ERROR] - Error al liberar la reserva actual:", error);
    }
}

async function patchSeatStatus(seatId, status) {
    const response = await fetchWithAuth(`${API_BASE_URL}/seats/${seatId}`, {
        method: "PATCH",
        body: JSON.stringify({
            Status: status
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
                for (const seat of selectedSeats) {
                    await patchSeatStatus(seat.id, "Disponible");
                }

                showError("La reserva expiró. Los asientos volvieron a estar disponibles.");

                activeReservation = null;
                selectedSeats = [];
                seatsCount = 0;

                payButton.disabled = true;
                cancelButton.disabled = true;

                await loadSectorsWithSeats();
                highlightSelectedSectorMatrix();
            } catch (error) {
                console.error("[CODE-ERROR] - Error al liberar asientos vencidos:", error);
                showError("La reserva expiró, pero no se pudieron liberar los asientos.");
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
    selectedSeats = [];
    seatsCount = 0;
    activeReservation = null;

    payButton.disabled = true;
    cancelButton.disabled = true;

    stopTimer();

    renderSectorsMatrix();
    highlightSelectedSectorMatrix();
    updateQuantityHelperText();
    hideMessages();
}

function normalizeSeatStatus(status) {
    const value = String(status || "").toLowerCase();

    if (value.includes("vendido") || value.includes("ocupado") || value.includes("reservado")) {
        return "occupied";
    }

    return "available";
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
    successMessage.innerHTML = "";
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

document.querySelectorAll("a").forEach((link) => {
    link.addEventListener("click", async function (event) {
        if (!activeReservation) {
            return;
        }

        event.preventDefault();

        const canExit = await confirmExitWithReservation();

        if (canExit) {
            await releaseCurrentReservation();
            window.location.href = link.href;
        }
    });
});

async function confirmExitWithReservation() {
    if (!activeReservation) {
        return true;
    }

    const result = await Swal.fire({
        title: "Reserva en curso",
        text: "Tenés una reserva activa. Si salís ahora, se liberarán los asientos seleccionados.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí, salir",
        cancelButtonText: "No, quedarme",
        confirmButtonColor: "#ef4444",
        cancelButtonColor: "#22c55e",
        background: "#0f172a",
        color: "#f8fafc"
    });

    return result.isConfirmed;
}

function showPaymentSuccess(payment) {
    successMessage.innerHTML = `
        <div class="payment-success">
            <h3>Pago realizado correctamente</h3>
            <p>Serás redirigido al inicio...</p>
        </div>
    `;
    successMessage.classList.remove("hidden");
    setTimeout(() => {
        window.location.href = HOME_PAGE_URL;
    }, 1500);
}

async function payReservation() {
    if (!selectedSeats || selectedSeats.length === 0) {
        showError("Debe seleccionar al menos un asiento.");
        return;
    }

    if (!selectedSector) {
        showError("No se encontró el sector seleccionado.");
        return;
    }

    const user_email = getLoggedUser();

    if (!user_email) {
        showError("No se encontró el usuario logueado.");
        return;
    }

    // Validar que tengamos exactamente la cantidad deseada de asientos
    if (selectedSeats.length !== desiredSeatCount) {
        showError(`Debe seleccionar exactamente ${desiredSeatCount} asiento(s).`);
        return;
    }

    setLoading(true);
    hideMessages();

    try {

        const userRequest = await fetchWithAuth(`${API_BASE_URL}/users/by-email?email=${encodeURIComponent(user_email.email)}`, {
            method: "GET"
        });

        if (!userRequest) {
            return;
        }

        const userData = await parseJsonSafely(userRequest);

        if (!userRequest.ok) {
            throw new Error(userData?.message || userData?.detail || "No se pudo obtener la información del usuario.");
        }

        const user = userData || {};
        console.log("Usuario obtenido para pago:", user);

        const paymentRequest = {
            eventId: parseInt(eventId),
            sectorId: selectedSector.id,
            quantitySeat: selectedSeats.length,
            userId: user.id || user.Id || 0,
            amount: (selectedSector.price ?? 0) * selectedSeats.length,
            currency: "ARS"
        };

        const response = await fetchWithAuth(`${API_BASE_URL}/payments`, {
            method: "POST",
            body: JSON.stringify(paymentRequest)
        });

        if (!response) {
            return;
        }

        const data = await parseJsonSafely(response);

        if (!response.ok) {
            throw new Error(data?.message || data?.detail || "No se pudo procesar el pago.");
        }

        // Actualizar estado de todos los asientos a "Vendido"
        for (const seat of selectedSeats) {
            await patchSeatStatus(seat.id, "Vendido");
        }

        stopTimer();
        showPaymentSuccess(data);

        activeReservation = null;
        selectedSeats = [];
        seatsCount = 0;
        payButton.disabled = true;
        cancelButton.disabled = true;
        
    } catch (error) {
        console.error("[CODE-ERROR] - Error al pagar reserva:", error);
        showError(error.message || "No se pudo procesar el pago.");
    } finally {
        setLoading(false);
    }
}
