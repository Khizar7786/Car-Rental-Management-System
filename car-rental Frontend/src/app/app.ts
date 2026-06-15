import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar/navbar';
import { LoginComponent } from './login/login';
import {  SidebarComponent } from './sidebar/sidebar';
import { AuthService } from './services/authservice';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,NavbarComponent,SidebarComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
    protected readonly title = signal('car-rental');
    sidebarCollapsed = false;

  onSidebarToggle(state: boolean) {
    this.sidebarCollapsed = state;
  }
  constructor(private auth: AuthService) {
    this.auth.loadUserFromStorage(); // ← seeds the BehaviorSubject on startup
  }
}
