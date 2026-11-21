
import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.html'
})
export class Home {

  private authService = inject(AuthService);
  private router = inject(Router);

  user = JSON.parse(
    localStorage.getItem('user') || 'null'
  );

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
