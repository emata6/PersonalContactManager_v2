import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { CardModule } from 'primeng/card';
import { SkeletonModule } from 'primeng/skeleton';
import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';
import { DashboardActions } from '../../store/dashboard/dashboard.actions';
import {
  selectDashboardStats,
  selectDashboardRecentContacts,
  selectDashboardUpcomingBirthdays,
  selectDashboardLoading,
} from '../../store/dashboard/dashboard.selectors';

@Component({
  selector: 'app-dashboard',
  imports: [CardModule, SkeletonModule, AvatarModule, ButtonModule],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit {
  private store = inject(Store);
  private router = inject(Router);

  stats = this.store.selectSignal(selectDashboardStats);
  recentContacts = this.store.selectSignal(selectDashboardRecentContacts);
  upcomingBirthdays = this.store.selectSignal(selectDashboardUpcomingBirthdays);
  loading = this.store.selectSignal(selectDashboardLoading);

  ngOnInit(): void {
    this.store.dispatch(DashboardActions.loadStats());
    this.store.dispatch(DashboardActions.loadRecentContacts());
    this.store.dispatch(DashboardActions.loadUpcomingBirthdays());
  }

  open(id: string): void {
    this.router.navigate(['/contacts', id]);
  }

  goToContacts(): void { this.router.navigate(['/contacts']); }
  goToFavorites(): void { this.router.navigate(['/contacts'], { queryParams: { favoritesOnly: true } }); }
  goToBirthdays(): void { this.router.navigate(['/contacts'], { queryParams: { hasBirthday: true } }); }
  goToReminders(): void { this.router.navigate(['/reminders']); }

  formatBirthday(birthday: string | null): string {
    if (!birthday) return '';
    const d = new Date(birthday + 'T00:00:00');
    return d.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
  }
}
