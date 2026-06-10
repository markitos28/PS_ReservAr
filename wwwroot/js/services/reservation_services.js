import { fetchWithAuth } from './api_services.js';

export const ReservationService = {
    getEvent: (id) => fetchWithAuth(`/events/${id}`).then(r => r.json()),
    
    getSectors: (eventId) => fetchWithAuth(`/sectors?eventId=${eventId}`).then(r => r.json()),
    
    getSeats: (sectorId) => fetchWithAuth(`/seats?sectorId=${sectorId}`).then(r => r.json()),
    
    createReservation: (userId, seatId) => fetchWithAuth('/reservations', {
        method: 'POST',
        body: JSON.stringify({ userId, seatId })
    }),

    processPayment: (paymentData) => fetchWithAuth('/payments', {
        method: 'POST',
        body: JSON.stringify(paymentData)
    }).then(r=> r.json())
};