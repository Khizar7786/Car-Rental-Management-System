import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, FormsModule,RouterLink],
  templateUrl: './signup.html',
  styleUrls: ['./signup.css']
})
export class SignupComponent {

  fullName = '';
  email = '';
  phone = '';
  password = '';
  confirmPassword = '';
  licenseNo ='';

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  onRegister() {

    if (this.password !== this.confirmPassword) {
      alert('Passwords do not match');
      return;
    }

    const registerData = {
      fullName: this.fullName,
      email: this.email,
      password: this.password,
      phone: this.phone,
      licenseNo  :this.licenseNo   
    };

    this.http.post(
      'http://localhost:5000/api/auth/register',
      registerData
    ).subscribe({
      next: (response: any) => {
        alert('Registration successful');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        alert(err.error?.message || 'Registration failed');
      }
    });
  }
}