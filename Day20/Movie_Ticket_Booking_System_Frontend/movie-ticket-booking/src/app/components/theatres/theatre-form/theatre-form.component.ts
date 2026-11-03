import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TheatreService } from '../../../services/theatre.service';
import { Theatre } from '../../../models/theatre.model';

@Component({
  selector: 'app-theatre-form',
  standalone: false,
  templateUrl: './theatre-form.component.html'
})
export class TheatreFormComponent implements OnInit {
  form: FormGroup;
  isEdit = false;
  theatreId: number | null = null;
  loading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private theatreService: TheatreService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(150)]],
      location: ['', [Validators.required, Validators.maxLength(250)]]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.theatreId = +id;
      this.theatreService.getById(this.theatreId).subscribe((t: Theatre) => this.form.patchValue(t));
    }
  }

  submit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    const action = this.isEdit
      ? this.theatreService.update(this.theatreId!, this.form.value)
      : this.theatreService.create(this.form.value);
    action.subscribe({
      next: () => this.router.navigate(['/admin/theatres']),
      error: (err: any) => { this.errorMessage = err.error?.message ?? 'Failed to save.'; this.loading = false; }
    });
  }
}
