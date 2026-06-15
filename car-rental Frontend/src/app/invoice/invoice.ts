import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Observable, map } from 'rxjs';

import { InvoiceService } from '../services/invoice';
import { CustomerService } from '../services/customer';

@Component({
  selector: 'app-invoice',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './invoice.html',
  styleUrl: './invoice.css'
})
export class Invoice implements OnInit {

  invoiceForm: FormGroup;

  invoices$!: Observable<any[]>;
  customers: any[] = [];

  showForm = false;
  isEdit = false;
  selectedInvoiceId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private invoiceService: InvoiceService,
    private customerService: CustomerService,
  ) {

    this.invoiceForm = this.fb.group({
      customerId: ['', Validators.required],
      vehicleId: ['', Validators.required],
      agreementId: ['', Validators.required],
      plateNo: ['', Validators.required],
      rate: ['', Validators.required],
      startDate: ['', Validators.required],
      period: ['', Validators.required],
      status: ['Pending']
    });

  }

  ngOnInit(): void {
    this.refreshInvoices();
    this.loadCustomers();
  }

  refreshInvoices() {
    this.invoices$ = this.invoiceService.getInvoices().pipe(
      map((res: any) => res.data ?? [])
    );
  }

  loadCustomers() {
    this.customerService.getCustomers().subscribe({
      next: (res: any) => {
        this.customers = res.data;
      },
      error: err => console.error(err)
    });
  }

  openForm() {
    this.showForm = true;
    this.isEdit = false;
    this.selectedInvoiceId = null;

    this.invoiceForm.reset({
      status: 'Pending'
    });
  }

  closeForm() {
    this.showForm = false;
  }

  saveInvoice() {

    if (this.invoiceForm.invalid) {
      return;
    }

    const invoiceData = this.invoiceForm.value;

    if (this.isEdit && this.selectedInvoiceId) {

      this.invoiceService
        .updateInvoice(this.selectedInvoiceId, invoiceData)
        .subscribe({
          next: () => {
            this.refreshInvoices();
            this.closeForm();
          }
        });

    } else {

      this.invoiceService
        .createInvoice(invoiceData)
        .subscribe({
          next: () => {
            this.refreshInvoices();
            this.closeForm();
          }
        });
    }
  }

  editInvoice(invoice: any) {

    this.showForm = true;
    this.isEdit = true;
    this.selectedInvoiceId = invoice.id;

    this.invoiceForm.patchValue({
      customerId: invoice.customerId,
      vehicleId: invoice.vehicleId,
      agreementId: invoice.agreementId,
      plateNo: invoice.plateNo,
      rate: invoice.rate,
      startDate: invoice.startDate?.split('T')[0],
      period: invoice.period,
      status: invoice.status
    });
  }

  deleteInvoice(id: number) {

    if (!confirm('Delete invoice?')) {
      return;
    }

    this.invoiceService.deleteInvoice(id).subscribe({
      next: () => {
        this.refreshInvoices();
      }
    });
  }

  generateInvoice(id: number) {

    this.invoiceService.generateInvoice(id).subscribe({
      next: () => {
        this.refreshInvoices();
      }
    });
  }

  markPaid(id: number) {

    this.invoiceService.markPaid(id).subscribe({
      next: () => {
        this.refreshInvoices();
      }
    });
  }
}