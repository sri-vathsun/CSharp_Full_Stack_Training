import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { EventService } from '../../services/event.service';

import {
  CreateEventRequest,
  EventResponse,
  UpdateEventRequest
} from '../../models/event.model';

@Component({
  selector: 'app-organizer-events',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './organizer-events.component.html',
  styleUrl: './organizer-events.component.css'
})
export class OrganizerEventsComponent implements OnInit {

  events: EventResponse[] = [];

  eventForm!: FormGroup;

  isLoading = false;
  isSaving = false;
  isEditing = false;

  selectedEventId: number | null = null;

  successMessage = '';
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private eventService: EventService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.loadEvents();
  }

  private initializeForm(): void {
    this.eventForm = this.fb.group({
      title: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(150)
        ]
      ],

      description: [
        '',
        [
          Validators.required,
          Validators.minLength(10),
          Validators.maxLength(2000)
        ]
      ],

      venue: [
        '',
        [
          Validators.required,
          Validators.maxLength(250)
        ]
      ],

      startDateUtc: [
        '',
        Validators.required
      ],

      endDateUtc: [
        '',
        Validators.required
      ],

      ticketPrice: [
        0,
        [
          Validators.required,
          Validators.min(0),
          Validators.max(1000000)
        ]
      ],

      totalTickets: [
        100,
        [
          Validators.required,
          Validators.min(1),
          Validators.max(100000)
        ]
      ]
    });
  }

  loadEvents(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.eventService.getMyEvents().subscribe({
      next: (response) => {
        this.events = response;
        this.isLoading = false;
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage =
          this.getErrorMessage(
            error,
            'Unable to load events. Please try again.'
          );

        this.isLoading = false;
      }
    });
  }

  get totalEvents(): number {
    return this.events.length;
  }

  get publishedEvents(): number {
    return this.events.filter(
      event => event.status === 'Published'
    ).length;
  }

  get draftEvents(): number {
    return this.events.filter(
      event => event.status === 'Draft'
    ).length;
  }

  get totalAvailableTickets(): number {
    return this.events.reduce(
      (total, event) =>
        total + event.availableTickets,
      0
    );
  }

  submitEvent(): void {
    this.clearMessages();

    if (this.eventForm.invalid) {
      this.eventForm.markAllAsTouched();
      return;
    }

    const formValue = this.eventForm.value;

    const startDate = new Date(formValue.startDateUtc);
    const endDate = new Date(formValue.endDateUtc);

    if (endDate <= startDate) {
      this.errorMessage =
        'The event end date must be after the start date.';
      return;
    }

    if (startDate <= new Date()) {
      this.errorMessage =
        'The event start date must be in the future.';
      return;
    }

    const eventData: CreateEventRequest = {
      title: formValue.title.trim(),
      description: formValue.description.trim(),
      venue: formValue.venue.trim(),
      startDateUtc: startDate.toISOString(),
      endDateUtc: endDate.toISOString(),
      ticketPrice: Number(formValue.ticketPrice),
      totalTickets: Number(formValue.totalTickets)
    };

    this.isSaving = true;

    if (this.isEditing && this.selectedEventId !== null) {
      this.updateExistingEvent(
        this.selectedEventId,
        eventData
      );
    } else {
      this.createNewEvent(eventData);
    }
  }

  private createNewEvent(
    eventData: CreateEventRequest
  ): void {
    this.eventService.createEvent(eventData).subscribe({
      next: () => {
        this.successMessage =
          'Event created successfully!';

        this.isSaving = false;

        this.resetForm();
        this.loadEvents();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage =
          this.getErrorMessage(
            error,
            'Failed to create event.'
          );

        this.isSaving = false;
      }
    });
  }

  private updateExistingEvent(
    id: number,
    eventData: UpdateEventRequest
  ): void {
    this.eventService.updateEvent(
      id,
      eventData
    ).subscribe({
      next: () => {
        this.successMessage =
          'Event updated successfully!';

        this.isSaving = false;

        this.resetForm();
        this.loadEvents();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage =
          this.getErrorMessage(
            error,
            'Failed to update event.'
          );

        this.isSaving = false;
      }
    });
  }

  editEvent(event: EventResponse): void {
    this.clearMessages();

    this.isEditing = true;
    this.selectedEventId = event.id;

    this.eventForm.patchValue({
      title: event.title,
      description: event.description,
      venue: event.venue,

      startDateUtc: this.toLocalDateTime(
        event.startDateUtc
      ),

      endDateUtc: this.toLocalDateTime(
        event.endDateUtc
      ),

      ticketPrice: event.ticketPrice,
      totalTickets: event.totalTickets
    });

    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }

  publishEvent(event: EventResponse): void {
    this.clearMessages();

    if (!confirm(
      `Publish "${event.title}"?`
    )) {
      return;
    }

    this.eventService.publishEvent(
      event.id
    ).subscribe({
      next: () => {
        this.successMessage =
          'Event published successfully!';

        this.loadEvents();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage =
          this.getErrorMessage(
            error,
            'Unable to publish event.'
          );
      }
    });
  }

  deleteEvent(event: EventResponse): void {
    this.clearMessages();

    if (!confirm(
      `Are you sure you want to delete "${event.title}"?`
    )) {
      return;
    }

    this.eventService.deleteEvent(
      event.id
    ).subscribe({
      next: () => {
        this.successMessage =
          'Event deleted successfully.';

        if (this.selectedEventId === event.id) {
          this.resetForm();
        }

        this.loadEvents();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage =
          this.getErrorMessage(
            error,
            'Unable to delete event.'
          );
      }
    });
  }

  resetForm(): void {
    this.eventForm.reset({
      title: '',
      description: '',
      venue: '',
      startDateUtc: '',
      endDateUtc: '',
      ticketPrice: 0,
      totalTickets: 100
    });

    this.isEditing = false;
    this.selectedEventId = null;
  }

  private toLocalDateTime(
    dateString: string
  ): string {
    const date = new Date(dateString);

    const localDate = new Date(
      date.getTime() -
      date.getTimezoneOffset() * 60000
    );

    return localDate.toISOString().slice(0, 16);
  }

  private clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }

  private getErrorMessage(
    error: HttpErrorResponse,
    fallback: string
  ): string {
    const body = error.error;

    if (body?.message) {
      return body.message;
    }

    if (body?.title) {
      return body.title;
    }

    if (body?.errors) {
      const validationMessages = Object.values(
        body.errors
      ).flat();

      if (validationMessages.length > 0) {
        return validationMessages.join(' ');
      }
    }

    return fallback;
  }

  isInvalid(controlName: string): boolean {
    const control =
      this.eventForm.get(controlName);

    return !!(
      control &&
      control.invalid &&
      control.touched
    );
  }
}