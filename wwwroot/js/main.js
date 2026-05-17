import { AuthManager } from './auth/auth_manager.js';
import { ReservationService } from './services/reservation_services.js';
import { UIManager } from './ui/render_ui.js';

// Estado global de la vista
let state = {
    eventId: new URLSearchParams(window.location.search).get("eventId"),
    sectors: [],
    selectedSector: null,
    selectedSeats: [],
    desiredCount: 1,
    activeReservation: null,
    timerInterval: null
};

async function init() {
    if (!state.eventId) {
        UIManager.showError("No se recibió el evento seleccionado.");
        return;
    }

    setupEventListeners();
    await loadData();
}

async function loadData() {
    UIManager.setLoading(true);
    try {
        const event = await ReservationService.getEvent(state.eventId);
        UIManager.elements.eventTitle.textContent = `${event.name} - ${event.venue}`;

        // Limpiar expirados y cargar sectores
        await ReservationService.clearExpired();
        state.sectors = await ReservationService.getSectors(state.eventId);

        // Cargar asientos para cada sector
        for (let sector of state.sectors) {
            sector.seats = await ReservationService.getSeats(sector.id);
        }

        refreshUI();
    } catch (error) {
        UIManager.showError("Error al cargar la información del evento.");
    } finally {
        UIManager.setLoading(false);
    }
}

function refreshUI() {
    UIManager.renderSectorsList(state.sectors, state.selectedSector?.id, handleSectorSelect);
    UIManager.renderSeatsMatrix(state.sectors, state.selectedSeats, handleSeatClick);
    UIManager.updateFooterUI(state.selectedSeats.length, state.desiredCount, state.selectedSeats.length > 0);
}

// Handlers de Interacción
async function handleSectorSelect(sector) {
    state.selectedSector = sector;
    refreshUI();
    // Scroll suave al sector
    document.querySelector(`[data-sector-id="${sector.id}"]`)?.scrollIntoView({ behavior: "smooth", block: "center" });
}

async function handleSeatClick(seat) {
    const isAlreadySelected = state.selectedSeats.some(s => s.id === seat.id);

    if (isAlreadySelected) {
        state.selectedSeats = state.selectedSeats.filter(s => s.id !== seat.id);
    } else {
        if (state.selectedSeats.length >= state.desiredCount) {
            UIManager.showError(`Máximo de ${state.desiredCount} asientos alcanzado.`);
            return;
        }

        const user = AuthManager.getLoggedUser();
        const success = await tryReserve(user.id, seat);
        if (success) state.selectedSeats.push(seat);
    }
    refreshUI();
}

async function tryReserve(userId, seat) {
    try {
        const response = await ReservationService.createReservation(userId, seat.id);
        if (response.status === 409) {
            Swal.fire({ icon: 'error', title: 'Asiento no disponible', toast: true, position: 'top-end' });
            await loadData(); // Recargar mapa
            return false;
        }
        state.activeReservation = await response.json();
        startTimer();
        return true;
    } catch (e) {
        UIManager.showError("No se pudo procesar la reserva.");
        return false;
    }
}

function startTimer() {
    if (state.timerInterval) clearInterval(state.timerInterval);
    let remaining = 300;
    state.timerInterval = setInterval(() => {
        remaining--;
        UIManager.updateTimer(remaining);
        if (remaining <= 0) {
            clearInterval(state.timerInterval);
            location.reload(); // Simplificado: recargar al expirar
        }
    }, 1000);
}

function setupEventListeners() {
    // Cantidad de asientos
    UIManager.elements.seatQuantityInput.addEventListener("change", (e) => {
        state.desiredCount = parseInt(e.target.value) || 1;
        state.selectedSeats = []; // Resetear selección al cambiar cantidad
        refreshUI();
    });

    // Botón Pago
    UIManager.elements.payButton.onclick = async () => {
        UIManager.setLoading(true);
        try {
            const user = AuthManager.getLoggedUser();
            const paymentData = {
                eventId: parseInt(state.eventId),
                sectorId: state.selectedSector.id,
                quantitySeat: state.selectedSeats.length,
                userId: user.id,
                reservationId: state.activeReservation.id,
                amount: state.selectedSector.price * state.selectedSeats.length,
                currency: "ARS"
            };
            await ReservationService.processPayment(paymentData);
            Swal.fire("¡Éxito!", "Pago procesado", "success").then(() => window.location.href = './index.html');
        } catch (e) {
            UIManager.showError("Error en el pago.");
        } finally {
            UIManager.setLoading(false);
        }
    };

    // Botón Incrementar (+)
    UIManager.elements.increaseQuantityBtn.onclick = () => {
        const currentValue = parseInt(UIManager.elements.seatQuantityInput.value) || 1;
        if (currentValue < 20) {
            const newValue = currentValue + 1;
            UIManager.elements.seatQuantityInput.value = newValue;
            updateQuantityLogic(newValue);
        }
    };

    // Botón Decrementar (-)
    UIManager.elements.decreaseQuantityBtn.onclick = () => {
        const currentValue = parseInt(UIManager.elements.seatQuantityInput.value) || 1;
        if (currentValue > 1) {
            const newValue = currentValue - 1;
            UIManager.elements.seatQuantityInput.value = newValue;
            updateQuantityLogic(newValue);
        }
    };

    // Listener para el input manual
    UIManager.elements.seatQuantityInput.oninput = (e) => {
        let val = parseInt(e.target.value);
        if (val > 20) val = 20;
        if (val < 1 || isNaN(val)) val = 1;
        e.target.value = val;
        updateQuantityLogic(val);
    };

    // Función auxiliar para no repetir código
    function updateQuantityLogic(count) {
        state.desiredCount = count;
        state.selectedSeats = []; // Limpiamos selección como hacías en el original
        UIManager.updateQuantityHelper(count);
        refreshUI();
    }

    // Logout
    document.getElementById("logoutButton").onclick = () => {
        AuthManager.logout();
        window.location.href = './login.html';
    };
}

document.addEventListener("DOMContentLoaded", init);