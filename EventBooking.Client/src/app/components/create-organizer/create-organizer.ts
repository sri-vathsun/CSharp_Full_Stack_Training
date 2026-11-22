import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-create-organizer',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './create-organizer.html',
  styleUrl: './create-organizer.css'
})
export class CreateOrganizer {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);

  errorMessage = '';
  successMessage = '';
  isLoading = false;

  organizerForm = this.fb.nonNullable.group({
    fullName: ['', [
      Validators.required,
      Validators.maxLength(100)
    ]],
    email: ['', [
      Validators.required,
      Validators.email
    ]],
    password: ['', [
      Validators.required,
      Validators.minLength(8)
    ]]
  });

  onSubmit(): void {
    if (this.organizerForm.invalid) {
      this.organizerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.userService.createOrganizer(this.organizerForm.getRawValue())
      .subscribe({
        next: (response) => {
          this.isLoading = false;
          this.successMessage =
            `Organizer ${response.fullName} created successfully.`;
          this.organizerForm.reset();
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage =
            error.error?.message ||
            error.error?.title ||
            'Unable to create organizer. Please try again.';
        }
      });
  }
}
