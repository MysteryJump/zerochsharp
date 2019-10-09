import { Component, OnInit } from '@angular/core';
import { AppState, selectAuthState } from './store/app.states';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { GetStatus } from './store/actions/auth.actions';
import { UserSession } from './models/session';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(private store: Store<AppState>) {
    this.getState = this.store.select(selectAuthState);
  }
  getState: Observable<any>;
  title = 'app';
  isAuthed: false;
  user = null;
  session: UserSession = null;
  ngOnInit(): void {
    this.store.dispatch(new GetStatus);
    this.getState.subscribe((state) => {
      this.isAuthed = state.isAuthed;
      this.session = state.session;
    });
  }
}
