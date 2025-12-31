import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { ProfessorsService, SubjectResponseDTO } from '../../../core/services/professors.service';
import { EnrollmentsService, EnrollmentResponseDTO } from '../../../core/services/enrollments.service';
import { StudentsService, StudentResponseDTO } from '../../../core/services/students.service';
import { GradesService } from '../../../core/services/grades.service';

interface StudentMini {
  id: number;
  firstName?: string;
  lastName?: string;
  email?: string;
  studentNumber?: string;
}

interface RosterRow {
  enrollmentId: number | null;    // not provided by subject → students; try to keep if present
  subjectId: number;
  studentId: number;
  status: 'attending' | 'completed' | 'dropped';
  student: StudentMini;
}

@Component({
  selector: 'app-professor-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './professor-dashboard.component.html',
  styleUrls: ['./professor-dashboard.component.css']
})
export class ProfessorDashboardComponent implements OnInit {
  professorName = '';
  professorId: number | null = null;

  subjects: SubjectResponseDTO[] = [];
  selectedSubject: SubjectResponseDTO | null = null;

  enrollments: RosterRow[] = []; // students in selected subject
  loading = true;
  activeTab: 'subjects' | 'students' = 'subjects';

  // Grade modal
  showGradeModal = false;
  selectedEnrollment: RosterRow | null = null;
  gradeValue: number | null = null;
  finalScore: number | null = null;

  // For any template bindings that previously used Date inline
  maxAttendanceDate = new Date().toISOString().split('T')[0];

  constructor(
    public authService: AuthService,
    private professorsService: ProfessorsService,
    private enrollmentsService: EnrollmentsService,
    private studentsService: StudentsService,
    private gradesService: GradesService
  ) {}

  async ngOnInit() {
    await this.initProfessor();
  }

  private async initProfessor() {
    this.loading = true;
    try {
      const user = this.authService.getUserProfile();
      this.professorName = user ? (user.first_name || user.email || 'Professor') : 'Professor';

      // TEMP: load professor id from localStorage; otherwise pick the first one
      const saved = localStorage.getItem('professor_id');
      if (saved) {
        this.professorId = Number(saved);
      } else {
        const all = await this.professorsService.getAll();
        if (all && all.length) {
          this.professorId = all[0].id;
          localStorage.setItem('professor_id', String(this.professorId));
        }
      }

      if (this.professorId != null) {
        await this.loadSubjects();
      }
    } finally {
      this.loading = false;
    }
  }

  async loadSubjects() {
    if (this.professorId == null) return;
    this.subjects = await this.professorsService.getProfessorSubjects(this.professorId);
  }

  async selectSubject(subject: SubjectResponseDTO) {
    this.selectedSubject = subject;
    this.activeTab = 'students';
    await this.loadRosterForSelectedSubject();
  }

  private async loadRosterForSelectedSubject() {
    this.enrollments = [];
    if (!this.selectedSubject) return;

    // Compose roster from enrollments + per-student calls
    // GET all enrollments, then filter by subjectId
    const allEnrollments = await this.enrollmentsService.getAll();

    // Find a property name that matches backend DTO: try "subjectId" or "subject_id"
    const filtered = (allEnrollments || []).filter((e: any) => {
      const sid = (e.subjectId ?? e.subject_id);
      return sid === this.selectedSubject!.id;
    });

    // For each, fetch student info
    const rows: RosterRow[] = [];
    for (const e of filtered) {
      const maybeId = (e.studentId ?? e.student_id);
if (maybeId == null) continue; // skip if backend didn’t send a student id
const studentId = Number(maybeId);
      let studentInfo: StudentResponseDTO | null = null;
      try {
        studentInfo = await this.studentsService.getById(studentId);
      } catch {
        studentInfo = null;
      }

      // Try to map common Student shape; leave blanks if fields differ
      const mini: StudentMini = {
        id: studentId,
        firstName: (studentInfo as any)?.firstName ?? (studentInfo as any)?.name ?? '',
        lastName: (studentInfo as any)?.lastName ?? (studentInfo as any)?.surname ?? '',
        email: (studentInfo as any)?.email ?? '',
        studentNumber: (studentInfo as any)?.studentNumber ?? (studentInfo as any)?.student_number ?? ''
      };

      rows.push({
        enrollmentId: e.id ?? null,
        subjectId: this.selectedSubject.id,
        studentId,
        status: (e.status ?? 'attending') as any,
        student: mini
      });
    }

    this.enrollments = rows;
  }

  // === Grade modal ===
  openGradeModal(enrollment: RosterRow) {
    this.selectedEnrollment = enrollment;
    this.gradeValue = null;
    this.finalScore = null;
    this.showGradeModal = true;
  }

  closeGradeModal() {
    this.showGradeModal = false;
    this.selectedEnrollment = null;
    this.gradeValue = null;
    this.finalScore = null;
  }

  async submitGrade() {
    if (!this.selectedEnrollment || !this.selectedSubject || this.professorId == null) {
      alert('Missing required data.');
      return;
    }
    if (this.gradeValue == null || this.finalScore == null) {
      alert('Please fill in grade and final score.');
      return;
    }

    // Best-guess GradeRequest payload based on your controllers/mappings.
    // If your GradeRequestDTO uses different field names, tell me and I’ll align it.
    const dto: any = {
      studentId: this.selectedEnrollment.studentId,
      subjectId: this.selectedSubject.id,
      professorId: this.professorId,
      gradeValue: this.gradeValue,
      finalScore: this.finalScore,
      // Optional extras if your DTO supports them:
      // enrollmentId: this.selectedEnrollment.enrollmentId,
      // subjectName: this.selectedSubject.title,
      // professorName: this.professorName
    };

    try {
      await this.gradesService.create(dto);
      this.closeGradeModal();
      // Optionally refresh something here (e.g., disable the button or mark as completed)
    } catch (err: any) {
      alert('Failed to assign grade. If the backend expects different field names, I can adjust the payload.');
    }
  }

  logout() {
    this.authService.logout();
  }
}
