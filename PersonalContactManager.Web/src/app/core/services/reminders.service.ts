import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Reminder, CreateReminderRequest, UpdateReminderRequest } from '../models/reminder.model';

@Injectable({ providedIn: 'root' })
export class RemindersService {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  getByContact(contactId: string): Observable<Reminder[]> {
    return this.http.get<Reminder[]>(`${this.base}/contacts/${contactId}/reminders`);
  }

  getUpcoming(daysAhead = 30): Observable<Reminder[]> {
    return this.http.get<Reminder[]>(`${this.base}/reminders/upcoming`, {
      params: { daysAhead: String(daysAhead) },
    });
  }

  create(request: CreateReminderRequest): Observable<string> {
    return this.http.post<string>(`${this.base}/contacts/${request.contactId}/reminders`, request);
  }

  update(id: string, request: UpdateReminderRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/reminders/${id}`, request);
  }

  cancel(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/reminders/${id}`);
  }

  dismiss(id: string): Observable<void> {
    return this.http.post<void>(`${this.base}/reminders/${id}/dismiss`, {});
  }
}
