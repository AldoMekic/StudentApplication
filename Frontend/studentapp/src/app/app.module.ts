import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { StudentMainPageComponent } from './student-main-page/student-main-page.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { StudentInfoComponent } from './pages/student-info/student-info.component';
import { StudentGradesComponent } from './pages/student-grades/student-grades.component';
import { StudentSubjectsComponent } from './pages/student-subjects/student-subjects.component';
import { StudentNavbarComponent } from './components/student-navbar/student-navbar.component';
import { ProfessorMainPageComponent } from './pages/professor-main-page/professor-main-page.component';
import { SubjectsMainPageComponent } from './pages/subjects-main-page/subjects-main-page.component';
import { GradesMainPageComponent } from './pages/grades-main-page/grades-main-page.component';

@NgModule({
  declarations: [
    AppComponent,
    StudentMainPageComponent,
    NavbarComponent,
    SidebarComponent,
    StudentInfoComponent,
    StudentGradesComponent,
    StudentSubjectsComponent,
    StudentNavbarComponent,
    ProfessorMainPageComponent,
    SubjectsMainPageComponent,
    GradesMainPageComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
