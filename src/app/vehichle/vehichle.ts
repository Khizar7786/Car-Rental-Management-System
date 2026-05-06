import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-vehichle',
  imports: [CommonModule ],
  templateUrl: './vehichle.html',
  styleUrl: './vehichle.css',
})
export class Vehichle {
    selectedType: string = 'all';
    cars = [
  {
    name: 'Suzuki Alto (2021-2024)',
    price: '4,500',
    image: 'assets/images/alto.jpg',
    type: 'hatchback'
  },
  {
    name: 'Suzuki Cultus (2021-2024)',
    price: '5,000',
    image: 'assets/images/cultus.jpg',
    type: 'hatchback'
  },
  {
    name: 'Suzuki Wagon R (2021-2024)',
    price: '5,500',
    image: 'assets/images/wagonR.jpg',
    type: 'hatchback'
  },
  {
    name: 'Toyota Yaris (2021-2024)',
    price: '6,500',
    image: 'assets/images/yaris.jpg',
    type: 'sedan'
  },
  {
    name: 'Honda City (2021-2024)',
    price: '7,500',
    image: 'assets/images/city.jpg',
    type: 'sedan'
  },
  {
    name: 'Toyota Corolla (2021-2024)',
    price: '8,500',
    image: 'assets/images/corolla.jpg',
    type: 'sedan'
  }
];
  get filteredCars() {
  if (this.selectedType === 'all') {
    return this.cars;
  }
  return this.cars.filter(car => car.type === this.selectedType);
}
setFilter(type: string) {
  this.selectedType = type;
}
}
