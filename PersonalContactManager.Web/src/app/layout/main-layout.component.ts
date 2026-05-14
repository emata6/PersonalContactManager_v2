import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Store } from '@ngrx/store';
import { ToastModule } from 'primeng/toast';
import { SidebarComponent } from './sidebar.component';
import { SignalRService } from '../core/services/signalr.service';
import { TagsActions } from '../store/tags/tags.actions';
import { GroupsActions } from '../store/groups/groups.actions';

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, SidebarComponent, ToastModule],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent implements OnInit {
  private store = inject(Store);
  private signalR = inject(SignalRService);

  ngOnInit(): void {
    this.signalR.start();
    this.store.dispatch(TagsActions.loadTags());
    this.store.dispatch(GroupsActions.loadGroups());
  }
}
