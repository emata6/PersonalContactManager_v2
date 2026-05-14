import { createSelector } from '@ngrx/store';
import { selectGroupsState } from './groups.reducer';

export const selectAllGroups = createSelector(selectGroupsState, (s) => s.groups);
export const selectGroupsLoading = createSelector(selectGroupsState, (s) => s.loading);
export const selectGroupsSaving = createSelector(selectGroupsState, (s) => s.saving);
export const selectGroupsError = createSelector(selectGroupsState, (s) => s.error);
