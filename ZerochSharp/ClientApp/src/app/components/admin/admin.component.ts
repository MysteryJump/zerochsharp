import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AppState, selectAuthState } from 'src/app/store/app.states';
import { Store } from '@ngrx/store';
import { Router } from '@angular/router';
import { UserSession } from 'src/app/models/session';
import { Authority } from 'src/app/models/authority';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  getState: Observable<any>;
  constructor(private store: Store<AppState>, private router: Router) {
    this.getState = this.store.select(selectAuthState);
  }
  statedCount = 0;
  isAdmin = true;
  ngOnInit() {
    this.getState.subscribe(state => {
      if (state.session === null && this.statedCount > 0) {
        this.router.navigateByUrl('/');
        this.statedCount = 0;
        return;
      }
      if (state.session !== null) {
        this.isAdmin =
          ((state.session as UserSession).authority & Authority.Admin) ===
          Authority.Admin;
        if (!this.isAdmin) {
          this.router.navigateByUrl('/');
          this.statedCount = 0;
          return;
        }
      }
      this.statedCount++;
    });
  }
}
