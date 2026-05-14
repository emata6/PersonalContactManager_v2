import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { MessageService } from 'primeng/api';
import { catchError, map, of, switchMap, tap, withLatestFrom } from 'rxjs';
import { ContactsActions } from './contacts.actions';
import { ContactsService } from '../../core/services/contacts.service';
import { selectSearchParams } from './contacts.reducer';

@Injectable()
export class ContactsEffects {
  private actions$ = inject(Actions);
  private service = inject(ContactsService);
  private router = inject(Router);
  private store = inject(Store);
  private messageService = inject(MessageService);

  loadContacts$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.loadContacts),
      switchMap(({ params }) =>
        this.service.getAll(params).pipe(
          map((result) => ContactsActions.loadContactsSuccess({ result })),
          catchError((err) => of(ContactsActions.loadContactsFailure({ error: err.message })))
        )
      )
    )
  );

  loadContact$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.loadContact),
      switchMap(({ id }) =>
        this.service.getById(id).pipe(
          map((contact) => ContactsActions.loadContactSuccess({ contact })),
          catchError((err) => of(ContactsActions.loadContactFailure({ error: err.message })))
        )
      )
    )
  );

  createContact$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.createContact),
      switchMap(({ request }) =>
        this.service.create(request).pipe(
          map((id) => ContactsActions.createContactSuccess({ id })),
          catchError((err) => of(ContactsActions.createContactFailure({ error: err.message })))
        )
      )
    )
  );

  createContactSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(ContactsActions.createContactSuccess),
        tap(({ id }) => this.router.navigate(['/contacts', id]))
      ),
    { dispatch: false }
  );

  updateContact$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.updateContact),
      switchMap(({ id, request }) =>
        this.service.update(id, request).pipe(
          map(() => ContactsActions.updateContactSuccess()),
          catchError((err) => of(ContactsActions.updateContactFailure({ error: err.message })))
        )
      )
    )
  );

  updateContactSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.updateContactSuccess),
      withLatestFrom(this.store.select(selectSearchParams)),
      map(([, params]) => ContactsActions.loadContacts({ params }))
    )
  );

  deleteContact$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.deleteContact),
      switchMap(({ id }) =>
        this.service.delete(id).pipe(
          map(() => ContactsActions.deleteContactSuccess({ id })),
          catchError((err) => of(ContactsActions.deleteContactFailure({ error: err.message })))
        )
      )
    )
  );

  toggleFavorite$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.toggleFavorite),
      switchMap(({ id }) =>
        this.service.toggleFavorite(id).pipe(
          map(() => ContactsActions.toggleFavoriteSuccess({ id })),
          catchError((err) => of(ContactsActions.toggleFavoriteFailure({ error: err.message })))
        )
      )
    )
  );

  // Reload list when SignalR reports a create or update
  reloadOnSignalR$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.signalRContactCreated, ContactsActions.signalRContactUpdated),
      withLatestFrom(this.store.select(selectSearchParams)),
      map(([, params]) => ContactsActions.loadContacts({ params }))
    )
  );

  reminderFired$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(ContactsActions.signalRReminderFired),
        tap(({ payload }) =>
          this.messageService.add({
            severity: 'warn',
            summary: 'Reminder',
            detail: payload.title,
            life: 8000,
          })
        )
      ),
    { dispatch: false }
  );

  exportCsv$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(ContactsActions.exportCsv),
        switchMap(() =>
          this.service.exportCsv().pipe(
            tap((blob) => {
              const url = URL.createObjectURL(blob);
              const a = document.createElement('a');
              a.href = url;
              a.download = `contacts_${new Date().toISOString().slice(0, 10)}.csv`;
              a.click();
              URL.revokeObjectURL(url);
              this.messageService.add({ severity: 'success', summary: 'Exported', detail: 'Contacts downloaded as CSV' });
            }),
            map(() => ContactsActions.exportCsvSuccess()),
            catchError((err) => {
              this.messageService.add({ severity: 'error', summary: 'Export failed', detail: err.message });
              return of(ContactsActions.exportCsvFailure({ error: err.message }));
            })
          )
        )
      )
  );

  importCsv$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.importCsv),
      switchMap(({ csvContent }) =>
        this.service.importCsv(csvContent).pipe(
          map(({ imported }) => {
            this.messageService.add({ severity: 'success', summary: 'Imported', detail: `${imported} contacts imported` });
            return ContactsActions.importCsvSuccess({ imported });
          }),
          catchError((err) => {
            this.messageService.add({ severity: 'error', summary: 'Import failed', detail: err.message });
            return of(ContactsActions.importCsvFailure({ error: err.message }));
          })
        )
      )
    )
  );

  importCsvSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.importCsvSuccess),
      withLatestFrom(this.store.select(selectSearchParams)),
      map(([, params]) => ContactsActions.loadContacts({ params }))
    )
  );

  assignTag$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.assignTag),
      switchMap(({ contactId, tagId }) =>
        this.service.assignTag(contactId, tagId).pipe(
          map(() => ContactsActions.assignTagSuccess({ contactId, tagId })),
          catchError((err) => of(ContactsActions.assignTagFailure({ error: err.message })))
        )
      )
    )
  );

  removeTag$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.removeTag),
      switchMap(({ contactId, tagId }) =>
        this.service.removeTag(contactId, tagId).pipe(
          map(() => ContactsActions.removeTagSuccess({ contactId, tagId })),
          catchError((err) => of(ContactsActions.removeTagFailure({ error: err.message })))
        )
      )
    )
  );

  assignGroup$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.assignGroup),
      switchMap(({ contactId, groupId }) =>
        this.service.assignGroup(contactId, groupId).pipe(
          map(() => ContactsActions.assignGroupSuccess({ contactId, groupId })),
          catchError((err) => of(ContactsActions.assignGroupFailure({ error: err.message })))
        )
      )
    )
  );

  removeGroup$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.removeGroup),
      switchMap(({ contactId, groupId }) =>
        this.service.removeGroup(contactId, groupId).pipe(
          map(() => ContactsActions.removeGroupSuccess({ contactId, groupId })),
          catchError((err) => of(ContactsActions.removeGroupFailure({ error: err.message })))
        )
      )
    )
  );

  addPhoneNumber$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.addPhoneNumber),
      switchMap(({ contactId, number, label }) =>
        this.service.addPhoneNumber(contactId, { number, label }).pipe(
          map(() => ContactsActions.addPhoneNumberSuccess({ contactId })),
          catchError((err) => of(ContactsActions.addPhoneNumberFailure({ error: err.message })))
        )
      )
    )
  );

  removePhoneNumber$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContactsActions.removePhoneNumber),
      switchMap(({ contactId, number, label }) =>
        this.service.removePhoneNumber(contactId, { number, label }).pipe(
          map(() => ContactsActions.removePhoneNumberSuccess({ contactId })),
          catchError((err) => of(ContactsActions.removePhoneNumberFailure({ error: err.message })))
        )
      )
    )
  );

  reloadContactAfterTagGroupChange$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        ContactsActions.assignTagSuccess, ContactsActions.removeTagSuccess,
        ContactsActions.assignGroupSuccess, ContactsActions.removeGroupSuccess,
        ContactsActions.addPhoneNumberSuccess, ContactsActions.removePhoneNumberSuccess,
      ),
      map(({ contactId }) => ContactsActions.loadContact({ id: contactId }))
    )
  );
}
