import { Component, OnInit } from '@angular/core';
import { Movie } from '../../../models/movie.model';
import { MovieService } from '../../../services/movie.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-movie-list',
  standalone: false,
  templateUrl: './movie-list.component.html'
})
export class MovieListComponent implements OnInit {
  movies: Movie[] = [];
  loading = false;
  errorMessage = '';
  searchTitle = '';
  isAdmin = false;

  constructor(private movieService: MovieService, private authService: AuthService) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.load();
  }

  load(): void {
    this.loading = true;
    this.movieService.getAll(1, 100, this.searchTitle || undefined).subscribe({
      next: (data: Movie[]) => { this.movies = data; this.loading = false; },
      error: () => { this.errorMessage = 'Failed to load movies.'; this.loading = false; }
    });
  }

  search(): void {
    this.load();
  }

  delete(id: number): void {
    if (!confirm('Delete this movie?')) return;
    this.movieService.delete(id).subscribe({
      next: () => this.movies = this.movies.filter(m => m.movieId !== id),
      error: (err: any) => alert(err.error?.message ?? 'Cannot delete movie.')
    });
  }
}
