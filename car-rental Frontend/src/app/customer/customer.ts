import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CustomerService } from '../services/customer';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-customer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './customer.html',
  styleUrls: ['./customer.css']
})
export class Customer implements OnInit {

  customers: any[] = [];

  customer = {
    fullName: '',
    email: '',
    password: '',
    phone: '',
    address: '',
    licenseNo: '',
    nationalId: ''
  };

  constructor(
    private customerService: CustomerService,
    private router: Router   // ← make sure this is injected
  )  {}
  ngOnInit(): void {
    this.loadCustomers();  // ← your existing call

    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.loadCustomers();  // ← fires again on every navigation
      });
  }
  
  loadCustomers() { 
    this.customerService.getCustomers().subscribe({
      next: (res: any) => {
        this.customers = res.data;
      },
      error: err => {
        console.error(err);
      }
    });
  }

  addCustomer() {
    this.customerService.createCustomer(this.customer).subscribe({
      next: () => {
        alert('Customer Created');
        this.loadCustomers();

        this.customer = {
          fullName: '',
          email: '',
          password: '',
          phone: '',
          address: '',
          licenseNo: '',
          nationalId: ''
        };
      },
      error: err => {
        console.error(err);
        alert('Failed');
      }
    });
  }

  deleteCustomer(id: number) {
    if (!confirm('Deactivate customer?')) return;

    this.customerService.deleteCustomer(id).subscribe({
      next: () => {
        this.loadCustomers();
      }
    });
  }
}