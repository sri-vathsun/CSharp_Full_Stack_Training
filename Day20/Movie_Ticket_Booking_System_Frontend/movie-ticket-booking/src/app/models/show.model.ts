export interface Show {
  showId: number;
  movieId: number;
  movieTitle: string;
  screenId: number;
  screenName: string;
  theatreId: number;
  theatreName: string;
  startTime: string;
  endTime: string;
  ticketPrice: number;
  totalSeats: number;
}

export interface CreateShowRequest {
  movieId: number;
  screenId: number;
  startTime: string;
  endTime: string;
  ticketPrice: number;
}

export interface UpdateShowRequest extends CreateShowRequest {}

