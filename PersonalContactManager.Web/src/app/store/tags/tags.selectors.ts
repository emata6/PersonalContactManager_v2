import { createSelector } from '@ngrx/store';
import { selectTagsState } from './tags.reducer';

export const selectAllTags = createSelector(selectTagsState, (s) => s.tags);
export const selectTagsLoading = createSelector(selectTagsState, (s) => s.loading);
export const selectTagsSaving = createSelector(selectTagsState, (s) => s.saving);
export const selectTagsError = createSelector(selectTagsState, (s) => s.error);
