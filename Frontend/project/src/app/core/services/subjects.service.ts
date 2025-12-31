import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { firstValueFrom } from 'rxjs';
import { API_BASE } from './api.config';
import { HttpClient } from '@angular/common/http';

export interface SubjectResponseDTO {
  id: number;
  title: string;
  academicYear?: string;
  academic_year?: string;
  description?: string;
}

export interface SubjectRequestDTO {
  title: string;
  academicYear?: string;
  description?: string;
}

@Injectable({ providedIn: 'root' })
export class SubjectsService {
  private base = API_BASE + 'api/subjects/';

  constructor(private http: HttpClient) {}

  async getAll(): Promise<SubjectResponseDTO[]> {
    return await firstValueFrom(this.http.get<SubjectResponseDTO[]>(this.base + 'getAllSubjects'));
  }

  create(dto: SubjectRequestDTO) {
    return firstValueFrom(this.http.post('api/subjects', dto));
  }

  delete(id: number) {
    return firstValueFrom(this.http.delete(`api/subjects/deleteSubject/${id}`));
  }
}
