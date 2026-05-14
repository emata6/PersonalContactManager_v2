import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap } from 'rxjs';
import { GroupsActions } from './groups.actions';
import { GroupsService } from '../../core/services/groups.service';

@Injectable()
export class GroupsEffects {
  private actions$ = inject(Actions);
  private service = inject(GroupsService);

  loadGroups$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GroupsActions.loadGroups),
      switchMap(() =>
        this.service.getAll().pipe(
          map((groups) => GroupsActions.loadGroupsSuccess({ groups })),
          catchError((err) => of(GroupsActions.loadGroupsFailure({ error: err.message })))
        )
      )
    )
  );

  createGroup$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GroupsActions.createGroup),
      switchMap(({ request }) =>
        this.service.create(request).pipe(
          map((id) => GroupsActions.createGroupSuccess({ id })),
          catchError((err) => of(GroupsActions.createGroupFailure({ error: err.message })))
        )
      )
    )
  );

  reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GroupsActions.createGroupSuccess, GroupsActions.updateGroupSuccess, GroupsActions.deleteGroupSuccess),
      map(() => GroupsActions.loadGroups())
    )
  );

  updateGroup$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GroupsActions.updateGroup),
      switchMap(({ id, request }) =>
        this.service.update(id, request).pipe(
          map(() => GroupsActions.updateGroupSuccess()),
          catchError((err) => of(GroupsActions.updateGroupFailure({ error: err.message })))
        )
      )
    )
  );

  deleteGroup$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GroupsActions.deleteGroup),
      switchMap(({ id }) =>
        this.service.delete(id).pipe(
          map(() => GroupsActions.deleteGroupSuccess({ id })),
          catchError((err) => of(GroupsActions.deleteGroupFailure({ error: err.message })))
        )
      )
    )
  );
}
