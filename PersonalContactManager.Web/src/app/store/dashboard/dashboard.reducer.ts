import { createFeature, createReducer, on } from '@ngrx/store';
import { ContactStats } from '../../core/models/dashboard.model';
import { ContactSummary } from '../../core/models/contact.model';
import { DashboardActions } from './dashboard.actions';

export interface DashboardState {
  stats: ContactStats | null;
  recentContacts: ContactSummary[];
  upcomingBirthdays: ContactSummary[];
  loading: boolean;
  error: string | null;
}

const initialState: DashboardState = {
  stats: null,
  recentContacts: [],
  upcomingBirthdays: [],
  loading: false,
  error: null,
};

export const dashboardFeature = createFeature({
  name: 'dashboard',
  reducer: createReducer(
    initialState,

    on(DashboardActions.loadStats, DashboardActions.loadRecentContacts, DashboardActions.loadUpcomingBirthdays,
      (state) => ({ ...state, loading: true, error: null })),

    on(DashboardActions.loadStatsSuccess, (state, { stats }) => ({ ...state, stats, loading: false })),
    on(DashboardActions.loadStatsFailure, (state, { error }) => ({ ...state, error, loading: false })),

    on(DashboardActions.loadRecentContactsSuccess, (state, { contacts }) =>
      ({ ...state, recentContacts: contacts, loading: false })),
    on(DashboardActions.loadRecentContactsFailure, (state, { error }) => ({ ...state, error, loading: false })),

    on(DashboardActions.loadUpcomingBirthdaysSuccess, (state, { contacts }) =>
      ({ ...state, upcomingBirthdays: contacts, loading: false })),
    on(DashboardActions.loadUpcomingBirthdaysFailure, (state, { error }) => ({ ...state, error, loading: false })),
  ),
});

export const { selectDashboardState } = dashboardFeature;
