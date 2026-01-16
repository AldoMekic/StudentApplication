import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { StudentsService, SubjectResponseDTO} from '../../../core/services/students.service';
import { SubjectsService } from '../../../core/services/subjects.service';
import { GradesService , GradeResponseDTO } from '../../../core/services/grades.service';
import { EnrollmentsService, EnrollmentResponseDTO } from '../../../core/services/enrollments.service';

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
  gradeByEnrollmentId = new Map<number, GradeResponseDTO>();

  constructor(
    public authService: AuthService,
    private studentsService: StudentsService,
    private subjectsService: SubjectsService,
    private gradesService: GradesService,
    private enrollmentsService: EnrollmentsService
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
        this.loadGrades(),            // ✅ now uses gradesService.getMyGrades()
        this.loadAvailableSubjects()
      ]);
    } finally {
      this.loading = false;
    }
  }

  private normalizeStatus(s: any): 'attending' | 'completed' | 'dropped' {
  const v = String(s ?? '').toLowerCase();
  if (v === 'completed') return 'completed';
  if (v === 'dropped') return 'dropped';
  return 'attending';
}

  async loadEnrollments() {
  const allEnrollments = await this.enrollmentsService.getAll();

  const myEnrollments = allEnrollments.filter(e => {
    const sid = e.studentId ?? e.student_id;
    return sid === this.studentId!;
  });

  const allSubjects = await this.subjectsService.getAll();
  const subjectById = new Map(allSubjects.map(s => [s.id, s]));

  this.enrollments = myEnrollments
    .filter(e => {
      const subjectId = e.subjectId ?? e.subject_id;
      return !!(subjectId && subjectById.get(subjectId));
    })
    .map(e => {
      const subjectId = (e.subjectId ?? e.subject_id)!;
      const subject = subjectById.get(subjectId)!;

      return {
        id: e.id,
        status: this.normalizeStatus(e.status),
        enrolled_at: '',   // keep as you do
        subject
      } satisfies EnrollmentCard;
    });
}
  async loadGrades() {
  const grades = await this.gradesService.getMyGrades();

  this.gradeByEnrollmentId = new Map<number, GradeResponseDTO>();
  for (const g of grades) {
    if (g.enrollmentId != null) {
      this.gradeByEnrollmentId.set(g.enrollmentId, g);
    }
  }

  // keep your existing table mapping
  this.grades = grades.map(g => ({
    ...g,
    subject_name: g.subjectName ?? '',
    professor_name: g.professorName ?? '',
    assigned_date: g.assignedAt,
    grade_value: g.officialGrade,
    final_score: g.totalScore,
    can_request_annulment: g.canRequestAnnulment,
    annulment_requested: g.annulmentRequested
  }));
}

  async loadAvailableSubjects() {
    const all = await this.subjectsService.getAll();
    const enrolledIds = new Set<number>(this.enrollments.map(e => e.subject?.id).filter((x): x is number => !!x));
    this.availableSubjects = all.filter(s => !enrolledIds.has(s.id));
  }

  async enrollInSubject(subjectId: number) {
  if (this.studentId == null) return;

  await this.enrollmentsService.create({
    studentId: this.studentId,
    subjectId
  });

  await this.loadEnrollments();
  await this.loadAvailableSubjects();
}

async dropSubject(enrollmentId: number) {
  await this.enrollmentsService.drop(enrollmentId);
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

  async requestAnnulment(gradeId: number) {
    try {
      await this.gradesService.requestAnnulment(gradeId);
      await this.loadGrades();
      alert('Annulment request submitted.');
    } catch (err: any) {
      console.error('[StudentDashboard] requestAnnulment failed', err);
      alert(err?.error ?? 'Failed to request annulment.');
    }
  }

  logout() {
    this.authService.logout();
  }
}
