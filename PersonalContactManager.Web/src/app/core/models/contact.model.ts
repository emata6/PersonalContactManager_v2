import { Tag } from './tag.model';
import { Group } from './group.model';

export interface PhoneNumber {
  number: string;
  label: string;
}

export interface Address {
  street: string | null;
  city: string | null;
  state: string | null;
  postalCode: string | null;
  country: string | null;
}

export interface ContactSummary {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string | null;
  birthday: string | null;
  isFavorite: boolean;
  tags: Tag[];
}

export interface ContactDetail extends ContactSummary {
  iban: string | null;
  notes: string | null;
  address: Address | null;
  phoneNumbers: PhoneNumber[];
  groups: Group[];
  createdAt: string;
  updatedAt: string;
}

export interface ContactSearchParams {
  searchTerm?: string;
  tagId?: string;
  groupId?: string;
  favoritesOnly?: boolean;
  hasBirthday?: boolean;
  sortBy?: 'name' | 'createdAt';
  sortDirection?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

export interface AddressInput {
  street?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
}

export interface CreateContactRequest {
  firstName: string;
  lastName: string;
  email?: string;
  birthday?: string;
  notes?: string;
  iban?: string;
  phone: { number: string; label: string };
  address?: AddressInput;
}

export interface UpdateContactRequest {
  firstName: string;
  lastName: string;
  email?: string;
  birthday?: string;
  notes?: string;
  iban?: string;
  address?: AddressInput;
}
