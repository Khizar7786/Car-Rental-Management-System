import { Component,EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../services/authservice';
@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,          
    RouterLink, 
    RouterLinkActive
  ],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})
export class SidebarComponent {
    user: string | null = null;
    isCollapsed = false;
    constructor(private auth: AuthService) {}
 @Output() collapsedChange = new EventEmitter<boolean>();
  toggleSidebar() {
    this.isCollapsed = !this.isCollapsed;
  }
ngOnInit() {
  // Load from storage safely (SSR-compatible)
  if (typeof window !== 'undefined') {
    this.user = localStorage.getItem('username');
  }

  // Keep reactive for login/logout
  this.auth.user$.subscribe(user => {
    this.user = user;
  });
}
  test() {
  console.log('Customer clicked');
}
}