import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-agreement',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './agreement.html',
  styleUrl: './agreement.css',
})
export class Agreement {

  agreementForm: FormGroup;

  customers = ['Ali', 'Ahmed', 'Usman'];
  vehicles = ['Corolla', 'Civic', 'Alto'];

  agreements: any[] = [];

  showForm = false;
  isEdit = false;
  editIndex: number | null = null;

  constructor(private fb: FormBuilder) {
    this.agreementForm = this.fb.group({
      customer: [''],
      vehicle: [''],
      startDate: [''],
      endDate: [''],
      rate: ['']
    });
  }

  // ================= UI CONTROL =================
  openForm() {
    this.showForm = true;
    this.isEdit = false;
    this.agreementForm.reset();
  }

  closeForm() {
    this.showForm = false;
  }

  // ================= CREATE / UPDATE =================
  saveAgreement() {
    if (this.isEdit && this.editIndex !== null) {
      this.agreements[this.editIndex] = this.agreementForm.value;
    } else {
      this.agreements.push(this.agreementForm.value);
    }

    this.closeForm();
  }

  // ================= EDIT =================
  editAgreement(index: number) {
    this.showForm = true;
    this.isEdit = true;
    this.editIndex = index;

    this.agreementForm.patchValue(this.agreements[index]);
  }

  // ================= DELETE =================
  deleteAgreement(index: number) {
    this.agreements.splice(index, 1);
  }
}