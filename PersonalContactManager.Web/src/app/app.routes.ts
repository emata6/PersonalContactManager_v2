import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
      },
      {
        path: 'contacts',
        children: [
          {
            path: '',
            loadComponent: () =>
              import('./features/contacts/contacts-list/contacts-list.component').then(
                (m) => m.ContactsListComponent
              ),
          },
          {
            path: 'new',
            loadComponent: () =>
              import('./features/contacts/contact-form/contact-form.component').then(
                (m) => m.ContactFormComponent
              ),
          },
          {
            path: ':id',
            loadComponent: () =>
              import('./features/contacts/contact-detail/contact-detail.component').then(
                (m) => m.ContactDetailComponent
              ),
          },
          {
            path: ':id/edit',
            loadComponent: () =>
              import('./features/contacts/contact-form/contact-form.component').then(
                (m) => m.ContactFormComponent
              ),
          },
        ],
      },
      {
        path: 'tags',
        loadComponent: () =>
          import('./features/tags/tags.component').then((m) => m.TagsComponent),
      },
      {
        path: 'groups',
        loadComponent: () =>
          import('./features/groups/groups.component').then((m) => m.GroupsComponent),
      },
      {
        path: 'reminders',
        loadComponent: () =>
          import('./features/reminders/reminders.component').then((m) => m.RemindersComponent),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
