import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ShowService } from '../../../services/show.service';
import { MovieService } from '../../../services/movie.service';
import { ScreenService } from '../../../services/screen.service';
import { Movie } from '../../../models/movie.model';
import { Screen } from '../../../models/screen.model';
import { Show } from '../../../models/show.model';

@Component({
  selector: 'app-show-form',
  standalone: false,
  templateUrl: './show-form.component.html'
})
export class ShowFormComponent implements OnInit {
  form: FormGroup;
  isEdit = false;
  showId: number | null = null;
  movies: Movie[] = [];
  screens: Screen[] = [];
  loading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private showService: ShowService,
    private movieService: MovieService,
    private screenService: ScreenService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      movieId: ['', Validators.required],
      screenId: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
      ticketPrice: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.movieService.getAll().subscribe((m: Movie[]) => this.movies = m);
    this.screenService.getAll().subscribe((s: Screen[]) => this.screens = s);

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.showId = +id;
      this.showService.getById(this.showId).subscribe((show: Show) => {
        this.form.patchValue({
          ...show,
          startTime: this.toLocalDateTimeInput(show.startTime),
          endTime: this.toLocalDateTimeInput(show.endTime)
        });
      });
    }
  }

  toLocalDateTimeInput(dt: string): string {
    const d = new Date(dt);
    return new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
  }

  submit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    const val = {
      ...this.form.value,
      movieId: +this.form.value.movieId,
      screenId: +this.form.value.screenId,
      ticketPrice: +this.form.value.ticketPrice,
      startTime: new Date(this.form.value.startTime).toISOString(),
      endTime: new Date(this.form.value.endTime).toISOString()
    };
    const action = this.isEdit
      ? this.showService.update(this.showId!, val)
      : this.showService.create(val);
    action.subscribe({
      next: () => this.router.navigate(['/shows']),
      error: (err: any) => { this.errorMessage = err.error?.message ?? 'Failed to save show.'; this.loading = false; }
    });
  }
}
