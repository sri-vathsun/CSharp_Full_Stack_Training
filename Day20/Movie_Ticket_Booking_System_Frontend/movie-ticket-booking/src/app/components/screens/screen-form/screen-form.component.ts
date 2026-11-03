import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ScreenService } from '../../../services/screen.service';
import { TheatreService } from '../../../services/theatre.service';
import { Theatre } from '../../../models/theatre.model';
import { Screen } from '../../../models/screen.model';

@Component({
  selector: 'app-screen-form',
  standalone: false,
  templateUrl: './screen-form.component.html'
})
export class ScreenFormComponent implements OnInit {
  form: FormGroup;
  isEdit = false;
  screenId: number | null = null;
  theatres: Theatre[] = [];
  loading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private screenService: ScreenService,
    private theatreService: TheatreService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      theatreId: ['', Validators.required],
      screenName: ['', [Validators.required, Validators.maxLength(50)]]
    });
  }

  ngOnInit(): void {
    this.theatreService.getAll().subscribe((t: Theatre[]) => this.theatres = t);
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.screenId = +id;
      this.screenService.getById(this.screenId).subscribe((s: Screen) => this.form.patchValue(s));
    }
  }

  submit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    const value = { ...this.form.value, theatreId: +this.form.value.theatreId };
    const action = this.isEdit
      ? this.screenService.update(this.screenId!, value)
      : this.screenService.create(value);
    action.subscribe({
      next: () => this.router.navigate(['/admin/screens']),
      error: (err: any) => { this.errorMessage = err.error?.message ?? 'Failed to save.'; this.loading = false; }
    });
  }
}
