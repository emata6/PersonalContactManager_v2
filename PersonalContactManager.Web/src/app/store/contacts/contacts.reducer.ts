import { createFeature, createReducer, on } from '@ngrx/store';
import { ContactsActions } from './contacts.actions';
import { ContactSummary, ContactDetail, ContactSearchParams } from '../../core/models/contact.model';
import { PagedResult } from '../../core/models/paged-result.model';

export interface ContactsState {
  list: PagedResult<ContactSummary> | null;
  selected: ContactDetail | null;
  searchParams: ContactSearchParams;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

const initialState: ContactsState = {
  list: null,
  selected: null,
  searchParams: { page: 1, pageSize: 20 },
  loading: false,
  saving: false,
  error: null,
};

export const contactsFeature = createFeature({
  name: 'contacts',
  reducer: createReducer(
    initialState,

    on(ContactsActions.loadContacts, (state, { params }) => ({
      ...state,
      loading: true,
      error: null,
      searchParams: params,
    })),
    on(ContactsActions.loadContactsSuccess, (state, { result }) => ({
      ...state,
      loading: false,
      list: result,
    })),
    on(ContactsActions.loadContactsFailure, (state, { error }) => ({
      ...state,
      loading: false,
      error,
    })),

    on(ContactsActions.loadContact, (state) => ({ ...state, loading: true, selected: null, error: null })),
    on(ContactsActions.loadContactSuccess, (state, { contact }) => ({
      ...state,
      loading: false,
      selected: contact,
    })),
    on(ContactsActions.loadContactFailure, (state, { error }) => ({ ...state, loading: false, error })),

    on(ContactsActions.createContact, (state) => ({ ...state, saving: true, error: null })),
    on(ContactsActions.createContactSuccess, (state) => ({ ...state, saving: false })),
    on(ContactsActions.createContactFailure, (state, { error }) => ({ ...state, saving: false, error })),

    on(ContactsActions.updateContact, (state) => ({ ...state, saving: true, error: null })),
    on(ContactsActions.updateContactSuccess, (state) => ({ ...state, saving: false })),
    on(ContactsActions.updateContactFailure, (state, { error }) => ({ ...state, saving: false, error })),

    on(ContactsActions.deleteContactSuccess, (state, { id }) => ({
      ...state,
      list: state.list
        ? { ...state.list, items: state.list.items.filter((c) => c.id !== id) }
        : null,
    })),

    on(ContactsActions.toggleFavoriteSuccess, (state, { id }) => ({
      ...state,
      list: state.list
        ? {
            ...state.list,
            items: state.list.items.map((c) =>
              c.id === id ? { ...c, isFavorite: !c.isFavorite } : c
            ),
          }
        : null,
    })),

    on(ContactsActions.setSearchParams, (state, { params }) => ({
      ...state,
      searchParams: params,
    })),

    on(ContactsActions.signalRContactDeleted, (state, { id }) => ({
      ...state,
      list: state.list
        ? { ...state.list, items: state.list.items.filter((c) => c.id !== id) }
        : null,
    })),
  ),
});

export const {
  name: contactsFeatureName,
  reducer: contactsReducer,
  selectContactsState,
  selectList,
  selectSelected,
  selectSearchParams,
  selectLoading,
  selectSaving,
  selectError,
} = contactsFeature;
