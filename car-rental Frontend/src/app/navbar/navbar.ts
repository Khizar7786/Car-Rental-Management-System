import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../services/authservice';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports:[RouterLink,RouterLinkActive,CommonModule],
  templateUrl: './navbar.html',
    styleUrls: ['./navbar.css']
})
export class NavbarComponent {
  menuOpen = false;
  username: string | null = null;
  constructor(public auth: AuthService) {}

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
    console.log("Button was clicked");
  }
  ngOnInit() {
  this.auth.loadUserFromStorage();

  this.auth.user$.subscribe(user => {
    this.username = user;
  });
}

}