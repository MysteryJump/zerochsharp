import { reducerWithInitialState } from 'typescript-fsa-reducers';
import Axios from 'axios';
import { call, put, takeEvery } from 'redux-saga/effects';
import { sessionActions } from '../actions/sessionActions';

export interface SessionState {
  sesssionToken?: string;
  user?: User;
  logined: boolean;
}

export interface User {
  userId: string;
  authority: Authority;
  setAuthorization: string;
}

export enum Authority {
  Normal = 1,
  Admin = 1 << 1,
  Restricted = 1 << 2
}

const initialState: SessionState = { logined: false };

const loginWithCookieBase = () => {
  return Axios.get<User>(`/api/auth`)
    .then(x => x.data)
    .catch(x => x);
};

const loginWithPasswordBase = (userId: string, password: string) => {
  return Axios.post<User>(`/api/auth`, { userId, password })
    .then(x => x.data)
    .catch(x => x);
};

const signupBase = (userId: string, password: string) => {

}

function* loginWithCookie(action: any) {
  try {
    const session = yield call(loginWithCookieBase);
    yield put({
      type: sessionActions.loginWithCookieSucceeded,
      payload: { user: session }
    });
  } catch (e) {
    yield put({
      type: sessionActions.loginWithCookieFailed,
      payload: { error: e }
    });
  }
}

function* loginWithPassword(action: any) {
  try {
    const session = yield call(
      loginWithPasswordBase,
      action.payload.userId,
      action.payload.password
    );
    yield put({
      type: sessionActions.loginWithCookieSucceeded,
      payload: { user: session }
    });
  } catch (e) {
    yield put({
      type: sessionActions.loginWithCookieFailed,
      payload: { error: e }
    });
  }
}

function* signup(action :any) {
  try {
    
  } catch (e) {

  }
}

export const sessionReducers = reducerWithInitialState(initialState)
  .case(sessionActions.loginWithCookieSucceeded, (state, payload) => {
    return {
      ...state,
      sesssionToken: payload.user.setAuthorization,
      user: payload.user,
      logined: true
    };
  })
  .case(sessionActions.loginWithCookieFailed, (state, payload) => {
    console.error(payload.error);
    return state;
  })
  .case(sessionActions.loginWithPasswordSucceeded, (state, payload) => {
    return {
      ...state,
      sesssionToken: payload.user.setAuthorization,
      user: payload.user,
      logined: true
    };
  })
  .case(sessionActions.loginWithPasswordFailed, (state, payload) => {
    console.error(payload.error);
    return state;
  })
  .case(sessionActions.logoutSession, (state, payload) => {
    return {
      ...state,
      sesssionToken: undefined,
      user: undefined,
      logined: false
    };
  });

export function* sessionSaga() {
  yield takeEvery(sessionActions.loginWithCookie, loginWithCookie);
  yield takeEvery(sessionActions.loginWithPassword, loginWithPassword);
}
