# Movie Ticket Booking System - Angular Frontend

Modern Angular frontend built to pair with the ASP.NET Core Movie Ticket Booking System Web API.

---

## 🚀 Features

- **Authentication & Authorization**:
  - JWT Bearer Authentication with automatic token injection via `AuthInterceptor`.
  - Customer registration & login.
  - Role-based route guards (`AuthGuard`) supporting `Admin`, `Customer`, and `TheatreManager`.
- **Movies**:
  - Browse movies with title search, genre and language badges, duration, and release dates.
  - Admin movie management: Create, update, and delete movies.
- **Theatres & Screens**:
  - Browse theatres and screens.
  - Admin management for theatres and screens with theatre association.
- **Seats Management**:
  - Interactive seat configuration for screens.
  - Bulk seat creation (Row, Seat Number, and Seat Type: Standard, Premium, VIP, Recliner).
- **Shows**:
  - Browse upcoming shows filtered by movie and theatre.
  - Admin show scheduling (with screen overlap validation and pricing).
- **Interactive Ticket Booking**:
  - Realistic cinema seat layout with screen curve visualization.
  - Live seat availability check (`/api/shows/{id}/seats/available`).
  - Real-time seat selection & total price calculation.
  - Instant booking creation linked to authenticated customer claims.
- **My Bookings & Management**:
  - View bookings list with status (`Confirmed` / `Cancelled`).
  - Detailed booking pass view with reference code (`MOV...`) and seat chips.
  - Cancel booking functionality (automatically frees up booked seats).

---

## 🛠️ Getting Started

### 1. Run the .NET Backend
Open your backend project (`Movie_Ticket_Booking_System`) and run it:
```bash
dotnet run
```
The backend will run at `http://localhost:5184` (or `https://localhost:7084`).

### 2. Run the Angular Frontend
Inside the `movie-ticket-booking` folder:
```bash
npm start
```
`npm start` uses `proxy.conf.json` to proxy API requests to `http://localhost:5184`, avoiding CORS restrictions during development.

Open your browser at:
```
http://localhost:4200
```

---

## 📁 Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── auth/ (login, register)
│   │   ├── bookings/ (booking-form with seat map, booking-list, booking-detail)
│   │   ├── movies/ (movie-list, movie-form)
│   │   ├── navbar/ (header navigation, user state)
│   │   ├── screens/ (screen-list, screen-form)
│   │   ├── seats/ (seat-management with bulk creator)
│   │   ├── shows/ (show-list, show-form)
│   │   └── theatres/ (theatre-list, theatre-form)
│   ├── guards/
│   │   └── auth.guard.ts (Route guard with role checks)
│   ├── interceptors/
│   │   └── auth.interceptor.ts (Bearer token & 401 handling)
│   ├── models/ (auth, movie, theatre, screen, seat, show, booking)
│   ├── services/ (auth, movie, theatre, screen, seat, show, booking)
│   ├── app-module.ts
│   ├── app-routing-module.ts
│   ├── app.ts
│   └── app.html
├── environments/
│   ├── environment.ts
│   └── environment.prod.ts
├── styles.css (Modern cinema dark theme)
└── proxy.conf.json (Reverse proxy to .NET backend)
```
