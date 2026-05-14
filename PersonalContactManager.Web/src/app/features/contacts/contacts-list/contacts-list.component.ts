import { Component, OnInit, OnDestroy, inject, signal, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Store } from '@ngrx/store';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { AvatarModule } from 'primeng/avatar';
import { SkeletonModule } from 'primeng/skeleton';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SelectModule } from 'primeng/select';
import { DialogModule } from 'primeng/dialog';
import { ProgressBarModule } from 'primeng/progressbar';
import { ConfirmationService } from 'primeng/api';
import { ContactsActions } from '../../../store/contacts/contacts.actions';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import {
  selectContactItems,
  selectContactsLoading,
  selectContactsError,
  selectContactsTotalCount,
  selectContactsSearchParams,
} from '../../../store/contacts/contacts.selectors';
import { selectAllTags } from '../../../store/tags/tags.selectors';
import { selectAllGroups } from '../../../store/groups/groups.selectors';
import { ContactSearchParams } from '../../../core/models/contact.model';

interface SortOption { label: string; sortBy: 'name' | 'createdAt'; sortDirection: 'asc' | 'desc'; }

@Component({
  selector: 'app-contacts-list',
  imports: [
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    AvatarModule,
    SkeletonModule,
    ConfirmDialogModule,
    SelectModule,
    DialogModule,
    ProgressBarModule,
    PageHeaderComponent,
  ],
  providers: [ConfirmationService],
  templateUrl: './contacts-list.component.html',
})
export class ContactsListComponent implements OnInit, OnDestroy {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  private store = inject(Store);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private confirm = inject(ConfirmationService);

  contacts = this.store.selectSignal(selectContactItems);
  loading = this.store.selectSignal(selectContactsLoading);
  totalCount = this.store.selectSignal(selectContactsTotalCount);
  searchParams = this.store.selectSignal(selectContactsSearchParams);
  tags = this.store.selectSignal(selectAllTags);
  groups = this.store.selectSignal(selectAllGroups);

  searchTerm = signal('');
  selectedTagId = signal<string | null>(null);
  selectedGroupId = signal<string | null>(null);
  selectedSort = signal<SortOption | null>(null);

  favoritesOnly = signal(false);
  hasBirthday = signal(false);

  importDialogVisible = signal(false);
  importPreview = signal('');
  importing = signal(false);

  readonly sortOptions: SortOption[] = [
    { label: 'Name A → Z', sortBy: 'name', sortDirection: 'asc' },
    { label: 'Name Z → A', sortBy: 'name', sortDirection: 'desc' },
    { label: 'Newest first', sortBy: 'createdAt', sortDirection: 'desc' },
    { label: 'Oldest first', sortBy: 'createdAt', sortDirection: 'asc' },
  ];

  private qpSub!: Subscription;

  ngOnInit(): void {
    this.qpSub = this.route.queryParamMap.subscribe(qp => {
      this.favoritesOnly.set(qp.get('favoritesOnly') === 'true');
      this.hasBirthday.set(qp.get('hasBirthday') === 'true');
      this.load();
    });
  }

  ngOnDestroy(): void {
    this.qpSub.unsubscribe();
  }

  load(page = 1): void {
    const sort = this.selectedSort();
    const params: ContactSearchParams = {
      searchTerm: this.searchTerm() || undefined,
      tagId: this.selectedTagId() ?? undefined,
      groupId: this.selectedGroupId() ?? undefined,
      favoritesOnly: this.favoritesOnly() || undefined,
      hasBirthday: this.hasBirthday() || undefined,
      sortBy: sort?.sortBy,
      sortDirection: sort?.sortDirection,
      page,
      pageSize: 20,
    };
    this.store.dispatch(ContactsActions.loadContacts({ params }));
  }

  onSearch(): void { this.load(1); }
  onPage(event: { first: number; rows: number }): void { this.load(Math.floor(event.first / event.rows) + 1); }

  open(id: string): void { this.router.navigate(['/contacts', id]); }
  create(): void { this.router.navigate(['/contacts/new']); }

  toggleFavorite(event: Event, id: string): void {
    event.stopPropagation();
    this.store.dispatch(ContactsActions.toggleFavorite({ id }));
  }

  confirmDelete(event: Event, id: string): void {
    event.stopPropagation();
    this.confirm.confirm({
      target: event.target as EventTarget,
      message: 'Delete this contact?',
      accept: () => this.store.dispatch(ContactsActions.deleteContact({ id })),
    });
  }

  exportCsv(): void {
    this.store.dispatch(ContactsActions.exportCsv());
  }

  openImportDialog(): void {
    this.importPreview.set('');
    this.importDialogVisible.set(true);
  }

  onFileSelected(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = (e) => this.importPreview.set((e.target?.result as string) ?? '');
    reader.readAsText(file);
  }

  confirmImport(): void {
    const csv = this.importPreview();
    if (!csv) return;
    this.importing.set(true);
    this.store.dispatch(ContactsActions.importCsv({ csvContent: csv }));
    this.importDialogVisible.set(false);
    this.importing.set(false);
    if (this.fileInput?.nativeElement) this.fileInput.nativeElement.value = '';
    this.importPreview.set('');
  }
}
