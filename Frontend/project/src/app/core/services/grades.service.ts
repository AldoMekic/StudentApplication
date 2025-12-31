import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { firstValueFrom } from 'rxjs';
import { API_BASE } from './api.config';
import { HttpClient } from '@angular/common/http';

export interface CreateGradeRequest {
  studentId: number;
  subjectId: number;
  professorId: number;
  gradeValue: number;
  finalScore: number;
  // add/rename fields if your .NET DTO needs different names
}

export interface GradeResponseDTO {
  id: number;
  // adapt
}

export interface GradeRequestDTO {
  // adapt to your backend DTO
}

@Injectable({ providedIn: 'root' })
export class GradesService {
  private base = API_BASE + 'api/grades/';

  constructor(private http: HttpClient) {}

  getAll() {
    return firstValueFrom(this.http.get<GradeResponseDTO[]>('api/grades/getAllGrades'));
  }

  getById(id: number) {
    return firstValueFrom(this.http.get<GradeResponseDTO>(`api/grades/getGradeById/${id}`));
  }

  async create(body: CreateGradeRequest): Promise<void> {
    await firstValueFrom(this.http.post<void>(this.base, body));
  }

  delete(id: number) {
    return firstValueFrom(this.http.delete(`api/grades/deleteGrade/${id}`));
  }
}
