import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Store } from '@ngrx/store';
import { BadgeModule } from 'primeng/badge';
import { RippleModule } from 'primeng/ripple';
import { selectPendingReminders } from '../store/reminders/reminders.selectors';

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive, BadgeModule, RippleModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  private store = inject(Store);
  private pendingReminders = this.store.selectSignal(selectPendingReminders);
  get pendingCount() { return this.pendingReminders().length; }
}
