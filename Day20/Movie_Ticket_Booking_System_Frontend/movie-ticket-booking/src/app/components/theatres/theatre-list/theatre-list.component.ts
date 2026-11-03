import { Component, OnInit } from '@angular/core';
import { Theatre } from '../../../models/theatre.model';
import { TheatreService } from '../../../services/theatre.service';

@Component({
  selector: 'app-theatre-list',
  standalone: false,
  templateUrl: './theatre-list.component.html'
})
export class TheatreListComponent implements OnInit {
  theatres: Theatre[] = [];
  loading = false;
  errorMessage = '';

  constructor(private theatreService: TheatreService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.theatreService.getAll().subscribe({
      next: (data: Theatre[]) => { this.theatres = data; this.loading = false; },
      error: () => { this.errorMessage = 'Failed to load theatres.'; this.loading = false; }
    });
  }

  delete(id: number): void {
    if (!confirm('Delete this theatre?')) return;
    this.theatreService.delete(id).subscribe({
      next: () => this.theatres = this.theatres.filter(t => t.theatreId !== id),
      error: (err: any) => alert(err.error?.message ?? 'Cannot delete theatre.')
    });
  }
}
