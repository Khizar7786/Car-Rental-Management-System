import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-invoice',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './invoice.html',
  styleUrl: './invoice.css',
})
export class Invoice {

  invoiceForm: FormGroup;

  customers = ['Ali', 'Ahmed', 'Usman'];

  invoices: any[] = [];

  showForm = false;
  isEdit = false;
  editIndex: number | null = null;

  constructor(private fb: FormBuilder) {
    this.invoiceForm = this.fb.group({
      invoiceNo: [''],
      customer: [''],
      plateNo: [''],
      rate: [''],
      startDate: [''],
      period: [''],
      status: ['Pending']
    });
  }

  // ================= UI CONTROL =================
  openForm() {
    this.showForm = true;
    this.isEdit = false;
    this.invoiceForm.reset({ status: 'Pending' });
  }

  closeForm() {
    this.showForm = false;
  }

  // ================= CREATE / UPDATE =================
  saveInvoice() {
    if (this.isEdit && this.editIndex !== null) {
      this.invoices[this.editIndex] = this.invoiceForm.value;
    } else {
      this.invoices.push(this.invoiceForm.value);
    }

    this.closeForm();
  }

  // ================= EDIT =================
  editInvoice(index: number) {
    this.showForm = true;
    this.isEdit = true;
    this.editIndex = index;

    this.invoiceForm.patchValue(this.invoices[index]);
  }

  // ================= DELETE =================
  deleteInvoice(index: number) {
    this.invoices.splice(index, 1);
  }

  // ================= GENERATE =================
  generateInvoice(index: number) {
    this.invoices[index].status = 'Generated';
  }
}