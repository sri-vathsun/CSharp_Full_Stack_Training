export interface Booking {
  bookingId: number;
  bookingReference: string;
  showId: number;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  bookingDate: string;
  totalAmount: number;
  status: string;
  seats: string[];
}

export interface CreateBookingRequest {
  customerPhone: string;
  showId: number;
  seatIds: number[];
}

