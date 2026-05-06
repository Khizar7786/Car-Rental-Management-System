import { Component,EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,          // ✅ fixes *ngIf and ngClass
    RouterLink, 
    RouterLinkActive
  ],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})
export class SidebarComponent {
  isCollapsed = false;
 @Output() collapsedChange = new EventEmitter<boolean>();
  toggleSidebar() {
    this.isCollapsed = !this.isCollapsed;
  }
}