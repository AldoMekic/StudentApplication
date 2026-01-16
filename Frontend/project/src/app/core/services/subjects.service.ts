import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { firstValueFrom } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export interface SubjectResponseDTO {
  id: number;
  title: string;
  year?: number; 
  academic_year?: string;
  description?: string;
  professorId?: number;
}

export interface SubjectRequestDTO {
  title: string;
  academicYear?: string;
  description?: string;
  professorId: number
}

@Injectable({ providedIn: 'root' })
export class SubjectsService {
  constructor(private api: ApiService) {}

  getAll() {
    return firstValueFrom(this.api.get<SubjectResponseDTO[]>('api/subjects'));
  }

  create(body: SubjectRequestDTO) {
  const payload = {
    title: body.title,
    description: body.description,
    year: Number(body.academicYear) || 1,     // map academicYear -> year
    professorId: body.professorId
  };
  return firstValueFrom(this.api.post<void>('api/subjects', payload));
}

  delete(id: number) {
    return firstValueFrom(this.api.delete(`api/subjects/${id}`));
  }
}
