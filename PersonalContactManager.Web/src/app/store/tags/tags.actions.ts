import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Tag, CreateTagRequest, UpdateTagRequest } from '../../core/models/tag.model';

export const TagsActions = createActionGroup({
  source: 'Tags',
  events: {
    'Load Tags': emptyProps(),
    'Load Tags Success': props<{ tags: Tag[] }>(),
    'Load Tags Failure': props<{ error: string }>(),

    'Create Tag': props<{ request: CreateTagRequest }>(),
    'Create Tag Success': props<{ id: string }>(),
    'Create Tag Failure': props<{ error: string }>(),

    'Update Tag': props<{ id: string; request: UpdateTagRequest }>(),
    'Update Tag Success': emptyProps(),
    'Update Tag Failure': props<{ error: string }>(),

    'Delete Tag': props<{ id: string }>(),
    'Delete Tag Success': props<{ id: string }>(),
    'Delete Tag Failure': props<{ error: string }>(),
  },
});
