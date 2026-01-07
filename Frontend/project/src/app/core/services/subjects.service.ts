import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { firstValueFrom } from 'rxjs';
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
  professorId: number
}

@Injectable({ providedIn: 'root' })
export class SubjectsService {
  constructor(private api: ApiService) {}

  getAll() {
    return firstValueFrom(this.api.get<SubjectResponseDTO[]>('api/subjects'));
  }

  create(dto: SubjectRequestDTO) {
    return firstValueFrom(this.api.post('api/subjects', dto));
  }

  delete(id: number) {
    return firstValueFrom(this.api.delete(`api/subjects/${id}`));
  }
}
