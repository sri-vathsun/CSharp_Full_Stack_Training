import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Booking, CreateBookingRequest } from '../models/booking.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private apiUrl = `${environment.apiUrl}/api/bookings`;

  constructor(private http: HttpClient) {}

  getAll(showId?: number, status?: string): Observable<Booking[]> {
    let params = new HttpParams();
    if (showId) params = params.set('showId', showId);
    if (status) params = params.set('status', status);
    return this.http.get<Booking[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<Booking> {
    return this.http.get<Booking>(`${this.apiUrl}/${id}`);
  }

  getByShow(showId: number): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${this.apiUrl}/show/${showId}`);
  }

  create(request: CreateBookingRequest): Observable<Booking> {
    return this.http.post<Booking>(this.apiUrl, request);
  }

  cancel(id: number): Observable<Booking> {
    return this.http.put<Booking>(`${this.apiUrl}/${id}/cancel`, {});
  }
}

