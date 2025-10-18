import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { firstValueFrom } from 'rxjs';
import { API_BASE } from './api.config';
import { HttpClient } from '@angular/common/http';

export interface StudentResponseDTO {
  id: number;
  firstName?: string;
  lastName?: string;
  email?: string;
  studentNumber?: string;
  student_number?: string;
}


export interface SubjectResponseDTO {
  id: number;
  title: string;
  academicYear?: string;
  academic_year?: string;
  description?: string;
}

export interface GradeResponseDTO {
  id: number;
  studentId?: number;
  subjectId?: number;
  professorId?: number;
  gradeValue?: number;
  finalScore?: number;
  subjectName?: string;
  subject_name?: string;
  professorName?: string;
  professor_name?: string;
  assignedDate?: string;
  assigned_date?: string;
  annulment_requested?: boolean;
}

@Injectable({ providedIn: 'root' })
export class StudentsService {
  private base = API_BASE + 'api/students/';

  constructor(private http: HttpClient) {}

  async getAll(): Promise<StudentResponseDTO[]> {
    return await firstValueFrom(this.http.get<StudentResponseDTO[]>(this.base + 'getAllStudents'));
  }

   async getById(id: number): Promise<StudentResponseDTO> {
    return await firstValueFrom(this.http.get<StudentResponseDTO>(this.base + `getStudentById/${id}`));
  }

  async getStudentSubjects(studentId: number): Promise<SubjectResponseDTO[]> {
    return await firstValueFrom(
      this.http.get<SubjectResponseDTO[]>(this.base + `getStudentSubjects/${studentId}`)
    );
  }

  async getStudentGrades(studentId: number): Promise<GradeResponseDTO[]> {
    return await firstValueFrom(
      this.http.get<GradeResponseDTO[]>(this.base + `getStudentGrades/${studentId}`)
    );
  }

  async addSubjectToStudent(studentId: number, subjectId: number): Promise<void> {
    // POST api/students/addSubjectsToStudent with body { studentId, subjectIds:[subjectId] }
    await firstValueFrom(
      this.http.post<void>(this.base + 'addSubjectsToStudent', {
        studentId,
        subjectIds: [subjectId]
      })
    );
  }

  async removeStudentSubject(studentId: number, subjectId: number): Promise<void> {
    // DELETE api/students/removeStudentSubject/{studentId}/{subjectId}
    await firstValueFrom(
      this.http.delete<void>(this.base + `removeStudentSubject/${studentId}/${subjectId}`)
    );
  }
}