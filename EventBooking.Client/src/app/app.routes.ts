
import { Routes } from '@angular/router';

import { Register } from './components/register/register';
import { Login } from './components/login/login';
import { Home } from './components/home/home';
import { AdminDashboard } from './components/admin-dashboard/admin-dashboard';
import { CreateOrganizer } from './components/create-organizer/create-organizer';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'register', component: Register },
  { path: 'login', component: Login },
  { path: 'home', component: Home },
  { path: 'admin/dashboard', component: AdminDashboard },
  { path: 'admin/create-organizer', component: CreateOrganizer },
  { path: '**', redirectTo: 'login' }
];
