import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { SupabaseService } from '../../../core/services/supabase.service';

interface PendingProfessor {
  id: string;
  user_id: string;
  title: string;
  department_id: string;
  is_approved: boolean;
  user: {
    first_name: string;
    last_name: string;
    email: string;
    age: number;
  };
  department?: {      
    name: string;
    code: string;
  };
}

interface Department {
  id: string;
  name: string;
  code: string;
  description: string;
}

interface Subject {
  id: string;
  title: string;
  academic_year: string;
  description: string;
  total_classes: number;
  professor?: {
    user?: {
      first_name: string;
      last_name: string;
    };
  };
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  adminName = '';
  adminId = '';
  pendingProfessors: PendingProfessor[] = [];
  departments: Department[] = [];
  approvedProfessors: any[] = [];
  subjects: Subject[] = [];
  loading = true;
  activeTab: 'approvals' | 'departments' | 'subjects' = 'approvals';

  showDepartmentModal = false;
  showSubjectModal = false;
  newDepartment = {
    name: '',
    code: '',
    description: ''
  };
  newSubject = {
    title: '',
    academic_year: '',
    description: '',
    professor_id: '',
    total_classes: 15
  };

  constructor(
    public authService: AuthService,
    private supabase: SupabaseService
  ) {}

  async ngOnInit() {
    await this.loadAdminData();
  }

  async loadAdminData() {
    this.loading = true;
    const user = this.authService.getUserProfile();

    if (user) {
      this.adminName = `${user.first_name} ${user.last_name}`;
      this.adminId = user.id;
      await Promise.all([
        this.loadPendingProfessors(),
        this.loadDepartments(),
        this.loadApprovedProfessors(),
        this.loadSubjects()
      ]);
    }
    this.loading = false;
  }

  async loadPendingProfessors() {
    const { data, error } = await this.supabase.client
      .from('professors')
      .select(`
        id,
        user_id,
        title,
        department_id,
        is_approved,
        user:users!inner (
          first_name,
          last_name,
          email,
          age
        ),
        department:departments (
          name,
          code
        )
      `)
      .eq('is_approved', false);

    if (data) {
      this.pendingProfessors = data.map((p: any) => ({
        id: p.id,
        user_id: p.user_id,
        title: p.title,
        department_id: p.department_id,
        is_approved: p.is_approved,
        user: p.user,
        department: p.department
      }));
    }
  }

  async loadApprovedProfessors() {
    const { data, error } = await this.supabase.client
      .from('professors')
      .select(`
        id,
        user:users!inner (
          first_name,
          last_name
        )
      `)
      .eq('is_approved', true);

    if (data) {
      this.approvedProfessors = data;
    }
  }

  async loadDepartments() {
    const { data, error } = await this.supabase.client
      .from('departments')
      .select('*')
      .order('name');

    if (data) {
      this.departments = data;
    }
  }

  async loadSubjects() {
  const { data, error } = await this.supabase.client
    .from('subjects')
    .select(`
      id,
      title,
      academic_year,
      description,
      total_classes,
      professor:professors (
        user:users (
          first_name,
          last_name
        )
      )
    `)
    .order('academic_year', { ascending: false });


     if (error) {
    console.error(error);
    return;
  }

  if (data) {
    this.subjects = data.map((s: any) => {
      // Supabase might give arrays — normalize to single objects.
      const rawProfessor = Array.isArray(s.professor) ? s.professor[0] : s.professor;
      const rawUser = rawProfessor?.user;
      const userObj = Array.isArray(rawUser) ? rawUser[0] : rawUser;

      return {
        id: s.id,
        title: s.title,
        academic_year: s.academic_year,
        description: s.description,
        total_classes: s.total_classes,
        professor: rawProfessor
          ? {
              user: userObj
                ? {
                    first_name: userObj.first_name,
                    last_name: userObj.last_name
                  }
                : undefined
            }
          : undefined
      } as Subject;
    });
  }
  }

  async approveProfessor(professor: PendingProfessor) {
    const { error } = await this.supabase.client
      .from('professors')
      .update({
        is_approved: true,
        approval_date: new Date().toISOString(),
        approved_by_admin_id: this.adminId,
        approved_by_admin_name: this.adminName
      })
      .eq('id', professor.id);

    if (!error) {
      await this.loadPendingProfessors();
      await this.loadApprovedProfessors();
    }
  }

  async rejectProfessor(professor: PendingProfessor) {
    if (!confirm('Are you sure you want to reject this professor application? This will delete their account.')) {
      return;
    }

    const { error } = await this.supabase.client
      .from('users')
      .delete()
      .eq('id', professor.user_id);

    if (!error) {
      await this.loadPendingProfessors();
    }
  }

  openDepartmentModal() {
    this.showDepartmentModal = true;
    this.newDepartment = {
      name: '',
      code: '',
      description: ''
    };
  }

  async createDepartment() {
    if (!this.newDepartment.name || !this.newDepartment.code) {
      alert('Please fill in all required fields');
      return;
    }

    const { error } = await this.supabase.client
      .from('departments')
      .insert(this.newDepartment);

    if (!error) {
      this.showDepartmentModal = false;
      await this.loadDepartments();
    } else {
      alert('Error creating department: ' + error.message);
    }
  }

  openSubjectModal() {
    this.showSubjectModal = true;
    this.newSubject = {
      title: '',
      academic_year: '',
      description: '',
      professor_id: '',
      total_classes: 15
    };
  }

  async createSubject() {
    if (!this.newSubject.title || !this.newSubject.academic_year || !this.newSubject.professor_id) {
      alert('Please fill in all required fields');
      return;
    }

    const { error } = await this.supabase.client
      .from('subjects')
      .insert(this.newSubject);

    if (!error) {
      this.showSubjectModal = false;
      await this.loadSubjects();
    } else {
      alert('Error creating subject: ' + error.message);
    }
  }

  async deleteDepartment(id: string) {
    if (!confirm('Are you sure you want to delete this department?')) {
      return;
    }

    const { error } = await this.supabase.client
      .from('departments')
      .delete()
      .eq('id', id);

    if (!error) {
      await this.loadDepartments();
    } else {
      alert('Cannot delete department: ' + error.message);
    }
  }

  async deleteSubject(id: string) {
    if (!confirm('Are you sure you want to delete this subject?')) {
      return;
    }

    const { error } = await this.supabase.client
      .from('subjects')
      .delete()
      .eq('id', id);

    if (!error) {
      await this.loadSubjects();
    } else {
      alert('Cannot delete subject: ' + error.message);
    }
  }

  closeDepartmentModal() {
    this.showDepartmentModal = false;
  }

  closeSubjectModal() {
    this.showSubjectModal = false;
  }

  logout() {
    this.authService.logout();
  }
}
