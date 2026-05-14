import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Group, CreateGroupRequest, UpdateGroupRequest } from '../../core/models/group.model';

export const GroupsActions = createActionGroup({
  source: 'Groups',
  events: {
    'Load Groups': emptyProps(),
    'Load Groups Success': props<{ groups: Group[] }>(),
    'Load Groups Failure': props<{ error: string }>(),

    'Create Group': props<{ request: CreateGroupRequest }>(),
    'Create Group Success': props<{ id: string }>(),
    'Create Group Failure': props<{ error: string }>(),

    'Update Group': props<{ id: string; request: UpdateGroupRequest }>(),
    'Update Group Success': emptyProps(),
    'Update Group Failure': props<{ error: string }>(),

    'Delete Group': props<{ id: string }>(),
    'Delete Group Success': props<{ id: string }>(),
    'Delete Group Failure': props<{ error: string }>(),
  },
});
