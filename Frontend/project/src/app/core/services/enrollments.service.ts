import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { firstValueFrom } from 'rxjs';
import { API_BASE } from './api.config';
import { HttpClient } from '@angular/common/http';

export interface EnrollmentResponseDTO {
  id: number;
  studentId?: number;   // server might use studentId
  student_id?: number;  // or snake_case
  subjectId?: number;
  subject_id?: number;
  status?: 'attending' | 'completed' | 'dropped';
}

export interface EnrollmentRequestDTO {
  // adapt to your DTO
  studentId?: number;
  subjectId?: number;
}

@Injectable({ providedIn: 'root' })
export class EnrollmentsService {
  private base = API_BASE + 'api/enrollments/';

  constructor(private http: HttpClient) {}

  async getAll(): Promise<EnrollmentResponseDTO[]> {
    return await firstValueFrom(this.http.get<EnrollmentResponseDTO[]>(this.base + 'getAllEnrollments'));
  }

  getById(id: number) {
    return firstValueFrom(this.http.get<EnrollmentResponseDTO>(`api/enrollments/getEnrollmentById/${id}`));
  }

  create(dto: any /* EnrollmentRequestDTO */) {
    return firstValueFrom(this.http.post('api/enrollments', dto));
  }

  delete(id: number) {
    return firstValueFrom(this.http.delete(`api/enrollments/deleteEnrollment/${id}`));
  }
}
