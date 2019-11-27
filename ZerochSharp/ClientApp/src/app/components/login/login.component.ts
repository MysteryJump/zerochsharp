import { Component, OnInit, Inject } from '@angular/core';
import {
  HttpClient,
  HttpParams,
  HttpErrorResponse
} from '@angular/common/http';
import { Router } from '@angular/router';
import { UserSession } from 'src/app/models/session';
import { Store } from '@ngrx/store';
import { AppState, selectAuthState } from 'src/app/store/app.states';
import { Observable } from 'rxjs';
import { LogIn } from 'src/app/store/actions/auth.actions';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  public password: string;
  public userName: string;
  public failed: boolean;
  isJumpToHome = false;

  public errorMessage: string | null;
  getState: Observable<any>;
  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private baseApiUrl: string,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router,
    private store: Store<AppState>
  ) {
    this.getState = this.store.select(selectAuthState);
  }
  ngOnInit() {
    this.failed = false;
    this.getState.subscribe(state => {
      this.errorMessage = state.errorMessage;
      this.isJumpToHome = state.isAuthed;
    });
    this.errorMessage = null;
    if (this.isJumpToHome) {
      this.router.navigateByUrl('/');
    }
  }

  loginButtonClicked() {
    const payload = {
      username: this.userName,
      password: this.password
    };
    this.store.dispatch(new LogIn(payload));
  }
}
