import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap } from 'rxjs';
import { TagsActions } from './tags.actions';
import { TagsService } from '../../core/services/tags.service';

@Injectable()
export class TagsEffects {
  private actions$ = inject(Actions);
  private service = inject(TagsService);

  loadTags$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TagsActions.loadTags),
      switchMap(() =>
        this.service.getAll().pipe(
          map((tags) => TagsActions.loadTagsSuccess({ tags })),
          catchError((err) => of(TagsActions.loadTagsFailure({ error: err.message })))
        )
      )
    )
  );

  createTag$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TagsActions.createTag),
      switchMap(({ request }) =>
        this.service.create(request).pipe(
          map((id) => TagsActions.createTagSuccess({ id })),
          catchError((err) => of(TagsActions.createTagFailure({ error: err.message })))
        )
      )
    )
  );

  reloadAfterCreate$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TagsActions.createTagSuccess, TagsActions.updateTagSuccess, TagsActions.deleteTagSuccess),
      map(() => TagsActions.loadTags())
    )
  );

  updateTag$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TagsActions.updateTag),
      switchMap(({ id, request }) =>
        this.service.update(id, request).pipe(
          map(() => TagsActions.updateTagSuccess()),
          catchError((err) => of(TagsActions.updateTagFailure({ error: err.message })))
        )
      )
    )
  );

  deleteTag$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TagsActions.deleteTag),
      switchMap(({ id }) =>
        this.service.delete(id).pipe(
          map(() => TagsActions.deleteTagSuccess({ id })),
          catchError((err) => of(TagsActions.deleteTagFailure({ error: err.message })))
        )
      )
    )
  );
}
