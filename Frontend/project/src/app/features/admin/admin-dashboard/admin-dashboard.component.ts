import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { DepartmentsService, DepartmentRequestDTO, DepartmentResponseDTO } from '../../../core/services/departments.service';
import { SubjectsService, SubjectRequestDTO, SubjectResponseDTO } from '../../../core/services/subjects.service';
import { ProfessorsService, ProfessorResponseDTO } from '../../../core/services/professors.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  adminName = '';
  // approvals temporarily disabled (no matching backend endpoints right now)
  pendingProfessors: any[] = [];
  approvedProfessors: ProfessorResponseDTO[] = [];

  departments: DepartmentResponseDTO[] = [];
  subjects: SubjectResponseDTO[] = [];
  loading = true;
  activeTab: 'approvals' | 'departments' | 'subjects' = 'approvals';

  showDepartmentModal = false;
  showSubjectModal = false;

  newDepartment: DepartmentRequestDTO = {
    name: '',
    code: '',
    description: ''
  };

  newSubject: SubjectRequestDTO = {
    title: '',
    academicYear: '',
    description: '',
    professorId: 0
  };

  constructor(
    public authService: AuthService,
    private departmentsService: DepartmentsService,
    private subjectsService: SubjectsService,
    private professorsService: ProfessorsService
  ) {}

  async ngOnInit() {
    const user = this.authService.getUserProfile();
    this.adminName = user?.first_name || 'Admin';
    await this.loadAll();
  }

  async loadAll() {
  this.loading = true;
  try {
    await Promise.all([
      this.loadDepartments(),
      this.loadSubjects(),
      this.loadProfessors()
    ]);
  } finally {
    this.loading = false;
  }
}

  async loadDepartments() {
    this.departments = await this.departmentsService.getAll();
  }

  async loadSubjects() {
    this.subjects = await this.subjectsService.getAll();
  }

  async loadProfessors() {
  this.approvedProfessors = await this.professorsService.getAll();
}

  // Departments
  openDepartmentModal() {
    this.showDepartmentModal = true;
    this.newDepartment = { name: '', code: '', description: '' };
  }

  approveProfessor(professor: any) {
  // TODO: call backend once endpoint is confirmed.
  // For now, remove from pending list locally to avoid template errors.
  this.pendingProfessors = this.pendingProfessors.filter(p => p.id !== professor.id);
  // Optionally add to approved cache
  this.approvedProfessors = [...this.approvedProfessors, professor];
}

rejectProfessor(professor: any) {
  // TODO: call backend to reject/delete
  this.pendingProfessors = this.pendingProfessors.filter(p => p.id !== professor.id);
}

  async createDepartment() {
    if (!this.newDepartment.name || !this.newDepartment.code) {
      alert('Please fill in all required fields');
      return;
    }
    await this.departmentsService.create(this.newDepartment);
    this.showDepartmentModal = false;
    await this.loadDepartments();
  }

  async deleteDepartment(id: number) {
    if (!confirm('Are you sure you want to delete this department?')) return;
    await this.departmentsService.delete(id);
    await this.loadDepartments();
  }

  closeDepartmentModal() { this.showDepartmentModal = false; }

  // Subjects
  openSubjectModal() {
  this.showSubjectModal = true;
  this.newSubject = { title: '', academicYear: '', description: '', professorId: 0 };
}

  async createSubject() {
  if (!this.newSubject.title) {
    alert('Please fill in the subject title');
    return;
  }
  if (!this.newSubject.professorId || this.newSubject.professorId <= 0) {
    alert('Please select a professor');
    return;
  }

  await this.subjectsService.create(this.newSubject);
  this.showSubjectModal = false;
  await this.loadSubjects();
}

  async deleteSubject(id: number) {
    if (!confirm('Are you sure you want to delete this subject?')) return;
    await this.subjectsService.delete(id);
    await this.loadSubjects();
  }

  closeSubjectModal() { this.showSubjectModal = false; }

  logout() {
    this.authService.logout();
  }
}
