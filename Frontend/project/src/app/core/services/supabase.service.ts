import { Injectable } from '@angular/core';
import { createClient, SupabaseClient, User } from '@supabase/supabase-js';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SupabaseService {
  private supabase: SupabaseClient;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$: Observable<User | null> = this.currentUserSubject.asObservable();

  constructor() {
    this.supabase = createClient(
      'https://pepnqrtrgchgzsfrmqky.supabase.co',
      'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBlcG5xcnRyZ2NoZ3pzZnJtcWt5Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjA3MjkzNzUsImV4cCI6MjA3NjMwNTM3NX0.QWewObQjm6xub4L7KuvKEaqUMahBeD5dH_FLHNtvLSU'
    );

    this.supabase.auth.onAuthStateChange((event, session) => {
      (async () => {
        this.currentUserSubject.next(session?.user ?? null);
      })();
    });

    this.supabase.auth.getSession().then(({ data: { session } }) => {
      this.currentUserSubject.next(session?.user ?? null);
    });
  }

  get client(): SupabaseClient {
    return this.supabase;
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  async signUp(email: string, password: string) {
    return await this.supabase.auth.signUp({ email, password });
  }

  async signIn(email: string, password: string) {
    return await this.supabase.auth.signInWithPassword({ email, password });
  }

  async signOut() {
    return await this.supabase.auth.signOut();
  }

  async getSession() {
    return await this.supabase.auth.getSession();
  }
}
