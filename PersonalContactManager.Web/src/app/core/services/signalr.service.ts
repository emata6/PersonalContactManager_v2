import { Injectable, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { ContactsActions } from '../../store/contacts/contacts.actions';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private store = inject(Store);
  private connection: signalR.HubConnection | null = null;

  start(): void {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl)
      .withAutomaticReconnect()
      .build();

    this.connection.on('ContactCreated', (payload: { contactId: string }) => {
      this.store.dispatch(ContactsActions.signalRContactCreated({ id: payload.contactId }));
    });

    this.connection.on('ContactUpdated', (payload: { contactId: string }) => {
      this.store.dispatch(ContactsActions.signalRContactUpdated({ id: payload.contactId }));
    });

    this.connection.on('ContactDeleted', (payload: { contactId: string }) => {
      this.store.dispatch(ContactsActions.signalRContactDeleted({ id: payload.contactId }));
    });

    this.connection.on('ReminderFired', (payload: { reminderId: string; contactId: string; title: string }) => {
      this.store.dispatch(ContactsActions.signalRReminderFired({ payload }));
    });

    this.connection.start().catch(console.error);
  }

  stop(): void {
    this.connection?.stop();
  }
}
