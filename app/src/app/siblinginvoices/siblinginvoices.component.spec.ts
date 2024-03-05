import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SiblinginvoicesComponent } from './siblinginvoices.component';

describe('SiblinginvoicesComponent', () => {
  let component: SiblinginvoicesComponent;
  let fixture: ComponentFixture<SiblinginvoicesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SiblinginvoicesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SiblinginvoicesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
