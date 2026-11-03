import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Show } from '../../../models/show.model';
import { Movie } from '../../../models/movie.model';
import { Theatre } from '../../../models/theatre.model';
import { ShowService } from '../../../services/show.service';
import { MovieService } from '../../../services/movie.service';
import { TheatreService } from '../../../services/theatre.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-show-list',
  standalone: false,
  templateUrl: './show-list.component.html'
})
export class ShowListComponent implements OnInit {
  shows: Show[] = [];
  movies: Movie[] = [];
  theatres: Theatre[] = [];
  filterMovieId: number | null = null;
  filterTheatreId: number | null = null;
  loading = false;
  errorMessage = '';
  isAdmin = false;

  constructor(
    private showService: ShowService,
    private movieService: MovieService,
    private theatreService: TheatreService,
    private authService: AuthService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.movieService.getAll().subscribe((m: Movie[]) => this.movies = m);
    this.theatreService.getAll().subscribe((t: Theatre[]) => this.theatres = t);

    const movieId = this.route.snapshot.queryParamMap.get('movieId');
    const theatreId = this.route.snapshot.queryParamMap.get('theatreId');
    if (movieId) this.filterMovieId = +movieId;
    if (theatreId) this.filterTheatreId = +theatreId;
    this.load();
  }

  load(): void {
    this.loading = true;
    this.showService.getAll(
      undefined,
      this.filterMovieId ?? undefined,
      this.filterTheatreId ?? undefined
    ).subscribe({
      next: (data: Show[]) => { this.shows = data; this.loading = false; },
      error: () => { this.errorMessage = 'Failed to load shows.'; this.loading = false; }
    });
  }

  delete(id: number): void {
    if (!confirm('Delete this show?')) return;
    this.showService.delete(id).subscribe({
      next: () => this.shows = this.shows.filter(s => s.showId !== id),
      error: (err: any) => alert(err.error?.message ?? 'Cannot delete show.')
    });
  }
}
