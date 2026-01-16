import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { firstValueFrom } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export interface ProfessorResponseDTO {
  id: number;
  firstName?: string;
  lastName?: string;
  email?: string;
  departmentId?: number;
}

export interface SubjectResponseDTO {
  id: number;
  title: string;
  academicYear?: string;   // .NET might use this
  academic_year?: string;  // fallback for client display
  description?: string;
  totalClasses?: number;
  total_classes?: number;
  professorId?: number;
}


@Injectable({ providedIn: 'root' })
export class ProfessorsService {
  constructor(private api: ApiService) {}

  getAll() {
    return firstValueFrom(this.api.get<ProfessorResponseDTO[]>('api/professors'));
  }

  getApproved() {
    return firstValueFrom(this.api.get<ProfessorResponseDTO[]>('api/professors/approved'));
  }

  getById(id: number) {
    return firstValueFrom(this.api.get<ProfessorResponseDTO>(`api/professors/${id}`));
  }

  getProfessorSubjects(professorId: number) {
    return firstValueFrom(this.api.get<SubjectResponseDTO[]>(`api/professors/${professorId}/subjects`));
  }

  removeProfessorSubject(professorId: number, subjectId: number) {
    return firstValueFrom(this.api.delete(`api/professors/${professorId}/subjects/${subjectId}`));
  }
}