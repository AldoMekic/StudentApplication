import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GradesMainPageComponent } from './grades-main-page.component';

describe('GradesMainPageComponent', () => {
  let component: GradesMainPageComponent;
  let fixture: ComponentFixture<GradesMainPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GradesMainPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GradesMainPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
