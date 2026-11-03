import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ShowService } from '../../../services/show.service';
import { BookingService } from '../../../services/booking.service';
import { AuthService } from '../../../services/auth.service';
import { Show } from '../../../models/show.model';
import { ShowSeat } from '../../../models/seat.model';
import { Booking } from '../../../models/booking.model';

@Component({
  selector: 'app-booking-form',
  standalone: false,
  templateUrl: './booking-form.component.html'
})
export class BookingFormComponent implements OnInit {
  show: Show | null = null;
  seats: ShowSeat[] = [];
  selectedSeatIds: number[] = [];
  form: FormGroup;
  loading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private showService: ShowService,
    private bookingService: BookingService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      customerPhone: ['', [Validators.required, Validators.pattern(/^[+\d]{7,15}$/)]]
    });
  }

  ngOnInit(): void {
    const showId = +this.route.snapshot.paramMap.get('showId')!;
    this.showService.getById(showId).subscribe((s: Show) => this.show = s);
    this.showService.getSeats(showId).subscribe({
      next: (seats: ShowSeat[]) => this.seats = seats,
      error: () => this.showService.getAvailableSeats(showId).subscribe(s => this.seats = s)
    });
  }

  toggleSeat(seat: ShowSeat): void {
    if (seat.status !== 'Available') return;
    const idx = this.selectedSeatIds.indexOf(seat.seatId);
    if (idx === -1) {
      this.selectedSeatIds.push(seat.seatId);
    } else {
      this.selectedSeatIds.splice(idx, 1);
    }
  }

  isSelected(seatId: number): boolean {
    return this.selectedSeatIds.includes(seatId);
  }

  getSelectedLabels(): string {
    return this.seats
      .filter(s => this.selectedSeatIds.includes(s.seatId))
      .map(s => s.seatLabel)
      .join(', ');
  }

  get totalAmount(): number {
    return (this.show?.ticketPrice ?? 0) * this.selectedSeatIds.length;
  }

  submit(): void {
    if (this.form.invalid || this.selectedSeatIds.length === 0) return;
    this.loading = true;
    this.errorMessage = '';
    const request = {
      customerPhone: this.form.value.customerPhone,
      showId: this.show!.showId,
      seatIds: this.selectedSeatIds
    };
    this.bookingService.create(request).subscribe({
      next: (booking: Booking) => {
        // Save to customer's booking history
        const user = this.authService.getCurrentUser();
        const key = `user_bookings_${user?.email ?? 'guest'}`;
        const existing: number[] = JSON.parse(localStorage.getItem(key) ?? '[]');
        if (!existing.includes(booking.bookingId)) {
          existing.unshift(booking.bookingId);
          localStorage.setItem(key, JSON.stringify(existing));
        }
        this.router.navigate(['/bookings', booking.bookingId]);
      },
      error: (err: any) => {
        this.errorMessage = err.error?.message ?? 'Booking failed. Selected seats may already be booked.';
        this.loading = false;
      }
    });
  }
}
