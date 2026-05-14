import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { ContactSummary, ContactDetail, ContactSearchParams, CreateContactRequest, UpdateContactRequest } from '../../core/models/contact.model';
import { PagedResult } from '../../core/models/paged-result.model';

export const ContactsActions = createActionGroup({
  source: 'Contacts',
  events: {
    // Load list
    'Load Contacts': props<{ params: ContactSearchParams }>(),
    'Load Contacts Success': props<{ result: PagedResult<ContactSummary> }>(),
    'Load Contacts Failure': props<{ error: string }>(),

    // Load single
    'Load Contact': props<{ id: string }>(),
    'Load Contact Success': props<{ contact: ContactDetail }>(),
    'Load Contact Failure': props<{ error: string }>(),

    // Create
    'Create Contact': props<{ request: CreateContactRequest }>(),
    'Create Contact Success': props<{ id: string }>(),
    'Create Contact Failure': props<{ error: string }>(),

    // Update
    'Update Contact': props<{ id: string; request: UpdateContactRequest }>(),
    'Update Contact Success': emptyProps(),
    'Update Contact Failure': props<{ error: string }>(),

    // Delete
    'Delete Contact': props<{ id: string }>(),
    'Delete Contact Success': props<{ id: string }>(),
    'Delete Contact Failure': props<{ error: string }>(),

    // Toggle favorite
    'Toggle Favorite': props<{ id: string }>(),
    'Toggle Favorite Success': props<{ id: string }>(),
    'Toggle Favorite Failure': props<{ error: string }>(),

    // Tags
    'Assign Tag': props<{ contactId: string; tagId: string }>(),
    'Assign Tag Success': props<{ contactId: string; tagId: string }>(),
    'Assign Tag Failure': props<{ error: string }>(),

    'Remove Tag': props<{ contactId: string; tagId: string }>(),
    'Remove Tag Success': props<{ contactId: string; tagId: string }>(),
    'Remove Tag Failure': props<{ error: string }>(),

    // Groups
    'Assign Group': props<{ contactId: string; groupId: string }>(),
    'Assign Group Success': props<{ contactId: string; groupId: string }>(),
    'Assign Group Failure': props<{ error: string }>(),

    'Remove Group': props<{ contactId: string; groupId: string }>(),
    'Remove Group Success': props<{ contactId: string; groupId: string }>(),
    'Remove Group Failure': props<{ error: string }>(),

    // Phone numbers
    'Add Phone Number': props<{ contactId: string; number: string; label: string }>(),
    'Add Phone Number Success': props<{ contactId: string }>(),
    'Add Phone Number Failure': props<{ error: string }>(),

    'Remove Phone Number': props<{ contactId: string; number: string; label: string }>(),
    'Remove Phone Number Success': props<{ contactId: string }>(),
    'Remove Phone Number Failure': props<{ error: string }>(),

    // Search filter change
    'Set Search Params': props<{ params: ContactSearchParams }>(),

    // Import / Export
    'Export Csv': emptyProps(),
    'Export Csv Success': emptyProps(),
    'Export Csv Failure': props<{ error: string }>(),

    'Import Csv': props<{ csvContent: string }>(),
    'Import Csv Success': props<{ imported: number }>(),
    'Import Csv Failure': props<{ error: string }>(),

    // SignalR real-time events
    'SignalR Contact Created': props<{ id: string }>(),
    'SignalR Contact Updated': props<{ id: string }>(),
    'SignalR Contact Deleted': props<{ id: string }>(),
    'SignalR Reminder Fired': props<{ payload: { reminderId: string; contactId: string; title: string } }>(),
  },
});
