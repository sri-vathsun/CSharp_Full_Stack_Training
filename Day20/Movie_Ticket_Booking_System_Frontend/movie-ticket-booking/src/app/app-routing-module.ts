import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { MovieListComponent } from './components/movies/movie-list/movie-list.component';
import { MovieFormComponent } from './components/movies/movie-form/movie-form.component';
import { TheatreListComponent } from './components/theatres/theatre-list/theatre-list.component';
import { TheatreFormComponent } from './components/theatres/theatre-form/theatre-form.component';
import { ScreenListComponent } from './components/screens/screen-list/screen-list.component';
import { ScreenFormComponent } from './components/screens/screen-form/screen-form.component';
import { SeatManagementComponent } from './components/seats/seat-management/seat-management.component';
import { ShowListComponent } from './components/shows/show-list/show-list.component';
import { ShowFormComponent } from './components/shows/show-form/show-form.component';
import { BookingFormComponent } from './components/bookings/booking-form/booking-form.component';
import { BookingListComponent } from './components/bookings/booking-list/booking-list.component';
import { BookingDetailComponent } from './components/bookings/booking-detail/booking-detail.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  { path: '', redirectTo: 'movies', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // Movies
  { path: 'movies', component: MovieListComponent },
  { path: 'admin/movies/new', component: MovieFormComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },
  { path: 'admin/movies/:id/edit', component: MovieFormComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },

  // Theatres
  { path: 'admin/theatres', component: TheatreListComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },
  { path: 'admin/theatres/new', component: TheatreFormComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },
  { path: 'admin/theatres/:id/edit', component: TheatreFormComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },

  // Screens
  { path: 'admin/screens', component: ScreenListComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },
  { path: 'admin/screens/new', component: ScreenFormComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },
  { path: 'admin/screens/:id/edit', component: ScreenFormComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },

  // Seats
  { path: 'admin/seats', component: SeatManagementComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },

  // Shows
  { path: 'shows', component: ShowListComponent },
  { path: 'admin/shows/new', component: ShowFormComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },
  { path: 'admin/shows/:id/edit', component: ShowFormComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },
  { path: 'shows/:showId/book', component: BookingFormComponent, canActivate: [AuthGuard] },

  // Bookings
  { path: 'bookings', component: BookingListComponent, canActivate: [AuthGuard] },
  { path: 'bookings/:id', component: BookingDetailComponent, canActivate: [AuthGuard] },

  { path: '**', redirectTo: 'movies' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
