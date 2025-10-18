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
  total_classes: number;
}

interface Student {
  id: string;
  student_number: string;
  user: {
    first_name: string;
    last_name: string;
    email: string;
  };
}

interface Enrollment {
  id: string;
  student_id: string;
  status: string;
  student: Student;
  attendance_percentage?: number;
  attendance_records?: AttendanceRecord[];
}

interface AttendanceRecord {
  id: string;
  class_date: string;
  was_present: boolean;
  activity_comment: string;
  recorded_at: string;
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
  professorId = '';
  subjects: Subject[] = [];
  selectedSubject: Subject | null = null;
  enrollments: Enrollment[] = [];
  loading = true;
  activeTab: 'subjects' | 'students' | 'attendance' | 'grades' = 'subjects';

  showAttendanceModal = false;
  attendanceDate = new Date().toISOString().split('T')[0];
  today = new Date().toISOString().split('T')[0];
  
  attendanceData: { [key: string]: { present: boolean; comment: string } } = {};

  showGradeModal = false;
  selectedEnrollment: Enrollment | null = null;
  gradeValue: number | null = null;
  finalScore: number | null = null;

  constructor(
    public authService: AuthService,
    private supabase: SupabaseService
  ) {}

  async ngOnInit() {
    await this.loadProfessorData();
  }

  async loadProfessorData() {
    this.loading = true;
    const user = this.authService.getUserProfile();

    if (user) {
      this.professorName = `${user.first_name} ${user.last_name}`;

      const { data: professorData } = await this.supabase.client
        .from('professors')
        .select('id')
        .eq('user_id', user.id)
        .maybeSingle();

      if (professorData) {
        this.professorId = professorData.id;
        await this.loadSubjects();
      }
    }
    this.loading = false;
  }

  async loadSubjects() {
    const { data, error } = await this.supabase.client
      .from('subjects')
      .select('*')
      .eq('professor_id', this.professorId)
      .order('academic_year', { ascending: false });

    if (data) {
      this.subjects = data;
    }
  }

  async selectSubject(subject: Subject) {
    this.selectedSubject = subject;
    await this.loadEnrollments();
    this.activeTab = 'students';
  }

  async loadEnrollments() {
    if (!this.selectedSubject) return;

    const { data, error } = await this.supabase.client
      .from('enrollments')
      .select(`
        id,
        student_id,
        status,
        student:students!inner (
          id,
          student_number,
          user:users!inner (
            first_name,
            last_name,
            email
          )
        )
      `)
      .eq('subject_id', this.selectedSubject.id);

    if (data) {
      this.enrollments = data.map((e: any) => ({
        id: e.id,
        student_id: e.student_id,
        status: e.status,
        student: {
          id: e.student.id,
          student_number: e.student.student_number,
          user: e.student.user
        }
      }));

      for (const enrollment of this.enrollments) {
        const percentage = await this.calculateAttendancePercentage(enrollment.id);
        enrollment.attendance_percentage = percentage;
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

  openAttendanceModal() {
    this.showAttendanceModal = true;
    this.attendanceDate = new Date().toISOString().split('T')[0];
    this.attendanceData = {};

    this.enrollments
      .filter(e => e.status === 'attending')
      .forEach(enrollment => {
        this.attendanceData[enrollment.id] = {
          present: false,
          comment: ''
        };
      });
  }

  async submitAttendance() {
    if (!this.selectedSubject) return;

    const classDate = new Date(this.attendanceDate);
    const now = new Date();
    const hoursSinceClass = (now.getTime() - classDate.getTime()) / (1000 * 60 * 60);

    if (hoursSinceClass > 24) {
      alert('Cannot record attendance more than 24 hours after class date');
      return;
    }

    const records = Object.entries(this.attendanceData).map(([enrollmentId, data]) => {
      const enrollment = this.enrollments.find(e => e.id === enrollmentId);
      return {
        enrollment_id: enrollmentId,
        student_id: enrollment!.student_id,
        subject_id: this.selectedSubject!.id,
        professor_id: this.professorId,
        class_date: this.attendanceDate,
        was_present: data.present,
        activity_comment: data.comment || null
      };
    });

    const { error } = await this.supabase.client
      .from('attendance_records')
      .insert(records);

    if (!error) {
      this.showAttendanceModal = false;
      await this.loadEnrollments();
    } else {
      alert('Error recording attendance: ' + error.message);
    }
  }

  async openGradeModal(enrollment: Enrollment) {
    this.selectedEnrollment = enrollment;
    this.gradeValue = null;
    this.finalScore = null;

    const { data: attendanceRecords } = await this.supabase.client
      .from('attendance_records')
      .select('*')
      .eq('enrollment_id', enrollment.id)
      .order('class_date', { ascending: false });

    if (attendanceRecords) {
      this.selectedEnrollment.attendance_records = attendanceRecords;
    }

    this.showGradeModal = true;
  }

  async submitGrade() {
    if (!this.selectedEnrollment || !this.selectedSubject || this.gradeValue === null || this.finalScore === null) {
      alert('Please fill in all fields');
      return;
    }

    if (this.gradeValue < 0 || this.gradeValue > 10) {
      alert('Grade must be between 0 and 10');
      return;
    }

    const attendancePercentage = this.selectedEnrollment.attendance_percentage || 0;
    if (attendancePercentage < 75) {
      alert('Student must have at least 75% attendance to receive a grade');
      return;
    }

    const { data: existingGrade } = await this.supabase.client
      .from('grades')
      .select('id')
      .eq('enrollment_id', this.selectedEnrollment.id)
      .maybeSingle();

    if (existingGrade) {
      alert('Grade already assigned for this enrollment');
      return;
    }

    const { error: gradeError } = await this.supabase.client
      .from('grades')
      .insert({
        enrollment_id: this.selectedEnrollment.id,
        student_id: this.selectedEnrollment.student_id,
        subject_id: this.selectedSubject.id,
        professor_id: this.professorId,
        grade_value: this.gradeValue,
        final_score: this.finalScore,
        subject_name: this.selectedSubject.title,
        professor_name: this.professorName,
        student_name: `${this.selectedEnrollment.student.user.first_name} ${this.selectedEnrollment.student.user.last_name}`
      });

    if (gradeError) {
      alert('Error assigning grade: ' + gradeError.message);
      return;
    }

    const { error: enrollmentError } = await this.supabase.client
      .from('enrollments')
      .update({ status: 'completed', completed_at: new Date().toISOString() })
      .eq('id', this.selectedEnrollment.id);

    if (!enrollmentError) {
      this.showGradeModal = false;
      await this.loadEnrollments();
    }
  }

  closeAttendanceModal() {
    this.showAttendanceModal = false;
  }

  closeGradeModal() {
    this.showGradeModal = false;
    this.selectedEnrollment = null;
  }

  logout() {
    this.authService.logout();
  }
}
