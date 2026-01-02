import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { StudentsService, SubjectResponseDTO, GradeResponseDTO } from '../../../core/services/students.service';
import { SubjectsService } from '../../../core/services/subjects.service';

interface EnrollmentCard {
  id: number;                // we’ll treat this as subjectId for UI purposes
  status: 'attending' | 'completed' | 'dropped';
  enrolled_at?: string;
  subject: SubjectResponseDTO;
  attendance_percentage?: number; // backend doesn’t provide; we’ll omit for now
}

type AvailableSubject = SubjectResponseDTO & { professor_name?: string };

@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './student-dashboard.component.html',
  styleUrls: ['./student-dashboard.component.css']
})
export class StudentDashboardComponent implements OnInit {
  studentName = '';
  enrollments: EnrollmentCard[] = [];
  grades: (GradeResponseDTO & {
  subject_name?: string;
  professor_name?: string;
  assigned_date?: string;
  can_request_annulment?: boolean;
  annulment_requested?: boolean;
  grade_value?: number;
  final_score?: number;
})[] = [];
  availableSubjects: AvailableSubject[] = [];
  loading = true;
  activeTab: 'enrollments' | 'grades' | 'subjects' = 'enrollments';
  studentId: number | null = null;

  constructor(
    public authService: AuthService,
    private studentsService: StudentsService,
    private subjectsService: SubjectsService
  ) {}

  async ngOnInit() {
    await this.initStudent();
  }

  private async initStudent() {
  this.loading = true;
  try {
    const user = this.authService.getUserProfile();
    this.studentName = user ? (user.first_name || user.email || 'Student') : 'Student';

    // ✅ get the actual logged-in student's DB record
    const me = await this.studentsService.getMe();
this.studentId = me.id;
this.studentName = `${me.firstName ?? ''} ${me.lastName ?? ''}`.trim() || this.studentName;

await Promise.all([
  this.loadEnrollments(),
  this.loadGrades(),
  this.loadAvailableSubjects()
]);
  } finally {
    this.loading = false;
  }
}

  async loadEnrollments() {
    if (this.studentId == null) return;
    const subjects = await this.studentsService.getStudentSubjects(this.studentId);

    // Treat "enrollments" as subjects with default status 'attending'
    this.enrollments = subjects.map(s => ({
      id: s.id,
      status: 'attending', // backend endpoint doesn’t return status; placeholder
      enrolled_at: '',     // not available
      subject: s
    }));

    // If you later expose enrollment status, map it here properly.
  }

  async loadGrades() {
    if (this.studentId == null) return;
    const grades = await this.studentsService.getStudentGrades(this.studentId);

    // We’ll keep shape simple, and compute the 3-day rule if assignedDate is present
    this.grades = grades.map(g => {
  const assignedDate = g['assignedDate'] || g['assigned_date'];
  let canRequest = false;
  if (assignedDate) {
    const d = new Date(assignedDate);
    const days = Math.floor((Date.now() - d.getTime()) / (1000 * 60 * 60 * 24));
    canRequest = days <= 3 && !g['annulment_requested'];
  }
  return {
    ...g,
    subject_name: g['subjectName'] || g['subject_name'],
    professor_name: g['professorName'] || g['professor_name'],
    assigned_date: assignedDate,
    can_request_annulment: canRequest,
    // supply snake_case for the template if backend uses camelCase
    grade_value: (g as any).grade_value ?? g.gradeValue,
    final_score: (g as any).final_score ?? g.finalScore
  };
});
  }

  async loadAvailableSubjects() {
    const all = await this.subjectsService.getAll();
    const enrolledIds = new Set<number>(this.enrollments.map(e => e.subject.id));
    this.availableSubjects = all.filter(s => !enrolledIds.has(s.id));
  }

  async enrollInSubject(subjectId: number) {
    if (this.studentId == null) return;
    await this.studentsService.addSubjectToStudent(this.studentId, subjectId);
    await this.loadEnrollments();
    await this.loadAvailableSubjects();
  }

  async dropSubject(subjectOrEnrollmentId: number) {
    if (this.studentId == null) return;
    // our "enrollment id" is actually subjectId in this simplified mapping
    await this.studentsService.removeStudentSubject(this.studentId, subjectOrEnrollmentId);
    await this.loadEnrollments();
    await this.loadAvailableSubjects();
  }

  // Keep your CSS hook names for badges
  getStatusClass(status: string): string {
    switch (status) {
      case 'attending': return 'status-attending';
      case 'completed': return 'status-completed';
      case 'dropped': return 'status-dropped';
      default: return '';
    }
  }

  // Not supported by backend yet; leave as no-op
  async requestAnnulment(_gradeId: number) {
    alert('Annulment request flow is not available on the backend yet.');
  }

  logout() {
    this.authService.logout();
  }
}
