import { createSelector } from '@ngrx/store';
import { selectDashboardState } from './dashboard.reducer';

export const selectDashboardStats = createSelector(selectDashboardState, (s) => s.stats);
export const selectDashboardRecentContacts = createSelector(selectDashboardState, (s) => s.recentContacts);
export const selectDashboardUpcomingBirthdays = createSelector(selectDashboardState, (s) => s.upcomingBirthdays);
export const selectDashboardLoading = createSelector(selectDashboardState, (s) => s.loading);
