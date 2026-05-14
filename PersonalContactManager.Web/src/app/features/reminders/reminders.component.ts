import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService } from 'primeng/api';
import { RemindersActions } from '../../store/reminders/reminders.actions';
import { selectAllReminders, selectRemindersLoading, selectRemindersSaving } from '../../store/reminders/reminders.selectors';
import { Reminder } from '../../core/models/reminder.model';
import { selectContactItems } from '../../store/contacts/contacts.selectors';
import { ContactsActions } from '../../store/contacts/contacts.actions';
import { reminderStatusSeverity } from '../../core/utils/reminder.utils';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { ReminderDialogComponent, ReminderFormValue } from '../../shared/components/reminder-dialog/reminder-dialog.component';

@Component({
  selector: 'app-reminders',
  imports: [
    DatePipe,
    TableModule, ButtonModule, TagModule, ConfirmDialogModule, TooltipModule,
    PageHeaderComponent, ReminderDialogComponent,
  ],
  providers: [ConfirmationService],
  templateUrl: './reminders.component.html',
})
export class RemindersComponent implements OnInit {
  private store = inject(Store);
  private confirm = inject(ConfirmationService);

  reminders = this.store.selectSignal(selectAllReminders);
  loading   = this.store.selectSignal(selectRemindersLoading);
  saving    = this.store.selectSignal(selectRemindersSaving);
  contacts  = this.store.selectSignal(selectContactItems);

  dialogVisible = signal(false);

  readonly statusSeverity = reminderStatusSeverity;

  ngOnInit(): void {
    this.store.dispatch(RemindersActions.loadAllReminders());
    this.store.dispatch(ContactsActions.loadContacts({ params: { page: 1, pageSize: 200 } }));
  }

  onReminderSaved(value: ReminderFormValue): void {
    this.store.dispatch(RemindersActions.createReminder({
      request: {
        contactId: value.contactId!,
        title:     value.title,
        note:      value.note,
        dueAt:     value.dueAt.toISOString(),
        channel:   value.channel,
      },
    }));
  }

  dismiss(id: string): void {
    this.store.dispatch(RemindersActions.dismissReminder({ id }));
  }

  confirmCancel(event: Event, id: string): void {
    this.confirm.confirm({
      target:  event.target as EventTarget,
      message: 'Cancel this reminder?',
      accept:  () => this.store.dispatch(RemindersActions.cancelReminder({ id })),
    });
  }
}
