import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { AuthService } from 'src/app/services/auth.service';
import {
  UserSessionActions,
  LoginFailure,
  GetStatus,
  Logout,
  Signup,
  SignupFailure,
  SignupSuccess
} from 'src/app/store/actions/auth.actions';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { Action } from '@ngrx/store';
import {
  UserSessionActionTypes,
  LogIn,
  LoginSuccess
} from '../actions/auth.actions';

import { switchMap, tap, concatMap, map, catchError } from 'rxjs/operators';
import { UserSession } from 'src/app/models/session';

@Injectable()
export class AuthEffects {
  constructor(
    private actions$: Actions,
    private authService: AuthService,
    private router: Router
  ) {}

  @Effect()
  LogIn: Observable<any> = this.actions$.pipe(
    ofType<LogIn>(UserSessionActionTypes.Login),
    concatMap((payload: LogIn, index: number) => {
      return this.authService
        .login(payload.payload.username, payload.payload.password)
        .pipe(
          map(result => new LoginSuccess(result)),
          catchError(error => of(new LoginFailure({ error })))
        );
    })
  );

  @Effect({ dispatch: false })
  LogInSuccess: Observable<any> = this.actions$.pipe(
    ofType<LoginSuccess>(UserSessionActionTypes.LoginSuccess),
    tap((user: any) => {
      const u = user.payload as UserSession;
      if (u.sessionToken !== undefined && u.sessionToken !== null) {
        localStorage.setItem('token', u.sessionToken);
      }
      if (this.router.url.indexOf('login') > 0) {
        this.router.navigateByUrl('/');
      }
    })
  );

  @Effect({ dispatch: false })
  LoginFailure: Observable<any> = this.actions$.pipe(
    ofType<LoginFailure>(UserSessionActionTypes.LoginFailure)
  );

  @Effect({ dispatch: false })
  Logout: Observable<any> = this.actions$.pipe(
    ofType<Logout>(UserSessionActionTypes.Logout),
    tap(_ => {
      localStorage.removeItem('token');
    })
  );

  @Effect()
  GetStatus: Observable<any> = this.actions$.pipe(
    ofType<GetStatus>(UserSessionActionTypes.GetStatus),
    concatMap(payload => {
      return this.authService.getStatus().pipe(
        map(result => new LoginSuccess(result)),
        catchError(error => of(new LoginFailure({ error })))
      );
    })
  );

  @Effect()
  Signup: Observable<any> = this.actions$.pipe(
    ofType<Signup>(UserSessionActionTypes.Signup),
    concatMap(payload => {
      return this.authService
        .signup(payload.payload.userName, payload.payload.password)
        .pipe(
          map(result => new SignupSuccess(result)),
          catchError(error => of(new SignupFailure({ error })))
        );
    })
  );

  @Effect({ dispatch: false })
  SignupSuccess: Observable<any> = this.actions$.pipe(
    ofType<SignupSuccess>(UserSessionActionTypes.SignupSucess),
    tap(x => {
      this.router.navigateByUrl('/login');
    })
  );

  @Effect({ dispatch: false })
  SignupFailure: Observable<any> = this.actions$.pipe(
    ofType<SignupFailure>(UserSessionActionTypes.SignupFailure)
  );
}