import { Action } from '@ngrx/store';
import { Session, UserSession } from './../../models/session';

export enum UserSessionActionTypes {
  Login = '[User Session] Login',
  LoginSuccess = '[User Session] Login Success',
  LoginFailure = '[User Session] Login Failure',
  GetStatus = '[User Session] Get Status',
  Logout = '[User Session] Logout',
  Signup = '[User Session] Sign up',
  SignupSucess = '[User Session] Sign up Success',
  SignupFailure = '[User Session] Sign up Failure'
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

export class Logout implements Action {
  readonly type = UserSessionActionTypes.Logout;
}

export class Signup implements Action {
  readonly type = UserSessionActionTypes.Signup;
  constructor(public payload: any) {}
}

export class SignupSuccess implements Action {
  readonly type = UserSessionActionTypes.SignupSucess;
  constructor(public payload: any) {}
}

export class SignupFailure implements Action {
  readonly type = UserSessionActionTypes.SignupFailure;
  constructor(public payload: any) {}
}

export type UserSessionActions =
  | LogIn
  | LoginSuccess
  | LoginFailure
  | Logout
  | GetStatus
  | Signup
  | SignupSuccess
  | SignupFailure;
