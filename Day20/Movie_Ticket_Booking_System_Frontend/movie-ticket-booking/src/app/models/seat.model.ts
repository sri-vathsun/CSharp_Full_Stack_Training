export interface Seat {
  seatId: number;
  screenId: number;
  screenName: string;
  rowName: string;
  seatNumber: number;
  seatLabel: string;
  seatType: string;
}

export interface CreateSeatRequest {
  screenId: number;
  rowName: string;
  seatNumber: number;
  seatType: string;
}

export interface UpdateSeatRequest extends CreateSeatRequest {}

export interface CreateSeatItem {
  rowName: string;
  seatNumber: number;
  seatType: string;
}

export interface BulkCreateSeatsRequest {
  screenId: number;
  seats: CreateSeatItem[];
}

export interface ShowSeat {
  seatId: number;
  rowName: string;
  seatNumber: number;
  seatLabel: string;
  seatType: string;
  price: number;
  status: 'Available' | 'Booked';
}

