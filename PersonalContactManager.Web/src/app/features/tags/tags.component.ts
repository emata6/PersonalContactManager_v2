import { Component, OnInit, inject, signal } from '@angular/core';
import { Store } from '@ngrx/store';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ColorPickerModule } from 'primeng/colorpicker';
import { ConfirmationService } from 'primeng/api';
import { TagsActions } from '../../store/tags/tags.actions';
import { selectAllTags, selectTagsLoading, selectTagsSaving } from '../../store/tags/tags.selectors';
import { Tag } from '../../core/models/tag.model';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { FormLabelComponent } from '../../shared/components/form-label/form-label.component';

@Component({
  selector: 'app-tags',
  imports: [FormsModule, TableModule, ButtonModule, InputTextModule, DialogModule, ConfirmDialogModule, ColorPickerModule, PageHeaderComponent, FormLabelComponent],
  providers: [ConfirmationService],
  templateUrl: './tags.component.html',
})
export class TagsComponent implements OnInit {
  private store = inject(Store);
  private confirm = inject(ConfirmationService);

  tags = this.store.selectSignal(selectAllTags);
  loading = this.store.selectSignal(selectTagsLoading);
  saving = this.store.selectSignal(selectTagsSaving);

  dialogVisible = false;
  editingTag: Tag | null = null;
  tagName = '';
  tagColor = '#3b82f6';

  ngOnInit(): void {
    this.store.dispatch(TagsActions.loadTags());
  }

  openDialog(tag?: Tag): void {
    this.editingTag = tag ?? null;
    this.tagName = tag?.name ?? '';
    this.tagColor = tag?.color ?? '#3b82f6';
    this.dialogVisible = true;
  }

  save(): void {
    if (!this.tagName.trim()) return;
    if (this.editingTag) {
      this.store.dispatch(TagsActions.updateTag({ id: this.editingTag.id, request: { name: this.tagName, color: this.tagColor } }));
    } else {
      this.store.dispatch(TagsActions.createTag({ request: { name: this.tagName, color: this.tagColor } }));
    }
    this.dialogVisible = false;
  }

  confirmDelete(event: Event, id: string): void {
    this.confirm.confirm({ target: event.target as EventTarget, message: 'Delete this tag?', accept: () => this.store.dispatch(TagsActions.deleteTag({ id })) });
  }
}
