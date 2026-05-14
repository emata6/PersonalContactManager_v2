import { Component, OnInit, inject, computed, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { AvatarModule } from 'primeng/avatar';
import { DividerModule } from 'primeng/divider';
import { SkeletonModule } from 'primeng/skeleton';
import { ChipModule } from 'primeng/chip';
import { SelectModule } from 'primeng/select';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ContactsActions } from '../../../store/contacts/contacts.actions';
import { RemindersActions } from '../../../store/reminders/reminders.actions';
import { selectSelectedContact, selectContactsLoading } from '../../../store/contacts/contacts.selectors';
import { selectAllReminders, selectRemindersSaving } from '../../../store/reminders/reminders.selectors';
import { selectAllTags } from '../../../store/tags/tags.selectors';
import { selectAllGroups } from '../../../store/groups/groups.selectors';
import { reminderStatusSeverity } from '../../../core/utils/reminder.utils';
import { ReminderDialogComponent, ReminderFormValue } from '../../../shared/components/reminder-dialog/reminder-dialog.component';

@Component({
  selector: 'app-contact-detail',
  imports: [
    DatePipe, FormsModule,
    ButtonModule, CardModule, TagModule, AvatarModule, DividerModule,
    SkeletonModule, ChipModule, SelectModule, ConfirmDialogModule, TooltipModule,
    ReminderDialogComponent,
  ],
  providers: [ConfirmationService],
  templateUrl: './contact-detail.component.html',
})
export class ContactDetailComponent implements OnInit {
  private store   = inject(Store);
  private route   = inject(ActivatedRoute);
  private router  = inject(Router);
  private confirm = inject(ConfirmationService);
  private message = inject(MessageService);

  contact        = this.store.selectSignal(selectSelectedContact);
  loading        = this.store.selectSignal(selectContactsLoading);
  reminders      = this.store.selectSignal(selectAllReminders);
  remindersSaving = this.store.selectSignal(selectRemindersSaving);
  allTags        = this.store.selectSignal(selectAllTags);
  allGroups      = this.store.selectSignal(selectAllGroups);

  availableTags = computed(() => {
    const assigned = new Set(this.contact()?.tags.map(t => t.id) ?? []);
    return this.allTags().filter(t => !assigned.has(t.id));
  });

  availableGroups = computed(() => {
    const assigned = new Set(this.contact()?.groups.map(g => g.id) ?? []);
    return this.allGroups().filter(g => !assigned.has(g.id));
  });

  pendingTagId:   string | null = null;
  pendingGroupId: string | null = null;

  reminderDialogVisible = signal(false);

  readonly statusSeverity = reminderStatusSeverity;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.store.dispatch(ContactsActions.loadContact({ id }));
    this.store.dispatch(RemindersActions.loadReminders({ contactId: id }));
  }

  edit(): void { this.router.navigate(['/contacts', this.contact()!.id, 'edit']); }

  toggleFavorite(): void {
    this.store.dispatch(ContactsActions.toggleFavorite({ id: this.contact()!.id }));
  }

  confirmDelete(): void {
    this.confirm.confirm({
      message: 'Delete this contact permanently?',
      accept: () => {
        this.store.dispatch(ContactsActions.deleteContact({ id: this.contact()!.id }));
        this.router.navigate(['/contacts']);
      },
    });
  }

  onTagSelect(tagId: string | null): void {
    if (!tagId) return;
    this.store.dispatch(ContactsActions.assignTag({ contactId: this.contact()!.id, tagId }));
    this.pendingTagId = null;
  }

  removeTag(tagId: string): void {
    this.store.dispatch(ContactsActions.removeTag({ contactId: this.contact()!.id, tagId }));
  }

  onGroupSelect(groupId: string | null): void {
    if (!groupId) return;
    this.store.dispatch(ContactsActions.assignGroup({ contactId: this.contact()!.id, groupId }));
    this.pendingGroupId = null;
  }

  removeGroup(groupId: string): void {
    this.store.dispatch(ContactsActions.removeGroup({ contactId: this.contact()!.id, groupId }));
  }

  onReminderSaved(value: ReminderFormValue): void {
    const c = this.contact();
    if (!c) return;
    this.store.dispatch(RemindersActions.createReminder({
      request: {
        contactId: c.id,
        title:     value.title,
        note:      value.note,
        dueAt:     value.dueAt.toISOString(),
        channel:   value.channel,
      },
    }));
  }

  dismissReminder(id: string): void {
    this.store.dispatch(RemindersActions.dismissReminder({ id }));
  }

  copyIban(iban: string): void {
    navigator.clipboard.writeText(iban).then(() =>
      this.message.add({ severity: 'success', summary: 'Copied', detail: 'IBAN copied to clipboard', life: 2000 })
    );
  }
}
