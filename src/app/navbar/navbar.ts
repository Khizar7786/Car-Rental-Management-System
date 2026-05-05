import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports:[RouterLink,RouterLinkActive],
  templateUrl: './navbar.html',
    styleUrls: ['./navbar.css']
})
export class NavbarComponent {
  menuOpen = false;

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
    console.log("Button was clicked");
  }
}