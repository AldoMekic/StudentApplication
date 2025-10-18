import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { SupabaseService } from './supabase.service';

export interface UserProfile {
  id: string;
  email: string;
  role: 'student' | 'professor' | 'admin';
  first_name: string;
  last_name: string;
  age?: number;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserProfileSubject = new BehaviorSubject<UserProfile | null>(null);
  public currentUserProfile$: Observable<UserProfile | null> = this.currentUserProfileSubject.asObservable();

  constructor(
    private supabase: SupabaseService,
    private router: Router
  ) {
    this.supabase.currentUser$.subscribe(async (user) => {
      if (user) {
        await this.loadUserProfile(user.id);
      } else {
        this.currentUserProfileSubject.next(null);
      }
    });
  }

  private async loadUserProfile(userId: string) {
    const { data, error } = await this.supabase.client
      .from('users')
      .select('*')
      .eq('id', userId)
      .maybeSingle();

    if (data) {
      this.currentUserProfileSubject.next(data as UserProfile);
    }
  }

  async registerStudent(email: string, password: string, firstName: string, lastName: string, age: number, departmentId: string, yearOfStudy: number) {
    const { data: authData, error: authError } = await this.supabase.signUp(email, password);

    if (authError || !authData.user) {
      throw authError;
    }

    const { error: userError } = await this.supabase.client
      .from('users')
      .insert({
        id: authData.user.id,
        email,
        role: 'student',
        first_name: firstName,
        last_name: lastName,
        age
      });

    if (userError) throw userError;

    const { error: studentError } = await this.supabase.client
      .from('students')
      .insert({
        user_id: authData.user.id,
        department_id: departmentId,
        student_number: `STU${Date.now()}`,
        year_of_study: yearOfStudy
      });

    if (studentError) throw studentError;

    return authData;
  }

  async registerProfessor(email: string, password: string, firstName: string, lastName: string, age: number, title: string, departmentId: string) {
    const { data: authData, error: authError } = await this.supabase.signUp(email, password);

    if (authError || !authData.user) {
      throw authError;
    }

    const { error: userError } = await this.supabase.client
      .from('users')
      .insert({
        id: authData.user.id,
        email,
        role: 'professor',
        first_name: firstName,
        last_name: lastName,
        age
      });

    if (userError) throw userError;

    const { error: professorError } = await this.supabase.client
      .from('professors')
      .insert({
        user_id: authData.user.id,
        department_id: departmentId,
        title,
        is_approved: false
      });

    if (professorError) throw professorError;

    return authData;
  }

  async login(email: string, password: string) {
    const { data, error } = await this.supabase.signIn(email, password);

    if (error) throw error;

    if (data.user) {
      await this.loadUserProfile(data.user.id);
      const profile = this.currentUserProfileSubject.value;

      if (profile?.role === 'professor') {
        const { data: professorData } = await this.supabase.client
          .from('professors')
          .select('is_approved')
          .eq('user_id', data.user.id)
          .maybeSingle();

        if (!professorData?.is_approved) {
          await this.supabase.signOut();
          throw new Error('Your professor account is pending admin approval');
        }
      }

      this.navigateByRole(profile!.role);
    }

    return data;
  }

  async logout() {
    await this.supabase.signOut();
    this.router.navigate(['/login']);
  }

  getUserProfile(): UserProfile | null {
    return this.currentUserProfileSubject.value;
  }

  hasRole(roles: string[]): boolean {
    const profile = this.getUserProfile();
    return profile ? roles.includes(profile.role) : false;
  }

  private navigateByRole(role: string) {
    switch (role) {
      case 'student':
        this.router.navigate(['/student']);
        break;
      case 'professor':
        this.router.navigate(['/professor']);
        break;
      case 'admin':
        this.router.navigate(['/admin']);
        break;
      default:
        this.router.navigate(['/login']);
    }
  }
}
