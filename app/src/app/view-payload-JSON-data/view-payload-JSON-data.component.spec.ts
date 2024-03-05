import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewPayloadJSONDataComponent } from './view-payload-JSON-data.component';

describe('ViewPayloadJSONDataComponent', () => {
  let component: ViewPayloadJSONDataComponent;
  let fixture: ComponentFixture<ViewPayloadJSONDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewPayloadJSONDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewPayloadJSONDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
