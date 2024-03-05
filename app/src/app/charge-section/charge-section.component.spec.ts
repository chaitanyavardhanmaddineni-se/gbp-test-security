import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChargeSectionComponent } from './charge-section.component';

describe('ChargeSectionComponent', () => {
  let component: ChargeSectionComponent;
  let fixture: ComponentFixture<ChargeSectionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChargeSectionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChargeSectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
