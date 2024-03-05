import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddMultipleinvoiceDialogComponent } from './add-multipleinvoice-dialog.component';

describe('AddMultipleinvoiceDialogComponent', () => {
  let component: AddMultipleinvoiceDialogComponent;
  let fixture: ComponentFixture<AddMultipleinvoiceDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddMultipleinvoiceDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddMultipleinvoiceDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
