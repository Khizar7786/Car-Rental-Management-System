import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  private apiUrl = 'https://carzest-brd8fhbderagenbq.koreasouth-01.azurewebsites.net/api/customers';

  constructor(private http: HttpClient) {}

private getHeaders() {
  console.log('SAFE GETHEADERS RUNNING');

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

  getCustomers(): Observable<any> {
    return this.http.get(this.apiUrl);
  }

  getCustomer(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  createCustomer(customer: any): Observable<any> {
    return this.http.post(this.apiUrl, customer);
  }

  updateCustomer(id: number, customer: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, customer);
  }

  deleteCustomer(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}