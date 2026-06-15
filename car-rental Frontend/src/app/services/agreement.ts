import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class AgreementService {

  private apiUrl = 'http://localhost:5000/api/agreements';

  constructor(private http: HttpClient) {}

private getHeaders() {

  let token = '';

  if (typeof window !== 'undefined') {
    token = localStorage.getItem('token') || '';
  }

  return {
    headers: new HttpHeaders({
      Authorization: `Bearer ${token}`
    })
  };
}

  getAgreements(): Observable<any> {
    return this.http.get(this.apiUrl);
  }

  getAgreement(id: number): Observable<any> {
     const token = localStorage.getItem('token');

  if (!token) {
    throw new Error('No token available');
  }
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  createAgreement(data: any): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }

  updateAgreement(id: number, data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, data);
  }

  deleteAgreement(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}