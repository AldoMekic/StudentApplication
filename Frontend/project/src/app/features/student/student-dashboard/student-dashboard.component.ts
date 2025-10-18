import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { SupabaseService } from '../../../core/services/supabase.service';

interface Subject {
  id: string;
  title: string;
  academic_year: string;
  description: string;
  professor_name?: string;
}

interface Enrollment {
  id: string;
  status: string;
  enrolled_at: string;
  subject: Subject;
  attendance_percentage?: number;
}

interface Grade {
  id: string;
  grade_value: number;
  final_score: number;
  subject_name: string;
  professor_name: string;
  assigned_date: string;
  annulment_requested: boolean;
  can_request_annulment: boolean;
}

@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './student-dashboard.component.html',
  styleUrls: ['./student-dashboard.component.css']
})
export class StudentDashboardComponent implements OnInit {
  studentName = '';
  enrollments: Enrollment[] = [];
  grades: Grade[] = [];
  availableSubjects: Subject[] = [];
  loading = true;
  activeTab: 'enrollments' | 'grades' | 'subjects' = 'enrollments';
  studentId = '';

  constructor(
    public authService: AuthService,
    private supabase: SupabaseService
  ) {}

  async ngOnInit() {
    await this.loadStudentData();
  }

  async loadStudentData() {
    this.loading = true;
    const user = this.authService.getUserProfile();

    if (user) {
      this.studentName = `${user.first_name} ${user.last_name}`;

      const { data: studentData } = await this.supabase.client
        .from('students')
        .select('id')
        .eq('user_id', user.id)
        .maybeSingle();

      if (studentData) {
        this.studentId = studentData.id;
        await Promise.all([
          this.loadEnrollments(),
          this.loadGrades(),
          this.loadAvailableSubjects()
        ]);
      }
    }
    this.loading = false;
  }

  async loadEnrollments() {
    const { data, error } = await this.supabase.client
      .from('enrollments')
      .select(`
        id,
        status,
        enrolled_at,
        subject:subjects (
          id,
          title,
          academic_year,
          description
        )
      `)
      .eq('student_id', this.studentId)
      .order('enrolled_at', { ascending: false });

    if (data) {
      this.enrollments = data.map((e: any) => ({
        ...e,
        subject: e.subject
      }));

      for (const enrollment of this.enrollments) {
        enrollment.attendance_percentage = await this.calculateAttendancePercentage(enrollment.id);
      }
    }
  }

  async calculateAttendancePercentage(enrollmentId: string): Promise<number> {
    const { data: attendanceRecords } = await this.supabase.client
      .from('attendance_records')
      .select('was_present')
      .eq('enrollment_id', enrollmentId);

    if (!attendanceRecords || attendanceRecords.length === 0) return 0;

    const presentCount = attendanceRecords.filter(r => r.was_present).length;
    return Math.round((presentCount / attendanceRecords.length) * 100);
  }

  async loadGrades() {
    const { data, error } = await this.supabase.client
      .from('grades')
      .select('*')
      .eq('student_id', this.studentId)
      .order('assigned_date', { ascending: false });

    if (data) {
      this.grades = data.map((grade: any) => {
        const assignedDate = new Date(grade.assigned_date);
        const daysSinceAssignment = Math.floor((Date.now() - assignedDate.getTime()) / (1000 * 60 * 60 * 24));
        return {
          ...grade,
          can_request_annulment: daysSinceAssignment <= 3 && !grade.annulment_requested
        };
      });
    }
  }

  async loadAvailableSubjects() {
    const { data, error } = await this.supabase.client
      .from('subjects')
      .select(`
        id,
        title,
        academic_year,
        description,
        professor:professors (
          user:users (
            first_name,
            last_name
          )
        )
      `);

    if (data) {
      const enrolledSubjectIds = this.enrollments.map(e => e.subject.id);
      this.availableSubjects = data
        .filter((s: any) => !enrolledSubjectIds.includes(s.id))
        .map((s: any) => ({
          id: s.id,
          title: s.title,
          academic_year: s.academic_year,
          description: s.description,
          professor_name: s.professor?.user ? `${s.professor.user.first_name} ${s.professor.user.last_name}` : 'N/A'
        }));
    }
  }

  async enrollInSubject(subjectId: string) {
    const { error } = await this.supabase.client
      .from('enrollments')
      .insert({
        student_id: this.studentId,
        subject_id: subjectId,
        status: 'attending'
      });

    if (!error) {
      await this.loadEnrollments();
      await this.loadAvailableSubjects();
    }
  }

  async dropSubject(enrollmentId: string) {
    const { error } = await this.supabase.client
      .from('enrollments')
      .update({ status: 'dropped' })
      .eq('id', enrollmentId);

    if (!error) {
      await this.loadEnrollments();
    }
  }

  async requestAnnulment(gradeId: string) {
    const { error } = await this.supabase.client
      .from('grades')
      .update({
        annulment_requested: true,
        annulment_request_date: new Date().toISOString()
      })
      .eq('id', gradeId);

    if (!error) {
      await this.loadGrades();
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'attending': return 'status-attending';
      case 'completed': return 'status-completed';
      case 'dropped': return 'status-dropped';
      default: return '';
    }
  }

  logout() {
    this.authService.logout();
  }
}
