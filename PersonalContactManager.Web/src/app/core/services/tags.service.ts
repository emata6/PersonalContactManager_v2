import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Tag, CreateTagRequest, UpdateTagRequest } from '../models/tag.model';

@Injectable({ providedIn: 'root' })
export class TagsService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/tags`;

  getAll(): Observable<Tag[]> {
    return this.http.get<Tag[]>(this.base);
  }

  create(request: CreateTagRequest): Observable<string> {
    return this.http.post<string>(this.base, request);
  }

  update(id: string, request: UpdateTagRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
