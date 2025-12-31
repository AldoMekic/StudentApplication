import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { firstValueFrom } from 'rxjs';
import { API_BASE } from './api.config';
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
  private base = API_BASE + 'api/professors/';

    constructor(private http: HttpClient) {}

  async getAll(): Promise<ProfessorResponseDTO[]> {
    return await firstValueFrom(this.http.get<ProfessorResponseDTO[]>(this.base + 'getAllProfessors'));
  }

  getById(id: number) {
    return firstValueFrom(this.http.get<ProfessorResponseDTO>(`api/professors/getProfessorById/${id}`));
  }

  getByName(name: string) {
    return firstValueFrom(this.http.get<ProfessorResponseDTO>(`api/professors/getProfessorByName/${encodeURIComponent(name)}`));
  }

  async getProfessorSubjects(professorId: number): Promise<SubjectResponseDTO[]> {
    return await firstValueFrom(
      this.http.get<SubjectResponseDTO[]>(this.base + `getProfessorSubjects/${professorId}`)
    );
  }

  // The commented “add subject to professor” doesn’t exist now; we’ll skip until exposed.
  removeProfessorSubject(professorId: number, subjectId: number) {
    return firstValueFrom(this.http.delete(`api/professors/removeProfessorSubject/${professorId}/${subjectId}`));
  }
}
