import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { Observable, map } from 'rxjs';
import { AgreementService } from '../services/agreement';
import { CustomerService } from '../services/customer';
import { ChangeDetectorRef } from '@angular/core';
// import { VehicleService } from '../services/vehicle';

@Component({
  selector: 'app-agreement',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './agreement.html',
  styleUrl: './agreement.css'
})
export class Agreement implements OnInit {

  agreementForm: FormGroup;

  agreements$!: Observable<any[]>;
  customers: any[] = [];
  vehicles: any[] = [
    { id: 1, make: 'Suzuki', model: 'Alto', plateNo: 'ABC-123', isAvailable: true },
    { id: 2, make: 'Suzuki', model: 'Cultus', plateNo: 'DEF-456', isAvailable: true },
    { id: 3, make: 'Toyota', model: 'Yaris', plateNo: 'GHI-789', isAvailable: true },
    { id: 4, make: 'Honda', model: 'City', plateNo: 'JKL-111', isAvailable: true },
    { id: 5, make: 'Toyota', model: 'Corolla', plateNo: 'MNO-222', isAvailable: true }
  ];

  showForm = false;
  isEdit = false;
  selectedAgreementId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private agreementService: AgreementService,
    private customerService: CustomerService,
    private router: Router,

    // private vehicleService: VehicleService
  ) {

    this.agreementForm = this.fb.group({
      customerId: ['', Validators.required],
      vehicleId: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      notes: [''],
      status: ['Active']
    });

  }

  ngOnInit(): void {
    console.trace('LOAD AGREEMENTS CALLED');

    this.refreshAgreements();
    this.loadCustomers();
    // this.loadVehichles();
  }

  refreshAgreements() {
    this.agreements$ = this.agreementService.getAgreements().pipe(
      map((res: any) => res.data ?? [])
    );
  }

  loadCustomers() {
    this.customerService.getCustomers().subscribe({
      next: (res: any) => {
        this.customers = res.data;
      }
    });
  }

  // loadVehicles() {
  //   this.vehicleService.getVehicles().subscribe({
  //     next: (res: any) => {
  //       this.vehicles = res.data.filter(
  //         (v: any) => v.isAvailable === true
  //       );
  //     }
  //   });
  // }

  openForm() {
    this.showForm = true;
    this.isEdit = false;
    this.selectedAgreementId = null;

    this.agreementForm.reset({
      status: 'Active'
    });
  }

  closeForm() {
    this.showForm = false;
  }

  saveAgreement() {

    if (this.agreementForm.invalid) {
      return;
    }

    const agreementData = this.agreementForm.value;

    if (this.isEdit && this.selectedAgreementId) {

      this.agreementService
        .updateAgreement(
          this.selectedAgreementId,
          agreementData
        )
        .subscribe({
          next: () => {
            this.refreshAgreements();
            this.closeForm();
          }
        });

    } else {

      this.agreementService
        .createAgreement(agreementData)
        .subscribe({
          next: () => {
            this.refreshAgreements();
            this.closeForm();
          }
        });
    }
  }

  editAgreement(agreement: any) {

    this.showForm = true;
    this.isEdit = true;
    this.selectedAgreementId = agreement.id;

    this.agreementForm.patchValue({
      customerId: agreement.customerId,
      vehicleId: agreement.vehicleId,
      startDate: agreement.startDate?.split('T')[0],
      endDate: agreement.endDate?.split('T')[0],
      notes: agreement.notes,
      status: agreement.status
    });
  }

  deleteAgreement(id: number) {

    if (!confirm('Delete agreement?')) {
      return;
    }

    this.agreementService.deleteAgreement(id).subscribe({
      next: () => {
        this.refreshAgreements();
      }
    });
  }
}