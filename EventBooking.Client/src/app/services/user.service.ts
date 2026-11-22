import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AuthService } from './auth';
import {
  CreateOrganizerRequest,
  CreateOrganizerResponse
} from '../models/create-organizer.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private apiUrl = 'https://localhost:7128/api/users';

  createOrganizer(data: CreateOrganizerRequest): Observable<CreateOrganizerResponse> {
    const token = this.authService.getToken();
    const headers = token
      ? new HttpHeaders().set('Authorization', `Bearer ${token}`)
      : new HttpHeaders();

    return this.http.post<CreateOrganizerResponse>(
      `${this.apiUrl}/create-organizer`,
      data,
      { headers }
    );
  }
}
