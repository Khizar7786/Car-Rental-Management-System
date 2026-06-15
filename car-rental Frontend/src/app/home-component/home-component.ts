import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterOutlet } from '@angular/router';
import { Vehichle } from '../vehichle/vehichle';
@Component({
  selector: 'app-home-component',
  imports: [CommonModule, RouterLink, RouterOutlet, Vehichle],
  templateUrl: './home-component.html',
  styleUrl: './home-component.css',
})
export class HomeComponent {

  
}
