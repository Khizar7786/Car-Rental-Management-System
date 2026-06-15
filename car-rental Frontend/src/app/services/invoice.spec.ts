import { TestBed } from '@angular/core/testing';

import { Invoice } from '../invoice/invoice';

describe('Invoice', () => {
  let service: Invoice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Invoice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
