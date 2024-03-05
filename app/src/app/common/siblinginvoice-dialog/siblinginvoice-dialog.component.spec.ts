import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SiblinginvoiceDialogComponent } from './siblinginvoice-dialog.component';

describe('SiblinginvoiceDialogComponent', () => {
  let component: SiblinginvoiceDialogComponent;
  let fixture: ComponentFixture<SiblinginvoiceDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SiblinginvoiceDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SiblinginvoiceDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
