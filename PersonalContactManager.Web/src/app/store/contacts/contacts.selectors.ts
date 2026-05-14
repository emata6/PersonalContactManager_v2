import { createSelector } from '@ngrx/store';
import { selectContactsState } from './contacts.reducer';

export const selectContactsList = createSelector(selectContactsState, (s) => s.list);
export const selectContactItems = createSelector(selectContactsList, (list) => list?.items ?? []);
export const selectContactsLoading = createSelector(selectContactsState, (s) => s.loading);
export const selectContactsSaving = createSelector(selectContactsState, (s) => s.saving);
export const selectContactsError = createSelector(selectContactsState, (s) => s.error);
export const selectSelectedContact = createSelector(selectContactsState, (s) => s.selected);
export const selectContactsSearchParams = createSelector(selectContactsState, (s) => s.searchParams);
export const selectContactsTotalCount = createSelector(selectContactsList, (list) => list?.totalCount ?? 0);
export const selectContactsTotalPages = createSelector(selectContactsList, (list) => list?.totalPages ?? 0);
