import { createFeature, createReducer, createSelector, on } from '@ngrx/store';
import { RemindersActions } from './reminders.actions';
import { Reminder } from '../../core/models/reminder.model';

export interface RemindersState {
  reminders: Reminder[];
  currentContactId: string | null;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

const initialState: RemindersState = {
  reminders: [],
  currentContactId: null,
  loading: false,
  saving: false,
  error: null,
};

export const remindersFeature = createFeature({
  name: 'reminders',
  reducer: createReducer(
    initialState,

    on(RemindersActions.loadReminders, (state, { contactId }) =>
      ({ ...state, loading: true, error: null, currentContactId: contactId })),
    on(RemindersActions.loadRemindersSuccess, (state, { reminders }) => ({ ...state, loading: false, reminders })),
    on(RemindersActions.loadRemindersFailure, (state, { error }) => ({ ...state, loading: false, error })),

    on(RemindersActions.loadAllReminders, (state) =>
      ({ ...state, loading: true, error: null, currentContactId: null })),
    on(RemindersActions.loadAllRemindersSuccess, (state, { reminders }) => ({ ...state, loading: false, reminders })),
    on(RemindersActions.loadAllRemindersFailure, (state, { error }) => ({ ...state, loading: false, error })),

    on(RemindersActions.createReminder, (state) => ({ ...state, saving: true })),
    on(RemindersActions.createReminderSuccess, (state) => ({ ...state, saving: false })),
    on(RemindersActions.createReminderFailure, (state, { error }) => ({ ...state, saving: false, error })),

    on(RemindersActions.updateReminder, (state) => ({ ...state, saving: true })),
    on(RemindersActions.updateReminderSuccess, (state) => ({ ...state, saving: false })),
    on(RemindersActions.updateReminderFailure, (state, { error }) => ({ ...state, saving: false, error })),

    on(RemindersActions.cancelReminderSuccess, (state, { id }) => ({
      ...state,
      reminders: state.reminders.filter((r) => r.id !== id),
    })),

    on(RemindersActions.dismissReminderSuccess, (state, { id }) => ({
      ...state,
      reminders: state.reminders.map((r) =>
        r.id === id ? { ...r, status: 'Dismissed' as const } : r
      ),
    })),
  ),
});

export const {
  name: remindersFeatureName,
  reducer: remindersReducer,
  selectRemindersState,
  selectReminders,
  selectLoading: selectRemindersLoading,
  selectSaving: selectRemindersSaving,
  selectError: selectRemindersError,
} = remindersFeature;

export const selectCurrentContactId = createSelector(
  selectRemindersState,
  (s) => s.currentContactId
);
