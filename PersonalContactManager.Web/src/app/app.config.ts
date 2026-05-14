import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { provideRouterStore } from '@ngrx/router-store';
import { providePrimeNG } from 'primeng/config';
import { MessageService } from 'primeng/api';
import Aura from '@primeuix/themes/aura';
import { routes } from './app.routes';
import { contactsFeature } from './store/contacts/contacts.reducer';
import { tagsFeature } from './store/tags/tags.reducer';
import { groupsFeature } from './store/groups/groups.reducer';
import { remindersFeature } from './store/reminders/reminders.reducer';
import { dashboardFeature } from './store/dashboard/dashboard.reducer';
import { ContactsEffects } from './store/contacts/contacts.effects';
import { TagsEffects } from './store/tags/tags.effects';
import { GroupsEffects } from './store/groups/groups.effects';
import { RemindersEffects } from './store/reminders/reminders.effects';
import { DashboardEffects } from './store/dashboard/dashboard.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch(), withInterceptors([errorInterceptor])),
    provideAnimationsAsync(),

    // NgRx
    provideStore({
      [contactsFeature.name]: contactsFeature.reducer,
      [tagsFeature.name]: tagsFeature.reducer,
      [groupsFeature.name]: groupsFeature.reducer,
      [remindersFeature.name]: remindersFeature.reducer,
      [dashboardFeature.name]: dashboardFeature.reducer,
    }),
    provideEffects([ContactsEffects, TagsEffects, GroupsEffects, RemindersEffects, DashboardEffects]),
    provideRouterStore(),
    provideStoreDevtools({ maxAge: 25, logOnly: false }),

    // PrimeNG
    MessageService,
    providePrimeNG({
      theme: {
        preset: Aura,
        options: { darkModeSelector: 'body', ripple: true },
      },
    }),
  ],
};
