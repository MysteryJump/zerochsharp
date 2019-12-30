import { reducerWithInitialState } from 'typescript-fsa-reducers';
import Axios from 'axios';
import { call, put, takeEvery } from 'redux-saga/effects';
import { sessionActions } from '../actions/sessionActions';
import { User } from '../models/user';

export interface SessionState {
  sesssionToken?: string;
  user?: User;
  logined: boolean;
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
  return Axios.post<{ userId: string }>(`/api/users`, { userId, password })
    .then(x => x.data)
    .catch(x => x);
};

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

function* signup(action: any) {
  try {
    const user = yield call(
      signupBase,
      action.payload.userId,
      action.payload.password
    );
    yield put({
      type: sessionActions.signupSucceeded,
      payload: { userId: user.userId }
    });
    try {
      const session = yield call(
        loginWithPasswordBase,
        action.payload.userId,
        action.payload.password
      );
      yield put({
        type: sessionActions.loginWithPasswordSucceeded,
        payload: { user: session }
      });
    } catch (e) {
      yield put({
        type: sessionActions.loginWithPasswordFailed,
        payload: { error: e }
      });
    }
  } catch (e) {
    yield put({
      type: sessionActions.signupFailed,
      payload: { error: e }
    });
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
    return Object.assign({}, state);
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
    return Object.assign({}, state);
  })
  .case(sessionActions.logoutSession, (state, payload) => {
    return {
      ...state,
      sesssionToken: undefined,
      user: undefined,
      logined: false
    };
  })
  .case(sessionActions.signupSucceeded, (state, payload) => {
    return Object.assign({}, state);
  })
  .case(sessionActions.signupFailed, (state, payload) => {
    console.error(payload.error);
    return Object.assign({}, state);
  });

export function* sessionSaga() {
  yield takeEvery(sessionActions.loginWithCookie, loginWithCookie);
  yield takeEvery(sessionActions.loginWithPassword, loginWithPassword);
  yield takeEvery(sessionActions.signup, signup);
}
