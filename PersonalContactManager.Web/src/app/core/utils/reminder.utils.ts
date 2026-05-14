import { Reminder, ReminderChannel } from '../models/reminder.model';

export const REMINDER_CHANNEL_OPTIONS: { label: string; value: ReminderChannel }[] = [
  { label: 'In-app', value: 'SignalR' },
  { label: 'Email', value: 'Email' },
  { label: 'Both', value: 'Both' },
];

export function reminderStatusSeverity(status: Reminder['status']): 'warn' | 'success' | 'secondary' | 'danger' {
  const map: Record<Reminder['status'], 'warn' | 'success' | 'secondary' | 'danger'> = {
    Pending: 'warn',
    Fired: 'success',
    Dismissed: 'secondary',
    Cancelled: 'danger',
  };
  return map[status];
}
