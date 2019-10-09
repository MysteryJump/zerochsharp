import { Action } from '@ngrx/store';
import { Session, UserSession } from './../../models/session';

export enum UserSessionActionTypes {
  Login = '[User Session] Login',
  LoginSuccess = '[User Session] Login Success',
  LoginFailure = '[User Session] Login Failure',
  GetStatus = '[User Session] Get Status'
}

export class LogIn implements Action {
  readonly type = UserSessionActionTypes.Login;
  constructor(public payload: any) {}
}

export class LoginSuccess implements Action {
  readonly type = UserSessionActionTypes.LoginSuccess;
  constructor(public payload: any) {}
}

export class LoginFailure implements Action {
  readonly type = UserSessionActionTypes.LoginFailure;
  constructor(public payload: any) {}
}

export class GetStatus implements Action {
  readonly type = UserSessionActionTypes.GetStatus;
}

export type UserSessionActions =
  | LogIn
  | LoginSuccess
  | LoginFailure
  | GetStatus;
