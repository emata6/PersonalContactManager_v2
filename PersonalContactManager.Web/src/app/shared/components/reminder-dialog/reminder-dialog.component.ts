import { Component, effect, input, model, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { ReminderChannel } from '../../../core/models/reminder.model';
import { REMINDER_CHANNEL_OPTIONS } from '../../../core/utils/reminder.utils';
import { FormLabelComponent } from '../form-label/form-label.component';

export interface ReminderFormValue {
  contactId?: string;
  title: string;
  note?: string;
  dueAt: Date;
  channel: ReminderChannel;
}

@Component({
  selector: 'app-reminder-dialog',
  imports: [
    FormsModule,
    DialogModule, ButtonModule,
    InputTextModule, TextareaModule, DatePickerModule, SelectModule,
    FormLabelComponent,
  ],
  templateUrl: './reminder-dialog.component.html',
})
export class ReminderDialogComponent {
  visible  = model.required<boolean>();
  saving   = input<boolean>(false);
  /** When provided, a contact selector is shown at the top of the form. */
  contacts = input<{ id: string; fullName: string }[]>();

  saved = output<ReminderFormValue>();

  // Form state — plain properties work cleanly with ngModel
  contactId = '';
  title     = '';
  note      = '';
  dueAt: Date | null = null;
  channel: ReminderChannel = 'SignalR';

  readonly minDate       = new Date();
  readonly channelOptions = REMINDER_CHANNEL_OPTIONS;

  get isSubmitDisabled(): boolean {
    return !this.title.trim()
      || !this.dueAt
      || (!!this.contacts() && !this.contactId);
  }

  constructor() {
    // Reset the form every time the dialog is opened
    effect(() => { if (this.visible()) this.reset(); });
  }

  submit(): void {
    if (this.isSubmitDisabled) return;
    this.saved.emit({
      contactId: this.contacts() ? this.contactId : undefined,
      title:     this.title.trim(),
      note:      this.note.trim() || undefined,
      dueAt:     this.dueAt!,
      channel:   this.channel,
    });
    this.visible.set(false);
  }

  cancel(): void {
    this.visible.set(false);
  }

  private reset(): void {
    this.contactId = '';
    this.title     = '';
    this.note      = '';
    this.dueAt     = null;
    this.channel   = 'SignalR';
  }
}
