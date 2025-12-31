import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

interface Department {
  id: string | number;
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
  username = '';
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
    private router: Router
  ) {}

  async ngOnInit() {
    // For now, departments will be filled from Admin dashboard section (we’ll wire student-related after Step 2+)
  }

  async onSubmit() {
    this.error = '';
    this.success = '';

    if (!this.username || !this.email || !this.password) {
      this.error = 'Please fill in username, email and password';
      return;
    }
    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    this.loading = true;
    try {
      await this.authService.registerUser({
        username: this.username,
        email: this.email,
        password: this.password,
        isStudent: this.accountType === 'student',
        isProfessor: this.accountType === 'professor'
      });

      this.success = 'Registration successful! Redirecting to login...';
      setTimeout(() => this.router.navigate(['/login']), 1500);
    } catch (err: any) {
      this.error = err?.message || 'Registration failed';
    } finally {
      this.loading = false;
    }
  }
}
