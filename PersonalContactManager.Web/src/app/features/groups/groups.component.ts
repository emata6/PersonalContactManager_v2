import { Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TextareaModule } from 'primeng/textarea';
import { AvatarModule } from 'primeng/avatar';
import { SkeletonModule } from 'primeng/skeleton';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService } from 'primeng/api';
import { GroupsActions } from '../../store/groups/groups.actions';
import { selectAllGroups, selectGroupsLoading, selectGroupsSaving } from '../../store/groups/groups.selectors';
import { GroupsService } from '../../core/services/groups.service';
import { Group } from '../../core/models/group.model';
import { ContactSummary } from '../../core/models/contact.model';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { FormLabelComponent } from '../../shared/components/form-label/form-label.component';

@Component({
  selector: 'app-groups',
  imports: [
    FormsModule, TableModule, ButtonModule, InputTextModule,
    DialogModule, ConfirmDialogModule, TextareaModule, AvatarModule, SkeletonModule,
    TooltipModule, PageHeaderComponent, FormLabelComponent,
  ],
  providers: [ConfirmationService],
  templateUrl: './groups.component.html',
})
export class GroupsComponent implements OnInit {
  private store = inject(Store);
  private groupsService = inject(GroupsService);
  private router = inject(Router);
  private confirm = inject(ConfirmationService);

  groups = this.store.selectSignal(selectAllGroups);
  loading = this.store.selectSignal(selectGroupsLoading);
  saving = this.store.selectSignal(selectGroupsSaving);

  editDialogVisible = false;
  editingGroup: Group | null = null;
  groupName = '';
  groupDesc = '';

  membersDialogVisible = false;
  viewingGroup: Group | null = null;
  members = signal<ContactSummary[]>([]);
  membersLoading = signal(false);

  ngOnInit(): void {
    this.store.dispatch(GroupsActions.loadGroups());
  }

  openEditDialog(group?: Group): void {
    this.editingGroup = group ?? null;
    this.groupName = group?.name ?? '';
    this.groupDesc = group?.description ?? '';
    this.editDialogVisible = true;
  }

  save(): void {
    if (!this.groupName.trim()) return;
    if (this.editingGroup) {
      this.store.dispatch(GroupsActions.updateGroup({ id: this.editingGroup.id, request: { name: this.groupName, description: this.groupDesc } }));
    } else {
      this.store.dispatch(GroupsActions.createGroup({ request: { name: this.groupName, description: this.groupDesc } }));
    }
    this.editDialogVisible = false;
  }

  openMembers(group: Group): void {
    this.viewingGroup = group;
    this.members.set([]);
    this.membersLoading.set(true);
    this.membersDialogVisible = true;
    this.groupsService.getContacts(group.id).subscribe({
      next: (contacts) => { this.members.set(contacts); this.membersLoading.set(false); },
      error: () => this.membersLoading.set(false),
    });
  }

  openContact(id: string): void {
    this.membersDialogVisible = false;
    this.router.navigate(['/contacts', id]);
  }

  confirmDelete(event: Event, id: string): void {
    this.confirm.confirm({
      target: event.target as EventTarget,
      message: 'Delete this group?',
      accept: () => this.store.dispatch(GroupsActions.deleteGroup({ id })),
    });
  }
}
