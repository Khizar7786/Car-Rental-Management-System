import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {

  private apiUrl = 'http://localhost:5000/api/invoices';

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

  getInvoices(): Observable<any> {
    return this.http.get(this.apiUrl);
  }

  getInvoice(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  createInvoice(invoice: any): Observable<any> {
    return this.http.post(this.apiUrl, invoice);
  }

  updateInvoice(id: number, invoice: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, invoice);
  }

  deleteInvoice(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  generateInvoice(id: number): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/${id}/generate`,
      {}
    );
  }

  markPaid(id: number): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/${id}/pay`,
      {}
    );
  }
}