import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ContactSummary,
  ContactDetail,
  ContactSearchParams,
  CreateContactRequest,
  UpdateContactRequest,
  PhoneNumber,
} from '../models/contact.model';
import { PagedResult } from '../models/paged-result.model';

@Injectable({ providedIn: 'root' })
export class ContactsService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/contacts`;

  getAll(params: ContactSearchParams = {}): Observable<PagedResult<ContactSummary>> {
    let httpParams = new HttpParams();
    if (params.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);
    if (params.tagId) httpParams = httpParams.set('tagId', params.tagId);
    if (params.groupId) httpParams = httpParams.set('groupId', params.groupId);
    if (params.favoritesOnly != null) httpParams = httpParams.set('favoritesOnly', String(params.favoritesOnly));
    if (params.hasBirthday != null) httpParams = httpParams.set('hasBirthday', String(params.hasBirthday));
    if (params.sortBy) httpParams = httpParams.set('sortBy', params.sortBy);
    if (params.sortDirection) httpParams = httpParams.set('sortDirection', params.sortDirection);
    if (params.page) httpParams = httpParams.set('page', String(params.page));
    if (params.pageSize) httpParams = httpParams.set('pageSize', String(params.pageSize));
    return this.http.get<PagedResult<ContactSummary>>(this.base, { params: httpParams });
  }

  exportCsv(): Observable<Blob> {
    return this.http.get(`${this.base}/export`, { responseType: 'blob' });
  }

  importCsv(csvContent: string): Observable<{ imported: number }> {
    return this.http.post<{ imported: number }>(`${this.base}/import`, { csvContent });
  }

  getById(id: string): Observable<ContactDetail> {
    return this.http.get<ContactDetail>(`${this.base}/${id}`);
  }

  getFavorites(): Observable<ContactSummary[]> {
    return this.http.get<ContactSummary[]>(`${this.base}/favorites`);
  }

  getUpcomingBirthdays(daysAhead = 7): Observable<ContactSummary[]> {
    return this.http.get<ContactSummary[]>(`${this.base}/birthdays`, {
      params: new HttpParams().set('daysAhead', String(daysAhead)),
    });
  }

  create(request: CreateContactRequest): Observable<string> {
    return this.http.post<string>(this.base, request);
  }

  update(id: string, request: UpdateContactRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  restore(id: string): Observable<void> {
    return this.http.post<void>(`${this.base}/${id}/restore`, {});
  }

  toggleFavorite(id: string): Observable<void> {
    return this.http.patch<void>(`${this.base}/${id}/favorite`, {});
  }

  assignTag(contactId: string, tagId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/${contactId}/tags/${tagId}`, {});
  }

  removeTag(contactId: string, tagId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${contactId}/tags/${tagId}`);
  }

  assignGroup(contactId: string, groupId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/${contactId}/groups/${groupId}`, {});
  }

  removeGroup(contactId: string, groupId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${contactId}/groups/${groupId}`);
  }

  addPhoneNumber(contactId: string, phone: PhoneNumber): Observable<void> {
    return this.http.post<void>(`${this.base}/${contactId}/phone-numbers`, phone);
  }

  removePhoneNumber(contactId: string, phone: PhoneNumber): Observable<void> {
    return this.http.delete<void>(`${this.base}/${contactId}/phone-numbers`, { body: phone });
  }

  sendEmail(contactId: string, subject: string, body: string, replyTo?: string): Observable<void> {
    return this.http.post<void>(`${this.base}/${contactId}/email`, { subject, body, replyTo });
  }
}
