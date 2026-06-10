/**
 * UI Manager - Encargado de la manipulación del DOM y renderizado
 */
export const UIManager = {
    // Referencias a elementos del DOM
    elements: {
        eventTitle: document.getElementById("eventTitle"),
        sectorsContainer: document.getElementById("sectorsContainer"),
        seatsContainer: document.getElementById("seatsContainer"),
        selectedSectorLabel: document.getElementById("selectedSectorLabel"),
        loadingMessage: document.getElementById("loadingMessage"),
        errorMessage: document.getElementById("errorMessage"),
        successMessage: document.getElementById("successMessage"),
        seatQuantityInput: document.getElementById("seatQuantityInput"),
        quantityHelperText: document.getElementById("quantityHelperText"),
        payButton: document.getElementById("payButton"),
        cancelButton: document.getElementById("cancelButton"),
        timerBox: document.getElementById("timerBox"),
        timerValue: document.getElementById("timerValue"),
        decreaseQuantityBtn: document.getElementById("decreaseQuantityBtn"),
        increaseQuantityBtn: document.getElementById("increaseQuantityBtn"),
        quantityHelperText: document.getElementById("quantityHelperText"),
    },

    setLoading: (isLoading) => {
        UIManager.elements.loadingMessage.classList.toggle("hidden", !isLoading);
    },

    hideMessages: () => {
        const { errorMessage, successMessage } = UIManager.elements;
        errorMessage.classList.add("hidden");
        successMessage.classList.add("hidden");
        errorMessage.textContent = "";
        successMessage.innerHTML = "";
    },

    showError: (message) => {
        UIManager.elements.errorMessage.textContent = message;
        UIManager.elements.errorMessage.classList.remove("hidden");
    },

    updateTimer: (remainingSeconds) => {
        const { timerBox, timerValue } = UIManager.elements;
        if (remainingSeconds <= 0) {
            timerBox.classList.add("hidden");
            return;
        }
        timerBox.classList.remove("hidden");
        const minutes = Math.floor(remainingSeconds / 60);
        const seconds = remainingSeconds % 60;
        timerValue.textContent = `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;
    },

    renderSectorsList: (sectors, selectedSectorId, onSelect) => {
        const container = UIManager.elements.sectorsContainer;
        container.innerHTML = "";

        if (!sectors.length) {
            container.innerHTML = "<p>No hay sectores disponibles.</p>";
            return;
        }

        const ordered = [...sectors].sort((a, b) => a.name.toLowerCase().includes("baja") ? -1 : 1);

        ordered.forEach(sector => {
            const card = document.createElement("article");
            card.className = `sector-card ${Number(selectedSectorId) === Number(sector.id) ? 'active' : ''}`;
            card.innerHTML = `
                <h3>${UIManager.escapeHtml(sector.name)}</h3>
                <p>Precio: $${sector.price}</p>
                <p>Capacidad: ${sector.capacity}</p>
            `;
            card.onclick = () => onSelect(sector);
            container.appendChild(card);
        });
    },

    renderSeatsMatrix: (sectors, selectedSeats, onSeatClick) => {
        const container = UIManager.elements.seatsContainer;
        container.innerHTML = "";

        sectors.forEach(sector => {
            const sectorBlock = document.createElement("section");
            sectorBlock.className = "sector-matrix";
            sectorBlock.dataset.sectorId = sector.id;

            const title = document.createElement("h3");
            title.textContent = sector.name;

            const grid = document.createElement("div");
            grid.className = "seats-grid";

            const seats = [...(sector.seats || [])].sort((a, b) => Number(a.seatNumber) - Number(b.seatNumber));
            
            // Renderizado por filas (lógica original)
            for (let i = 0; i < seats.length; i += 10) {
                const rowDiv = document.createElement("div");
                rowDiv.className = "seat-row";
                
                seats.slice(i, i + 10).forEach(seat => {
                    const btn = document.createElement("button");
                    const status = UIManager.normalizeStatus(seat.status);
                    const isSelected = selectedSeats.some(s => s.id === seat.id);

                    btn.className = `seat ${status} ${isSelected ? 'selected' : ''}`;
                    btn.textContent = seat.seatNumber;
                    btn.disabled = status === "occupied";
                    btn.onclick = () => onSeatClick(seat);
                    rowDiv.appendChild(btn);
                });
                grid.appendChild(rowDiv);
            }

            sectorBlock.append(title, grid);
            container.appendChild(sectorBlock);
        });
    },

    updateQuantityHelper: (desiredCount) => {
        const text = desiredCount === 1 
            ? "Seleccioná 1 asiento en el mapa" 
            : `Seleccioná exactamente ${desiredCount} asientos en el mapa`;
        UIManager.elements.quantityHelperText.textContent = text;
    },

    showPaymentSuccess: () => {
        const { successMessage } = UIManager.elements;
        successMessage.innerHTML = `
            <div class="payment-success">
                <h3>Pago realizado correctamente</h3>
                <p>Serás redirigido al inicio...</p>
            </div>
        `;
        successMessage.classList.remove("hidden");
    },

    updateFooterUI: (count, desired, hasSelection) => {
        const { selectedSectorLabel, payButton, cancelButton } = UIManager.elements;
        cancelButton.disabled = !hasSelection;

        if (count === 0) {
            selectedSectorLabel.textContent = "Elegí un sector para ver sus butacas.";
            payButton.disabled = true;
        } else if (count === desired) {
            selectedSectorLabel.textContent = `${count} asiento(s) - Listo para pagar`;
            payButton.disabled = false;
        } else {
            selectedSectorLabel.textContent = `${count} de ${desired} asiento(s) seleccionado(s)`;
            payButton.disabled = true;
        }
    },

    normalizeStatus: (status) => {
        const val = String(status || "").toLowerCase();
        return (val.includes("vendido") || val.includes("ocupado") || val.includes("reservado")) ? "occupied" : "available";
    },

    escapeHtml: (str) => {
        const div = document.createElement('div');
        div.textContent = str;
        return div.innerHTML;
    }
};