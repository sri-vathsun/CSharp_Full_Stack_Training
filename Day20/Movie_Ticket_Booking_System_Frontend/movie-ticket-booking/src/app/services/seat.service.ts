import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Seat, CreateSeatRequest, UpdateSeatRequest, BulkCreateSeatsRequest, ShowSeat } from '../models/seat.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SeatService {
  private apiUrl = `${environment.apiUrl}/api/seats`;

  constructor(private http: HttpClient) {}

  getById(id: number): Observable<Seat> {
    return this.http.get<Seat>(`${this.apiUrl}/${id}`);
  }

  getByScreen(screenId: number): Observable<Seat[]> {
    return this.http.get<Seat[]>(`${this.apiUrl}/screen/${screenId}`);
  }

  create(request: CreateSeatRequest): Observable<Seat> {
    return this.http.post<Seat>(this.apiUrl, request);
  }

  bulkCreate(request: BulkCreateSeatsRequest): Observable<Seat[]> {
    return this.http.post<Seat[]>(`${this.apiUrl}/bulk`, request);
  }

  update(id: number, request: UpdateSeatRequest): Observable<Seat> {
    return this.http.put<Seat>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

