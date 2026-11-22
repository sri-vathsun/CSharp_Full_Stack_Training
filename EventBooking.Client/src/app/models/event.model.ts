export interface CreateEventRequest {
  title: string;
  description: string;
  venue: string;
  startDateUtc: string;
  endDateUtc: string;
  ticketPrice: number;
  totalTickets: number;
}

export interface UpdateEventRequest {
  title: string;
  description: string;
  venue: string;
  startDateUtc: string;
  endDateUtc: string;
  ticketPrice: number;
  totalTickets: number;
}

export interface EventResponse {
  id: number;
  title: string;
  description: string;
  venue: string;
  startDateUtc: string;
  endDateUtc: string;
  ticketPrice: number;
  totalTickets: number;
  availableTickets: number;
  status: string;
  organizerId: number;
  organizerName: string;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
}