import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Screen } from '../../../models/screen.model';
import { Theatre } from '../../../models/theatre.model';
import { ScreenService } from '../../../services/screen.service';
import { TheatreService } from '../../../services/theatre.service';

@Component({
  selector: 'app-screen-list',
  standalone: false,
  templateUrl: './screen-list.component.html'
})
export class ScreenListComponent implements OnInit {
  screens: Screen[] = [];
  theatres: Theatre[] = [];
  filteredTheatreId: number | null = null;
  loading = false;
  errorMessage = '';

  constructor(
    private screenService: ScreenService,
    private theatreService: TheatreService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.theatreService.getAll().subscribe((t: Theatre[]) => this.theatres = t);
    const theatreId = this.route.snapshot.queryParamMap.get('theatreId');
    if (theatreId) this.filteredTheatreId = +theatreId;
    this.load();
  }

  load(): void {
    this.loading = true;
    const obs = this.filteredTheatreId
      ? this.screenService.getByTheatre(this.filteredTheatreId)
      : this.screenService.getAll();
    obs.subscribe({
      next: (data: Screen[]) => { this.screens = data; this.loading = false; },
      error: () => { this.errorMessage = 'Failed to load screens.'; this.loading = false; }
    });
  }

  delete(id: number): void {
    if (!confirm('Delete this screen?')) return;
    this.screenService.delete(id).subscribe({
      next: () => this.screens = this.screens.filter(s => s.screenId !== id),
      error: (err: any) => alert(err.error?.message ?? 'Cannot delete.')
    });
  }
}
