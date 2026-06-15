import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-vehichle',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './vehichle.html',
  styleUrl: './vehichle.css',
})
export class Vehichle {

  selectedType: string = 'all';

  cars = [
    {
      id: 1,
      name: 'Suzuki Alto (2021-2024)',
      price: 4500,
      image: 'assets/images/alto.jpg',
      type: 'hatchback'
    },
    {
      id: 2,
      name: 'Suzuki Cultus (2021-2024)',
      price: 5000,
      image: 'assets/images/cultus.jpg',
      type: 'hatchback'
    },
    {
      id: 3,
      name: 'Suzuki Wagon R (2021-2024)',
      price: 5500,
      image: 'assets/images/wagonR.jpg',
      type: 'hatchback'
    },
    {
      id: 4,
      name: 'Toyota Yaris (2021-2024)',
      price: 6500,
      image: 'assets/images/yaris.jpg',
      type: 'sedan'
    },
    {
      id: 5,
      name: 'Honda City (2021-2024)',
      price: 7500,
      image: 'assets/images/city.jpg',
      type: 'sedan'
    },
    {
      id: 6,
      name: 'Toyota Corolla (2021-2024)',
      price: 8500,
      image: 'assets/images/corolla.jpg',
      type: 'sedan'
    }
  ];

  get filteredCars() {
    return this.selectedType === 'all'
      ? this.cars
      : this.cars.filter(c => c.type === this.selectedType);
  }

  setFilter(type: string) {
    this.selectedType = type;
  }
}