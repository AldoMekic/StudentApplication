import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StudentMainPageComponent } from './student-main-page/student-main-page.component';

const routes: Routes = [
  {path: '', redirectTo: '/student-main-page', pathMatch: 'full'},
  {path: 'student-main', component: StudentMainPageComponent},

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
