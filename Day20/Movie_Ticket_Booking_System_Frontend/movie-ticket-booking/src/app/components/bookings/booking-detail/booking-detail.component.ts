import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Booking } from '../../../models/booking.model';
import { BookingService } from '../../../services/booking.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-booking-detail',
  standalone: false,
  templateUrl: './booking-detail.component.html'
})
export class BookingDetailComponent implements OnInit {
  booking: Booking | null = null;
  loading = false;
  errorMessage = '';
  cancelling = false;

  constructor(
    private bookingService: BookingService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = +this.route.snapshot.paramMap.get('id')!;
    this.loading = true;
    this.bookingService.getById(id).subscribe({
      next: (b: Booking) => { this.booking = b; this.loading = false; },
      error: () => { this.errorMessage = 'Booking not found.'; this.loading = false; }
    });
  }

  cancelBooking(): void {
    if (!this.booking || !confirm('Are you sure you want to cancel this booking?')) return;
    this.cancelling = true;
    this.bookingService.cancel(this.booking.bookingId).subscribe({
      next: (b: Booking) => { this.booking = b; this.cancelling = false; },
      error: (err: any) => { this.errorMessage = err.error?.message ?? 'Cancel failed.'; this.cancelling = false; }
    });
  }
}
