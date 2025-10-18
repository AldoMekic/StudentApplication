import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { ApiService } from './api.service';
import { API_BASE } from './api.config';
import { HttpClient } from '@angular/common/http';

// Minimal shapes mirroring your backend DTO usage
export interface UserResponseDTO {
  id: number;
  username: string;
  email: string;
  isStudent: boolean;
  isProfessor: boolean;
}

export interface CurrentAuthState {
  token: string | null;
  username: string | null;
  email: string | null;
  is_student: boolean;
  is_professor: boolean;
  // admin role is not encoded in token by your backend; default: false
  is_admin: boolean;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private authStateSubject = new BehaviorSubject<CurrentAuthState>({
    token: localStorage.getItem('jwt_token'),
    username: localStorage.getItem('jwt_username'),
    email: localStorage.getItem('jwt_email'),
    is_student: localStorage.getItem('jwt_is_student') === 'true',
    is_professor: localStorage.getItem('jwt_is_professor') === 'true',
    is_admin: localStorage.getItem('jwt_is_admin') === 'true'
  });

  authState$ = this.authStateSubject.asObservable();

  constructor(
    private api: ApiService,
    private router: Router,
    private http: HttpClient
  ) {}

  // --- helpers ---
  private decodeJwt(token: string): any {
    try {
      const payload = token.split('.')[1];
      const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      return JSON.parse(decodeURIComponent(escape(json)));
    } catch {
      return null;
    }
  }

  private setAuthFromToken(token: string) {
    const payload = this.decodeJwt(token);

    // Your backend sets claims: sub (username), email, is_student, is_professor
    const username = payload?.sub ?? null;
    const email = payload?.email ?? null;
    const is_student = (payload?.is_student ?? 'false').toString() === 'true';
    const is_professor = (payload?.is_professor ?? 'false').toString() === 'true';

    // No admin claim in backend – keep false
    const is_admin = false;

    localStorage.setItem('jwt_token', token);
    if (username) localStorage.setItem('jwt_username', username);
    if (email) localStorage.setItem('jwt_email', email);
    localStorage.setItem('jwt_is_student', String(is_student));
    localStorage.setItem('jwt_is_professor', String(is_professor));
    localStorage.setItem('jwt_is_admin', String(is_admin));

    this.authStateSubject.next({
      token,
      username,
      email,
      is_student,
      is_professor,
      is_admin
    });
  }

  isAuthenticated(): boolean {
    return !!this.authStateSubject.value.token;
  }

  hasRole(required: string[]): boolean {
    const s = this.authStateSubject.value;
    const roles: string[] = [];
    if (s.is_student) roles.push('student');
    if (s.is_professor) roles.push('professor');
    if (s.is_admin) roles.push('admin'); // currently false unless you decide otherwise

    return required.some(r => roles.includes(r));
  }

  getUserProfile() {
    // Keep the shape components expect (first_name/last_name not available here; using username/email)
    const s = this.authStateSubject.value;
    if (!s.token) return null;
    return {
      id: s.username ?? '', // we don’t have numeric ID in token; backend uses username
      email: s.email ?? '',
      role: s.is_student ? 'student' : (s.is_professor ? 'professor' : (s.is_admin ? 'admin' : 'student')),
      first_name: s.username ?? '',
      last_name: '',
      age: undefined
    };
  }

  async registerUser(dto: {
    username: string;
    email: string;
    password: string;
    // backend UserRequestDTO can include booleans for role flags; we’ll default to student
    isStudent?: boolean;
    isProfessor?: boolean;
  }) {
    // defaults: create student account unless caller overrides
    const payload = {
      Username: dto.username,
      Email: dto.email,
      Password: dto.password,
      IsStudent: dto.isStudent ?? true,
      IsProfessor: dto.isProfessor ?? false
    };

    // POST /Users/register
    return await this.api.post<any>('Users/register', payload).toPromise();
  }

  async login(email: string, password: string) {
  const result = await firstValueFrom(
    this.http.post<{ token: string }>(
      API_BASE + 'api/auth/login',
      { email, password }
    )
  );

    if (!result?.token) throw new Error('Login failed');
  this.setAuthFromToken(result.token);

    // Navigate according to role flags present in JWT
    const s = this.authStateSubject.value;
    if (s.is_student) this.router.navigate(['/student']);
    else if (s.is_professor) this.router.navigate(['/professor']);
    else if (s.is_admin) this.router.navigate(['/admin']);
    else this.router.navigate(['/login']);
  }

  async logout() {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('jwt_username');
    localStorage.removeItem('jwt_email');
    localStorage.removeItem('jwt_is_student');
    localStorage.removeItem('jwt_is_professor');
    localStorage.removeItem('jwt_is_admin');

    this.authStateSubject.next({
      token: null, username: null, email: null,
      is_student: false, is_professor: false, is_admin: false
    });
    this.router.navigate(['/login']);
  }
}