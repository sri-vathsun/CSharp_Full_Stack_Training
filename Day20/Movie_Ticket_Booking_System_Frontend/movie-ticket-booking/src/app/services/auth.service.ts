import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { LoginRequest, RegisterRequest, AuthResponse, JwtPayload } from '../models/auth.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  private currentUserSubject = new BehaviorSubject<JwtPayload | null>(this.decodeToken(this.getToken()));
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  login(dto: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/api/auth/login`, dto).pipe(
      tap(res => {
        if (res.token) {
          localStorage.setItem(this.TOKEN_KEY, res.token);
          this.currentUserSubject.next(this.decodeToken(res.token));
        }
      })
    );
  }

  register(dto: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/api/auth/register`, dto);
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    const payload = this.decodeToken(token);
    if (!payload) return false;
    return payload.exp * 1000 > Date.now();
  }

  getRole(): string | null {
    return this.currentUserSubject.value?.role ?? null;
  }

  isAdmin(): boolean {
    return this.getRole() === 'Admin';
  }

  isCustomer(): boolean {
    return this.getRole() === 'Customer';
  }

  getCurrentUser(): JwtPayload | null {
    return this.currentUserSubject.value;
  }

  private decodeToken(token: string | null): JwtPayload | null {
    if (!token) return null;
    try {
      const base64 = token.split('.')[1];
      const decoded = JSON.parse(atob(base64));
      // Map .NET claim types to short names
      return {
        nameid: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ?? decoded['nameid'],
        unique_name: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ?? decoded['unique_name'],
        email: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? decoded['email'],
        role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? decoded['role'],
        exp: decoded['exp']
      };
    } catch {
      return null;
    }
  }
}

