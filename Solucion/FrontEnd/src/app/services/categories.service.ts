import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../models/category.model';
import { API_BASE } from './api.config';

@Injectable({ providedIn: 'root' })
export class CategoriesService {
  private url = `${API_BASE}/Categories`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.url);
  }

  getById(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.url}/${id}`);
  }

  create(category: Category): Observable<Category> {
    const dto = {
      name: category.name
    };
    return this.http.post<Category>(this.url, dto);
  }

  update(id: number, category: Partial<Category>): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, category);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
