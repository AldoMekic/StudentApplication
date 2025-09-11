import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StudentMainPageComponent } from './student-main-page/student-main-page.component';
import { StudentInfoComponent } from './pages/student-info/student-info.component';
import { StudentGradesComponent } from './pages/student-grades/student-grades.component';
import { StudentSubjectsComponent } from './pages/student-subjects/student-subjects.component';

const routes: Routes = [
  {path: '', redirectTo: '/student-main-page', pathMatch: 'full'},
  {
    path: 'student-main-page', 
    component: StudentMainPageComponent,
    children: [
      { path: 'student-info', component: StudentInfoComponent },
      { path: 'student-grades', component: StudentGradesComponent },
      { path: 'student-subjects', component: StudentSubjectsComponent },
    ]},

  { path: 'students', component: StudentMainPageComponent },
  { path: 'professors', component: StudentMainPageComponent },
  { path: 'subjects', component: StudentMainPageComponent },
  { path: 'grades', component: StudentMainPageComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
