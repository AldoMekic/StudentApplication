import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { firstValueFrom } from 'rxjs';
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
  constructor(private api: ApiService) {}

  getAll() {
    return firstValueFrom(this.api.get<GradeResponseDTO[]>('api/grades'));
  }

  getById(id: number) {
    return firstValueFrom(this.api.get<GradeResponseDTO>(`api/grades/${id}`));
  }

  create(body: CreateGradeRequest) {
    return firstValueFrom(this.api.post<void>('api/grades', body));
  }

  delete(id: number) {
    return firstValueFrom(this.api.delete<void>(`api/grades/${id}`));
  }

  requestAnnulment(id: number) {
    return firstValueFrom(this.api.post<GradeResponseDTO>(`api/grades/${id}/request-annulment`, {}));
  }
}