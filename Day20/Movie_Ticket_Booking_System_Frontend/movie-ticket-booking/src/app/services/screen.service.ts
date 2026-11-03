import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Screen, CreateScreenRequest, UpdateScreenRequest } from '../models/screen.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ScreenService {
  private apiUrl = `${environment.apiUrl}/api/screens`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Screen[]> {
    return this.http.get<Screen[]>(this.apiUrl);
  }

  getById(id: number): Observable<Screen> {
    return this.http.get<Screen>(`${this.apiUrl}/${id}`);
  }

  getByTheatre(theatreId: number): Observable<Screen[]> {
    return this.http.get<Screen[]>(`${this.apiUrl}/theatre/${theatreId}`);
  }

  create(request: CreateScreenRequest): Observable<Screen> {
    return this.http.post<Screen>(this.apiUrl, request);
  }

  update(id: number, request: UpdateScreenRequest): Observable<Screen> {
    return this.http.put<Screen>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

