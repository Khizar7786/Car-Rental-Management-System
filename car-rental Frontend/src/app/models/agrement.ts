export interface Agreement {
  agreementId?: number;
  customerId: number;
  vehicleId: number;
  startDate: string;
  endDate: string;
  notes: string;
}