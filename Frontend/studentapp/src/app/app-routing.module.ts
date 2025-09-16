import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StudentMainPageComponent } from './student-main-page/student-main-page.component';
import { StudentInfoComponent } from './pages/student-info/student-info.component';
import { StudentGradesComponent } from './pages/student-grades/student-grades.component';
import { StudentSubjectsComponent } from './pages/student-subjects/student-subjects.component';
import { ProfessorMainPageComponent } from './pages/professor-main-page/professor-main-page.component';
import { SubjectsMainPageComponent } from './pages/subjects-main-page/subjects-main-page.component';
import { GradesMainPageComponent } from './pages/grades-main-page/grades-main-page.component';

const routes: Routes = [
  {path: '', redirectTo: '/student-main-page', pathMatch: 'full'},
  {
    path: 'student-main-page', 
    component: StudentMainPageComponent,
    children: [
      { path: 'info', component: StudentInfoComponent },
      { path: 'grades', component: StudentGradesComponent },
      { path: 'subjects', component: StudentSubjectsComponent },
    ]},

  { path: 'students', component: StudentMainPageComponent },
  { path: 'professors',  component: ProfessorMainPageComponent },
  { path: 'subjects',    component: SubjectsMainPageComponent },
  { path: 'grades',      component: GradesMainPageComponent }, 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
