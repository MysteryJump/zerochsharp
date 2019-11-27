import { Component, OnInit } from '@angular/core';
import { AppState, selectAuthState } from 'src/app/store/app.states';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { Signup } from 'src/app/store/actions/auth.actions';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {
  userName: string;
  password: string;
  errorMessage: string | null;
  getState: Observable<any>;
  constructor(private store: Store<AppState>) {
    this.getState = this.store.select(selectAuthState);
  }

  ngOnInit() {
    this.getState.subscribe(x => {
      this.errorMessage = x.errorMessage;
    });
    this.errorMessage = null;

  }

  signUp(): void {
    this.store.dispatch(new Signup({userName: this.userName, password: this.password }));
  }

}
