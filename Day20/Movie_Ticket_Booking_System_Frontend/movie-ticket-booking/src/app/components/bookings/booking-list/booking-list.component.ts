import { Component, OnInit } from '@angular/core';
import { Booking } from '../../../models/booking.model';
import { BookingService } from '../../../services/booking.service';
import { AuthService } from '../../../services/auth.service';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-booking-list',
  standalone: false,
  templateUrl: './booking-list.component.html'
})
export class BookingListComponent implements OnInit {
  bookings: Booking[] = [];
  loading = false;
  errorMessage = '';
  isAdmin = false;

  constructor(
    private bookingService: BookingService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';

    if (this.isAdmin) {
      this.bookingService.getAll().subscribe({
        next: (data: Booking[]) => {
          this.bookings = data;
          this.loading = false;
        },
        error: (err: any) => {
          this.errorMessage = err.error?.message ?? 'Failed to load bookings.';
          this.loading = false;
        }
      });
    } else {
      // For Customer, load their booking IDs from localStorage and fetch individually
      const user = this.authService.getCurrentUser();
      const key = `user_bookings_${user?.email ?? 'guest'}`;
      const ids: number[] = JSON.parse(localStorage.getItem(key) ?? '[]');

      if (ids.length === 0) {
        this.bookings = [];
        this.loading = false;
        return;
      }

      const requests = ids.map(id =>
        this.bookingService.getById(id).pipe(catchError(() => of(null)))
      );

      forkJoin(requests).subscribe({
        next: (results) => {
          this.bookings = results.filter((b): b is Booking => b !== null);
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
    }
  }

  cancel(id: number): void {
    if (!confirm('Are you sure you want to cancel this booking?')) return;
    this.bookingService.cancel(id).subscribe({
      next: (updated: Booking) => {
        const idx = this.bookings.findIndex(b => b.bookingId === id);
        if (idx > -1) this.bookings[idx] = updated;
      },
      error: (err: any) => alert(err.error?.message ?? 'Cancellation failed.')
    });
  }
}
