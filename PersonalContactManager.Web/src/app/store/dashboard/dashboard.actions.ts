import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { ContactStats } from '../../core/models/dashboard.model';
import { ContactSummary } from '../../core/models/contact.model';

export const DashboardActions = createActionGroup({
  source: 'Dashboard',
  events: {
    'Load Stats': emptyProps(),
    'Load Stats Success': props<{ stats: ContactStats }>(),
    'Load Stats Failure': props<{ error: string }>(),

    'Load Recent Contacts': emptyProps(),
    'Load Recent Contacts Success': props<{ contacts: ContactSummary[] }>(),
    'Load Recent Contacts Failure': props<{ error: string }>(),

    'Load Upcoming Birthdays': emptyProps(),
    'Load Upcoming Birthdays Success': props<{ contacts: ContactSummary[] }>(),
    'Load Upcoming Birthdays Failure': props<{ error: string }>(),
  },
});
