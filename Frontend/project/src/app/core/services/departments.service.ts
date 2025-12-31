import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { firstValueFrom } from 'rxjs';
import { API_BASE } from './api.config';
import { HttpClient } from '@angular/common/http';

export interface DepartmentResponseDTO {
  id: number;
  name: string;
  code: string;
  description?: string;
}

export interface DepartmentRequestDTO {
  name: string;
  code: string;
  description?: string;
}

@Injectable({ providedIn: 'root' })
export class DepartmentsService {
  private base = API_BASE + 'api/departments/';

  constructor(private http: HttpClient) {}

  async getAll(): Promise<DepartmentResponseDTO[]> {
    return await firstValueFrom(
      this.http.get<DepartmentResponseDTO[]>(this.base + 'getAllDepartments')
    );
  }

  async create(body: Partial<DepartmentResponseDTO>): Promise<void> {
    await firstValueFrom(this.http.post<void>(this.base + 'createDepartment', body));
  }

  async delete(id: number): Promise<void> {
    await firstValueFrom(this.http.delete<void>(this.base + `deleteDepartment/${id}`));
  }
}
