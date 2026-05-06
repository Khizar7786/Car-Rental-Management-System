import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Vehichle } from './vehichle';

describe('Vehichle', () => {
  let component: Vehichle;
  let fixture: ComponentFixture<Vehichle>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Vehichle],
    }).compileComponents();

    fixture = TestBed.createComponent(Vehichle);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
