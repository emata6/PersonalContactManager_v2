import { createFeature, createReducer, on } from '@ngrx/store';
import { GroupsActions } from './groups.actions';
import { Group } from '../../core/models/group.model';

export interface GroupsState {
  groups: Group[];
  loading: boolean;
  saving: boolean;
  error: string | null;
}

const initialState: GroupsState = {
  groups: [],
  loading: false,
  saving: false,
  error: null,
};

export const groupsFeature = createFeature({
  name: 'groups',
  reducer: createReducer(
    initialState,

    on(GroupsActions.loadGroups, (state) => ({ ...state, loading: true, error: null })),
    on(GroupsActions.loadGroupsSuccess, (state, { groups }) => ({ ...state, loading: false, groups })),
    on(GroupsActions.loadGroupsFailure, (state, { error }) => ({ ...state, loading: false, error })),

    on(GroupsActions.createGroup, (state) => ({ ...state, saving: true })),
    on(GroupsActions.createGroupSuccess, (state) => ({ ...state, saving: false })),
    on(GroupsActions.createGroupFailure, (state, { error }) => ({ ...state, saving: false, error })),

    on(GroupsActions.updateGroup, (state) => ({ ...state, saving: true })),
    on(GroupsActions.updateGroupSuccess, (state) => ({ ...state, saving: false })),
    on(GroupsActions.updateGroupFailure, (state, { error }) => ({ ...state, saving: false, error })),

    on(GroupsActions.deleteGroupSuccess, (state, { id }) => ({
      ...state,
      groups: state.groups.filter((g) => g.id !== id),
    })),
  ),
});

export const {
  name: groupsFeatureName,
  reducer: groupsReducer,
  selectGroupsState,
  selectGroups,
  selectLoading: selectGroupsLoading,
  selectSaving: selectGroupsSaving,
  selectError: selectGroupsError,
} = groupsFeature;
