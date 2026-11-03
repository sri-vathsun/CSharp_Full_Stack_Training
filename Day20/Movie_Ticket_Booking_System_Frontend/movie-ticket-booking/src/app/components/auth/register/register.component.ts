import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  form: FormGroup;
  errorMessage = '';
  successMessage = '';
  loading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.form = this.fb.group({
      fullName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(100)]]
    });
  }

  submit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    this.errorMessage = '';
    this.authService.register(this.form.value).subscribe({
      next: (res: any) => {
        this.successMessage = res.message ?? 'Registration successful!';
        setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      error: (err: any) => {
        if (err.error?.message) {
          this.errorMessage = err.error.message;
        } else if (err.error?.errors) {
          const firstKey = Object.keys(err.error.errors)[0];
          this.errorMessage = err.error.errors[firstKey][0];
        } else {
          this.errorMessage = 'Registration failed. Please try again.';
        }
        this.loading = false;
      }
    });
  }
}
