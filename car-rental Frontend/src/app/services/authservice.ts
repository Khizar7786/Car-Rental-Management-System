import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';
@Injectable({ providedIn: 'root' })
export class AuthService {

  private userSubject = new BehaviorSubject<string | null>(null);
  user$ = this.userSubject.asObservable();
constructor(private router: Router) {}
  setUser(username: string) {
    this.userSubject.next(username);
    localStorage.setItem('username', username);
  }

  logout() {
    this.userSubject.next(null);
    localStorage.removeItem('username');
    localStorage.removeItem('token');
    this.router.navigate(['']);
  }

loadUserFromStorage() {
  if (typeof window !== 'undefined') {
    const user = localStorage.getItem('username');
    if (user) this.userSubject.next(user);
  }
}
}