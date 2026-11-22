import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';

import { Observable } from 'rxjs';

import {
  CreateEventRequest,
  UpdateEventRequest,
  EventResponse
} from '../models/event.model';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  private readonly apiUrl =
    'https://localhost:7128/api/events';

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token =
      localStorage.getItem('token') ||
      localStorage.getItem('jwtToken') ||
      localStorage.getItem('accessToken');

    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  // Create event
  createEvent(
    event: CreateEventRequest
  ): Observable<EventResponse> {
    return this.http.post<EventResponse>(
      this.apiUrl,
      event,
      { headers: this.getHeaders() }
    );
  }

  // Get logged-in Organizer's events
  getMyEvents(): Observable<EventResponse[]> {
    return this.http.get<EventResponse[]>(
      `${this.apiUrl}/my`,
      { headers: this.getHeaders() }
    );
  }

  // Get event by ID
  getEventById(
    id: number
  ): Observable<EventResponse> {
    return this.http.get<EventResponse>(
      `${this.apiUrl}/${id}`,
      { headers: this.getHeaders() }
    );
  }

  // Update event
  updateEvent(
    id: number,
    event: UpdateEventRequest
  ): Observable<EventResponse> {
    return this.http.put<EventResponse>(
      `${this.apiUrl}/${id}`,
      event,
      { headers: this.getHeaders() }
    );
  }

  // Publish event
  publishEvent(
    id: number
  ): Observable<EventResponse> {
    return this.http.put<EventResponse>(
      `${this.apiUrl}/${id}/publish`,
      {},
      { headers: this.getHeaders() }
    );
  }

  // Delete event
  deleteEvent(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`,
      { headers: this.getHeaders() }
    );
  }
}