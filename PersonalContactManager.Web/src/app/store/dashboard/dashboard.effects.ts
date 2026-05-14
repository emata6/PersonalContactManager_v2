import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap } from 'rxjs';
import { DashboardActions } from './dashboard.actions';
import { DashboardService } from '../../core/services/dashboard.service';
import { ContactsService } from '../../core/services/contacts.service';

@Injectable()
export class DashboardEffects {
  private actions$ = inject(Actions);
  private dashboardService = inject(DashboardService);
  private contactsService = inject(ContactsService);

  loadStats$ = createEffect(() =>
    this.actions$.pipe(
      ofType(DashboardActions.loadStats),
      switchMap(() =>
        this.dashboardService.getStats().pipe(
          map((stats) => DashboardActions.loadStatsSuccess({ stats })),
          catchError((err) => of(DashboardActions.loadStatsFailure({ error: err.message })))
        )
      )
    )
  );

  loadRecentContacts$ = createEffect(() =>
    this.actions$.pipe(
      ofType(DashboardActions.loadRecentContacts),
      switchMap(() =>
        this.contactsService.getAll({ sortBy: 'createdAt', sortDirection: 'desc', page: 1, pageSize: 5 }).pipe(
          map((result) => DashboardActions.loadRecentContactsSuccess({ contacts: result.items })),
          catchError((err) => of(DashboardActions.loadRecentContactsFailure({ error: err.message })))
        )
      )
    )
  );

  loadUpcomingBirthdays$ = createEffect(() =>
    this.actions$.pipe(
      ofType(DashboardActions.loadUpcomingBirthdays),
      switchMap(() =>
        this.contactsService.getUpcomingBirthdays(14).pipe(
          map((contacts) => DashboardActions.loadUpcomingBirthdaysSuccess({ contacts })),
          catchError((err) => of(DashboardActions.loadUpcomingBirthdaysFailure({ error: err.message })))
        )
      )
    )
  );
}
