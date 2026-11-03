import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Movie, CreateMovieRequest, UpdateMovieRequest } from '../models/movie.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MovieService {
  private apiUrl = `${environment.apiUrl}/api/movies`;

  constructor(private http: HttpClient) {}

  getAll(pageNumber = 1, pageSize = 10, title?: string): Observable<Movie[]> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);
    if (title) params = params.set('title', title);
    return this.http.get<Movie[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<Movie> {
    return this.http.get<Movie>(`${this.apiUrl}/${id}`);
  }

  search(title: string): Observable<Movie[]> {
    return this.http.get<Movie[]>(`${this.apiUrl}/search`, {
      params: new HttpParams().set('title', title)
    });
  }

  create(request: CreateMovieRequest): Observable<Movie> {
    return this.http.post<Movie>(this.apiUrl, request);
  }

  update(id: number, request: UpdateMovieRequest): Observable<Movie> {
    return this.http.put<Movie>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

