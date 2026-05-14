import { createSelector } from '@ngrx/store';
import { selectRemindersState } from './reminders.reducer';

export const selectAllReminders = createSelector(selectRemindersState, (s) => s.reminders);
export const selectPendingReminders = createSelector(selectAllReminders, (rs) =>
  rs.filter((r) => r.status === 'Pending')
);
export const selectRemindersLoading = createSelector(selectRemindersState, (s) => s.loading);
export const selectRemindersSaving = createSelector(selectRemindersState, (s) => s.saving);
export const selectRemindersError = createSelector(selectRemindersState, (s) => s.error);
