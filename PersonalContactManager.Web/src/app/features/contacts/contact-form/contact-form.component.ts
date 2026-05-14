import { Component, OnInit, inject, computed, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DatePickerModule } from 'primeng/datepicker';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { ChipModule } from 'primeng/chip';
import { ContactsActions } from '../../../store/contacts/contacts.actions';
import { selectSelectedContact, selectContactsSaving } from '../../../store/contacts/contacts.selectors';
import { PhoneNumber } from '../../../core/models/contact.model';
import { FormLabelComponent } from '../../../shared/components/form-label/form-label.component';

@Component({
  selector: 'app-contact-form',
  imports: [
    ReactiveFormsModule,
    InputTextModule,
    ButtonModule,
    CardModule,
    DatePickerModule,
    TextareaModule,
    SelectModule,
    ChipModule,
    FormLabelComponent,
  ],
  templateUrl: './contact-form.component.html',
})
export class ContactFormComponent implements OnInit {
  private store = inject(Store);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  saving = this.store.selectSignal(selectContactsSaving);
  existing = this.store.selectSignal(selectSelectedContact);
  existingPhones = computed(() => this.existing()?.phoneNumbers ?? []);

  isEdit = false;
  contactId: string | null = null;
  showPhoneError = false;
  private formPatched = false;

  readonly phoneLabelOptions = [
    { label: 'Mobile', value: 'mobile' },
    { label: 'Home', value: 'home' },
    { label: 'Work', value: 'work' },
    { label: 'Other', value: 'other' },
  ];

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', Validators.email],
    birthday: [null as Date | null],
    notes: [''],
    iban: [''],
    phoneNumber: [''],
    phoneLabel: ['mobile'],
    street: [''],
    city: [''],
    state: [''],
    postalCode: [''],
    country: [''],
  });

  constructor() {
    effect(() => {
      const c = this.existing();
      if (this.isEdit && c && !this.formPatched) {
        this.formPatched = true;
        this.form.patchValue({
          firstName: c.firstName,
          lastName: c.lastName,
          email: c.email ?? '',
          birthday: c.birthday ? new Date(c.birthday) : null,
          notes: c.notes ?? '',
          iban: c.iban ?? '',
          street: c.address?.street ?? '',
          city: c.address?.city ?? '',
          state: c.address?.state ?? '',
          postalCode: c.address?.postalCode ?? '',
          country: c.address?.country ?? '',
        });
      }
    });
  }

  ngOnInit(): void {
    this.contactId = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.contactId;
    if (this.isEdit) {
      this.store.dispatch(ContactsActions.loadContact({ id: this.contactId! }));
    }
  }

  addPhone(): void {
    const number = this.form.get('phoneNumber')?.value?.trim();
    const label = this.form.get('phoneLabel')?.value || 'mobile';
    if (!number || !this.contactId) return;
    this.store.dispatch(ContactsActions.addPhoneNumber({ contactId: this.contactId, number, label }));
    this.form.patchValue({ phoneNumber: '' });
  }

  removePhone(phone: PhoneNumber): void {
    if (!this.contactId) return;
    this.store.dispatch(ContactsActions.removePhoneNumber({
      contactId: this.contactId,
      number: phone.number,
      label: phone.label,
    }));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();

    const hasAddress = v.street || v.city || v.postalCode || v.country;
    const address = hasAddress
      ? { street: v.street || undefined, city: v.city || undefined, state: v.state || undefined, postalCode: v.postalCode || undefined, country: v.country || undefined }
      : undefined;

    if (this.isEdit) {
      this.store.dispatch(ContactsActions.updateContact({
        id: this.contactId!,
        request: {
          firstName: v.firstName!,
          lastName: v.lastName!,
          email: v.email || undefined,
          birthday: v.birthday ? (v.birthday as Date).toISOString().split('T')[0] : undefined,
          notes: v.notes || undefined,
          iban: v.iban || undefined,
          address,
        },
      }));
    } else {
      const number = v.phoneNumber?.trim();
      if (!number) {
        this.showPhoneError = true;
        return;
      }
      this.showPhoneError = false;
      this.store.dispatch(ContactsActions.createContact({
        request: {
          firstName: v.firstName!,
          lastName: v.lastName!,
          email: v.email || undefined,
          birthday: v.birthday ? (v.birthday as Date).toISOString().split('T')[0] : undefined,
          notes: v.notes || undefined,
          iban: v.iban || undefined,
          phone: { number, label: v.phoneLabel || 'mobile' },
          address,
        },
      }));
    }
  }

  cancel(): void {
    this.router.navigate(['/contacts']);
  }
}
