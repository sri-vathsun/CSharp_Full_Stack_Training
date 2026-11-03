import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Theatre, CreateTheatreRequest, UpdateTheatreRequest } from '../models/theatre.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TheatreService {
  private apiUrl = `${environment.apiUrl}/api/theatres`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Theatre[]> {
    return this.http.get<Theatre[]>(this.apiUrl);
  }

  getById(id: number): Observable<Theatre> {
    return this.http.get<Theatre>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateTheatreRequest): Observable<Theatre> {
    return this.http.post<Theatre>(this.apiUrl, request);
  }

  update(id: number, request: UpdateTheatreRequest): Observable<Theatre> {
    return this.http.put<Theatre>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

