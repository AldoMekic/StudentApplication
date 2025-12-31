import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  readonly base = environment.apiBaseUrl.replace(/\/+$/, '');

  constructor(private http: HttpClient) {}

  get<T>(path: string, options?: any) {
    return this.http.get<T>(`${this.base}/${path.replace(/^\/+/, '')}`, options);
  }

  post<T>(path: string, body: any, options?: any) {
    return this.http.post<T>(`${this.base}/${path.replace(/^\/+/, '')}`, body, options);
  }

  put<T>(path: string, body: any, options?: any) {
    return this.http.put<T>(`${this.base}/${path.replace(/^\/+/, '')}`, body, options);
  }

  delete<T>(path: string, options?: any) {
    return this.http.delete<T>(`${this.base}/${path.replace(/^\/+/, '')}`, options);
  }
}
