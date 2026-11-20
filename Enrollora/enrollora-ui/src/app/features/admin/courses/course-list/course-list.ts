import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { CourseService } from '../course.service';
import { CourseResponse } from '../course.models';

@Component({
  selector: 'app-course-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './course-list.html',
  styleUrl: './course-list.css',
})
export class CourseListComponent implements OnInit {
  private courseService = inject(CourseService);
  private formBuilder = inject(FormBuilder);

  courses = signal<CourseResponse[]>([]);

  isLoading = signal(false);
  errorMessage = signal('');
  isAddModalOpen = signal(false);
  isSaving = signal(false);
  saveErrorMessage = signal('');

  pageNumber = signal(1);
  pageSize = signal(10);

  totalRecords = signal(0);
  totalPages = signal(0);

  courseForm = this.formBuilder.nonNullable.group({
    courseName: ['', [Validators.required, Validators.maxLength(150)]],
    duration: ['', [Validators.required, Validators.maxLength(80)]],
    fees: [0, [Validators.required, Validators.min(0)]],
    capacity: [1, [Validators.required, Validators.min(1), Validators.max(100000)]],
    isActive: [true],
  });

  ngOnInit(): void {
    this.loadCourses();
  }

  // Load courses from backend
  loadCourses(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.courseService.getCourses(this.pageNumber(), this.pageSize()).subscribe({
      next: (response) => {
        this.courses.set(response.items);

        this.totalRecords.set(response.totalRecords);
        this.totalPages.set(response.totalPages);

        this.isLoading.set(false);
      },

      error: (error) => {
        console.error('Error loading courses:', error);

        this.errorMessage.set('Unable to load courses.');

        this.isLoading.set(false);
      },
    });
  }

  openAddCourseModal(): void {
    this.courseForm.reset({
      courseName: '',
      duration: '',
      fees: 0,
      capacity: 1,
      isActive: true,
    });
    this.saveErrorMessage.set('');
    this.isAddModalOpen.set(true);
  }

  closeAddCourseModal(): void {
    if (!this.isSaving()) {
      this.isAddModalOpen.set(false);
    }
  }

  addCourse(): void {
    if (this.courseForm.invalid) {
      this.courseForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.saveErrorMessage.set('');

    this.courseService.createCourse(this.courseForm.getRawValue()).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.isAddModalOpen.set(false);
        this.loadCourses();
      },
      error: (error) => {
        console.error('Error adding course:', error);
        this.saveErrorMessage.set(error?.error?.detail ?? 'Unable to add course.');
        this.isSaving.set(false);
      },
    });
  }

  nextPage(): void {
    if (this.pageNumber() < this.totalPages()) {
      this.pageNumber.update((page) => page + 1);

      this.loadCourses();
    }
  }

  // Navigate to previous page
  previousPage(): void {
    if (this.pageNumber() > 1) {
      this.pageNumber.update((page) => page - 1);

      this.loadCourses();
    }
  }
}
