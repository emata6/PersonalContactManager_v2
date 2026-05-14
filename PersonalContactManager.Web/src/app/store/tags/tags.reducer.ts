import { createFeature, createReducer, on } from '@ngrx/store';
import { TagsActions } from './tags.actions';
import { Tag } from '../../core/models/tag.model';

export interface TagsState {
  tags: Tag[];
  loading: boolean;
  saving: boolean;
  error: string | null;
}

const initialState: TagsState = {
  tags: [],
  loading: false,
  saving: false,
  error: null,
};

export const tagsFeature = createFeature({
  name: 'tags',
  reducer: createReducer(
    initialState,

    on(TagsActions.loadTags, (state) => ({ ...state, loading: true, error: null })),
    on(TagsActions.loadTagsSuccess, (state, { tags }) => ({ ...state, loading: false, tags })),
    on(TagsActions.loadTagsFailure, (state, { error }) => ({ ...state, loading: false, error })),

    on(TagsActions.createTag, (state) => ({ ...state, saving: true })),
    on(TagsActions.createTagSuccess, (state) => ({ ...state, saving: false })),
    on(TagsActions.createTagFailure, (state, { error }) => ({ ...state, saving: false, error })),

    on(TagsActions.updateTag, (state) => ({ ...state, saving: true })),
    on(TagsActions.updateTagSuccess, (state) => ({ ...state, saving: false })),
    on(TagsActions.updateTagFailure, (state, { error }) => ({ ...state, saving: false, error })),

    on(TagsActions.deleteTagSuccess, (state, { id }) => ({
      ...state,
      tags: state.tags.filter((t) => t.id !== id),
    })),
  ),
});

export const {
  name: tagsFeatureName,
  reducer: tagsReducer,
  selectTagsState,
  selectTags,
  selectLoading: selectTagsLoading,
  selectSaving: selectTagsSaving,
  selectError: selectTagsError,
} = tagsFeature;
