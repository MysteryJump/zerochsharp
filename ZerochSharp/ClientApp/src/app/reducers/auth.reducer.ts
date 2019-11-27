import { Session, UserSession } from '../models/session';
import {
  LogIn,
  LoginSuccess,
  LoginFailure,
  UserSessionActions,
  UserSessionActionTypes
} from '../store/actions/auth.actions';
import { state } from '@angular/animations';

export interface State {
  // loading: boolean;
  session: UserSession | null;
  isAuthed: boolean;
  errorMessage: string | null;
  isAdmin: boolean;
}

export const initialState: State = {
  // loading: true,
  session: null,
  isAuthed: false,
  errorMessage: null,
  isAdmin: false,
};

export function reducer(
  states = initialState,
  action: UserSessionActions
): State {
  switch (action.type) {
    case UserSessionActionTypes.LoginSuccess: {
      return {
        ...states,
        isAuthed: true,
        session: {
          sessionToken: action.payload.sessionToken,
          userName: action.payload.userName,
          createdAt: action.payload.createdAt,
          expired: action.payload.expired,
          authority: action.payload.authority
        },
        errorMessage: null
      };
    }
    case UserSessionActionTypes.LoginFailure: {
      return {
        ...states,
        isAuthed: false,
        errorMessage: 'Incorrenct password or username'
      };
    }
    case UserSessionActionTypes.Logout: {
      return {
        ...states,
        isAuthed: false,
        session: null
      };
    }
    case UserSessionActionTypes.SignupFailure: {
      return {
        ...states,
        isAuthed: false,
        errorMessage: 'This user name is used already.'
      };
    }
    default: {
      return states;
    }
  }
}
