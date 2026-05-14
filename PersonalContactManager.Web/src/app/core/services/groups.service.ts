import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Group, CreateGroupRequest, UpdateGroupRequest } from '../models/group.model';
import { ContactSummary } from '../models/contact.model';

@Injectable({ providedIn: 'root' })
export class GroupsService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/groups`;

  getAll(): Observable<Group[]> {
    return this.http.get<Group[]>(this.base);
  }

  getContacts(groupId: string): Observable<ContactSummary[]> {
    return this.http.get<ContactSummary[]>(`${this.base}/${groupId}/contacts`);
  }

  create(request: CreateGroupRequest): Observable<string> {
    return this.http.post<string>(this.base, request);
  }

  update(id: string, request: UpdateGroupRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
