import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { SeatService } from '../../../services/seat.service';
import { ScreenService } from '../../../services/screen.service';
import { Screen } from '../../../models/screen.model';
import { Seat } from '../../../models/seat.model';

@Component({
  selector: 'app-seat-management',
  standalone: false,
  templateUrl: './seat-management.component.html'
})
export class SeatManagementComponent implements OnInit {
  screens: Screen[] = [];
  seats: Seat[] = [];
  selectedScreenId: number | null = null;
  bulkForm: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';
  seatTypes = ['Standard', 'Premium', 'VIP', 'Recliner'];

  constructor(
    private fb: FormBuilder,
    private seatService: SeatService,
    private screenService: ScreenService,
    private route: ActivatedRoute
  ) {
    this.bulkForm = this.fb.group({
      screenId: ['', Validators.required],
      seats: this.fb.array([])
    });
  }

  get seatsArray(): FormArray {
    return this.bulkForm.get('seats') as FormArray;
  }

  ngOnInit(): void {
    this.screenService.getAll().subscribe((s: Screen[]) => this.screens = s);
    const screenId = this.route.snapshot.queryParamMap.get('screenId');
    if (screenId) {
      this.selectedScreenId = +screenId;
      this.bulkForm.patchValue({ screenId: +screenId });
      this.loadSeats(+screenId);
    }
  }

  onScreenChange(): void {
    const id = +this.bulkForm.value.screenId;
    if (id) this.loadSeats(id);
  }

  loadSeats(screenId: number): void {
    this.seatService.getByScreen(screenId).subscribe({
      next: (data: Seat[]) => this.seats = data,
      error: () => {}
    });
  }

  addSeatRow(): void {
    this.seatsArray.push(this.fb.group({
      rowName: ['A', [Validators.required, Validators.maxLength(5)]],
      seatNumber: [1, [Validators.required, Validators.min(1)]],
      seatType: ['Standard', Validators.required]
    }));
  }

  removeSeatRow(i: number): void {
    this.seatsArray.removeAt(i);
  }

  submitBulk(): void {
    if (this.bulkForm.invalid || this.seatsArray.length === 0) return;
    this.loading = true;
    this.errorMessage = '';
    const request = {
      screenId: +this.bulkForm.value.screenId,
      seats: this.seatsArray.value
    };
    this.seatService.bulkCreate(request).subscribe({
      next: () => {
        this.successMessage = 'Seats created successfully!';
        this.loadSeats(request.screenId);
        this.seatsArray.clear();
        this.loading = false;
      },
      error: (err: any) => {
        this.errorMessage = err.error?.message ?? 'Failed to create seats.';
        this.loading = false;
      }
    });
  }

  deleteSeat(id: number): void {
    if (!confirm('Delete this seat?')) return;
    this.seatService.delete(id).subscribe({
      next: () => this.seats = this.seats.filter(s => s.seatId !== id),
      error: (err: any) => alert(err.error?.message ?? 'Cannot delete.')
    });
  }
}
