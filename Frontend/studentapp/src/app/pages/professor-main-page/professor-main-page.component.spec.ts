import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfessorMainPageComponent } from './professor-main-page.component';

describe('ProfessorMainPageComponent', () => {
  let component: ProfessorMainPageComponent;
  let fixture: ComponentFixture<ProfessorMainPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProfessorMainPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProfessorMainPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
