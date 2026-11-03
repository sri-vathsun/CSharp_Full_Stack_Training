import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Show, CreateShowRequest, UpdateShowRequest } from '../models/show.model';
import { ShowSeat } from '../models/seat.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ShowService {
  private apiUrl = `${environment.apiUrl}/api/shows`;

  constructor(private http: HttpClient) {}

  getAll(date?: string, movieId?: number, theatreId?: number): Observable<Show[]> {
    let params = new HttpParams();
    if (date) params = params.set('date', date);
    if (movieId) params = params.set('movieId', movieId);
    if (theatreId) params = params.set('theatreId', theatreId);
    return this.http.get<Show[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<Show> {
    return this.http.get<Show>(`${this.apiUrl}/${id}`);
  }

  getByMovie(movieId: number): Observable<Show[]> {
    return this.http.get<Show[]>(`${this.apiUrl}/movie/${movieId}`);
  }

  getByTheatre(theatreId: number): Observable<Show[]> {
    return this.http.get<Show[]>(`${this.apiUrl}/theatre/${theatreId}`);
  }

  getSeats(showId: number): Observable<ShowSeat[]> {
    return this.http.get<ShowSeat[]>(`${this.apiUrl}/${showId}/seats`);
  }

  getAvailableSeats(showId: number): Observable<ShowSeat[]> {
    return this.http.get<ShowSeat[]>(`${this.apiUrl}/${showId}/seats/available`);
  }

  getBookedSeats(showId: number): Observable<ShowSeat[]> {
    return this.http.get<ShowSeat[]>(`${this.apiUrl}/${showId}/seats/booked`);
  }

  create(request: CreateShowRequest): Observable<Show> {
    return this.http.post<Show>(this.apiUrl, request);
  }

  update(id: number, request: UpdateShowRequest): Observable<Show> {
    return this.http.put<Show>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

