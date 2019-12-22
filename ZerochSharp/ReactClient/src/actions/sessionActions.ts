import actionCreatorFactory from 'typescript-fsa';
import { User } from '../states/sessionState';

const actionCreator = actionCreatorFactory();

export const sessionActions = {
  loginWithCookie: actionCreator<void>('ACTION_LOGIN_WITH_COOKIE'),
  loginWithCookieSucceeded: actionCreator<{ user: User }>(
    'ACTION_LOGIN_WITH_COOKIE_SUCCEEDED'
  ),
  loginWithCookieFailed: actionCreator<{ error: any }>(
    'ACTION_LOGIN_WITH_COOKIE_FAILED'
  ),
  loginWithPassword: actionCreator<{ userId: string; password: string }>(
    'ACTION_LOGIN_WITH_PASSWORD'
  ),
  loginWithPasswordSucceeded: actionCreator<{ user: User }>(
    'ACTION_LOGIN_WITH_PASSWORD_SUCCEEDED'
  ),
  loginWithPasswordFailed: actionCreator<{ error: any }>(
    'ACTION_LOGIN_WITH_PASSWORD_FAILED'
  ),
  logoutSession: actionCreator<void>('ACTION_LOGOUT_SESSION')
};
