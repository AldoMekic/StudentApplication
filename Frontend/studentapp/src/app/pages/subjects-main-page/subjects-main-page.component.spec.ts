import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubjectsMainPageComponent } from './subjects-main-page.component';

describe('SubjectsMainPageComponent', () => {
  let component: SubjectsMainPageComponent;
  let fixture: ComponentFixture<SubjectsMainPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SubjectsMainPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubjectsMainPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
