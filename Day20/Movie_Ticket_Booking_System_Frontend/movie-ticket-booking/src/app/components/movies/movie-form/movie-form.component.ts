import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MovieService } from '../../../services/movie.service';
import { Movie } from '../../../models/movie.model';

@Component({
  selector: 'app-movie-form',
  standalone: false,
  templateUrl: './movie-form.component.html'
})
export class MovieFormComponent implements OnInit {
  form: FormGroup;
  isEdit = false;
  movieId: number | null = null;
  loading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private movieService: MovieService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(150)]],
      description: ['', Validators.maxLength(500)],
      durationMinutes: [90, [Validators.required, Validators.min(1), Validators.max(1000)]],
      releaseDate: ['', Validators.required],
      language: ['', [Validators.required, Validators.maxLength(50)]],
      genre: ['', [Validators.required, Validators.maxLength(50)]]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.movieId = +id;
      this.movieService.getById(this.movieId).subscribe((movie: Movie) => {
        this.form.patchValue({
          ...movie,
          releaseDate: movie.releaseDate
        });
      });
    }
  }

  submit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    const action = this.isEdit
      ? this.movieService.update(this.movieId!, this.form.value)
      : this.movieService.create(this.form.value);

    action.subscribe({
      next: () => this.router.navigate(['/movies']),
      error: (err: any) => {
        this.errorMessage = err.error?.message ?? 'Failed to save movie.';
        this.loading = false;
      }
    });
  }
}
