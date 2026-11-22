import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard {
  private authService = inject(AuthService);
  private router = inject(Router);

  user = JSON.parse(localStorage.getItem('user') || 'null');

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
