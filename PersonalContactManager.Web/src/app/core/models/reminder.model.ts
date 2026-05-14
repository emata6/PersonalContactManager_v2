export type ReminderChannel = 'Email' | 'SignalR' | 'Both';
export type ReminderStatus = 'Pending' | 'Fired' | 'Dismissed' | 'Cancelled';

export interface Reminder {
  id: string;
  contactId: string;
  contactFullName: string;
  title: string;
  note: string | null;
  dueAt: string;
  recurrenceRule: string | null;
  channel: ReminderChannel;
  status: ReminderStatus;
  firedAt: string | null;
  createdAt: string;
}

export interface CreateReminderRequest {
  contactId: string;
  title: string;
  note?: string;
  dueAt: string;
  recurrenceRule?: string;
  channel: ReminderChannel;
}

export interface UpdateReminderRequest {
  title: string;
  note?: string;
  dueAt: string;
  recurrenceRule?: string;
  channel: ReminderChannel;
}
