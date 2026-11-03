import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { provideHttpClient, withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';

import { AuthInterceptor } from './interceptors/auth.interceptor';

import { NavbarComponent } from './components/navbar/navbar.component';
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

@NgModule({
  declarations: [
    App,
    NavbarComponent,
    LoginComponent,
    RegisterComponent,
    MovieListComponent,
    MovieFormComponent,
    TheatreListComponent,
    TheatreFormComponent,
    ScreenListComponent,
    ScreenFormComponent,
    SeatManagementComponent,
    ShowListComponent,
    ShowFormComponent,
    BookingFormComponent,
    BookingListComponent,
    BookingDetailComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [App]
})
export class AppModule { }
