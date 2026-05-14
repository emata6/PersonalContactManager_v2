import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Reminder, CreateReminderRequest, UpdateReminderRequest } from '../../core/models/reminder.model';

export const RemindersActions = createActionGroup({
  source: 'Reminders',
  events: {
    'Load Reminders': props<{ contactId: string }>(),
    'Load Reminders Success': props<{ reminders: Reminder[] }>(),
    'Load Reminders Failure': props<{ error: string }>(),

    'Load All Reminders': emptyProps(),
    'Load All Reminders Success': props<{ reminders: Reminder[] }>(),
    'Load All Reminders Failure': props<{ error: string }>(),

    'Create Reminder': props<{ request: CreateReminderRequest }>(),
    'Create Reminder Success': props<{ id: string }>(),
    'Create Reminder Failure': props<{ error: string }>(),

    'Update Reminder': props<{ id: string; request: UpdateReminderRequest }>(),
    'Update Reminder Success': emptyProps(),
    'Update Reminder Failure': props<{ error: string }>(),

    'Cancel Reminder': props<{ id: string }>(),
    'Cancel Reminder Success': props<{ id: string }>(),
    'Cancel Reminder Failure': props<{ error: string }>(),

    'Dismiss Reminder': props<{ id: string }>(),
    'Dismiss Reminder Success': props<{ id: string }>(),
    'Dismiss Reminder Failure': props<{ error: string }>(),
  },
});
