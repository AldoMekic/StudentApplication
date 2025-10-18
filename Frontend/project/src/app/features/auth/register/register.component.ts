import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { SupabaseService } from '../../../core/services/supabase.service';

interface Department {
  id: string;
  name: string;
  code: string;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  accountType: 'student' | 'professor' = 'student';
  email = '';
  password = '';
  confirmPassword = '';
  firstName = '';
  lastName = '';
  age: number | null = null;
  departmentId = '';
  yearOfStudy: number | null = null;
  title = '';
  error = '';
  success = '';
  loading = false;
  departments: Department[] = [];

  constructor(
    private authService: AuthService,
    private supabase: SupabaseService,
    private router: Router
  ) {}

  async ngOnInit() {
    await this.loadDepartments();
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

  async onSubmit() {
    this.error = '';
    this.success = '';

    if (!this.email || !this.password || !this.firstName || !this.lastName || !this.age || !this.departmentId) {
      this.error = 'Please fill in all required fields';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    if (this.password.length < 6) {
      this.error = 'Password must be at least 6 characters';
      return;
    }

    if (this.accountType === 'student' && !this.yearOfStudy) {
      this.error = 'Please select your year of study';
      return;
    }

    if (this.accountType === 'professor' && !this.title) {
      this.error = 'Please enter your academic title';
      return;
    }

    this.loading = true;

    try {
      if (this.accountType === 'student') {
        await this.authService.registerStudent(
          this.email,
          this.password,
          this.firstName,
          this.lastName,
          this.age,
          this.departmentId,
          this.yearOfStudy!
        );
        this.success = 'Registration successful! Redirecting to login...';
        setTimeout(() => this.router.navigate(['/login']), 2000);
      } else {
        await this.authService.registerProfessor(
          this.email,
          this.password,
          this.firstName,
          this.lastName,
          this.age,
          this.title,
          this.departmentId
        );
        this.success = 'Registration successful! Your professor account is pending admin approval. You will be notified once approved.';
        setTimeout(() => this.router.navigate(['/login']), 4000);
      }
    } catch (err: any) {
      this.error = err.message || 'Registration failed';
    } finally {
      this.loading = false;
    }
  }
}
