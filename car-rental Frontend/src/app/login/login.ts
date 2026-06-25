import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../services/authservice';
import { Router } from '@angular/router';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule,RouterLink],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class LoginComponent {
  
  public email: string = '';
  public password: string = '';
  constructor(private http: HttpClient, private auth: AuthService, private router: Router) {}

onLogin() {
  const loginData = {
    email: this.email,
    password: this.password
  };

  this.http.post('https://carzest-brd8fhbderagenbq.koreasouth-01.azurewebsites.net/api/auth/login', loginData)
    .subscribe({
      next: (response: any) => {
      if (response.success && response.data?.token) {

        // store toke
        localStorage.setItem('token', response.data.token);

        // extract user info
        const username = response.data.fullName;
        const role = response.data.role;

        // (optional but useful later)
        localStorage.setItem('role', role);

        // update navbar state
        this.auth.setUser(username);
        this.router.navigate(['/']);
      }
    },
      error: (err) => {
        alert('Login failed');
      }
    });
}
}