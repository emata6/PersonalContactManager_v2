import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { catchError, map, of, switchMap, withLatestFrom } from 'rxjs';
import { RemindersActions } from './reminders.actions';
import { RemindersService } from '../../core/services/reminders.service';
import { selectCurrentContactId } from './reminders.reducer';

@Injectable()
export class RemindersEffects {
  private actions$ = inject(Actions);
  private service = inject(RemindersService);
  private store = inject(Store);

  loadReminders$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RemindersActions.loadReminders),
      switchMap(({ contactId }) =>
        this.service.getByContact(contactId).pipe(
          map((reminders) => RemindersActions.loadRemindersSuccess({ reminders })),
          catchError((err) => of(RemindersActions.loadRemindersFailure({ error: err.message })))
        )
      )
    )
  );

  loadAllReminders$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RemindersActions.loadAllReminders),
      switchMap(() =>
        this.service.getUpcoming(30).pipe(
          map((reminders) => RemindersActions.loadAllRemindersSuccess({ reminders })),
          catchError((err) => of(RemindersActions.loadAllRemindersFailure({ error: err.message })))
        )
      )
    )
  );

  createReminder$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RemindersActions.createReminder),
      switchMap(({ request }) =>
        this.service.create(request).pipe(
          map((id) => RemindersActions.createReminderSuccess({ id })),
          catchError((err) => of(RemindersActions.createReminderFailure({ error: err.message })))
        )
      )
    )
  );

  reloadAfterCreate$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RemindersActions.createReminderSuccess),
      withLatestFrom(this.store.select(selectCurrentContactId)),
      map(([, contactId]) =>
        contactId
          ? RemindersActions.loadReminders({ contactId })
          : RemindersActions.loadAllReminders()
      )
    )
  );

  updateReminder$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RemindersActions.updateReminder),
      switchMap(({ id, request }) =>
        this.service.update(id, request).pipe(
          map(() => RemindersActions.updateReminderSuccess()),
          catchError((err) => of(RemindersActions.updateReminderFailure({ error: err.message })))
        )
      )
    )
  );

  cancelReminder$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RemindersActions.cancelReminder),
      switchMap(({ id }) =>
        this.service.cancel(id).pipe(
          map(() => RemindersActions.cancelReminderSuccess({ id })),
          catchError((err) => of(RemindersActions.cancelReminderFailure({ error: err.message })))
        )
      )
    )
  );

  dismissReminder$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RemindersActions.dismissReminder),
      switchMap(({ id }) =>
        this.service.dismiss(id).pipe(
          map(() => RemindersActions.dismissReminderSuccess({ id })),
          catchError((err) => of(RemindersActions.dismissReminderFailure({ error: err.message })))
        )
      )
    )
  );
}
