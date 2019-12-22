import { Dispatch } from 'redux';
import { Action } from 'typescript-fsa';
import { AppState } from '../store';
import { connect } from 'react-redux';
import { LoginComponent } from '../components/Login';
import { sessionActions } from '../actions/sessionActions';

export interface LoginActions {
  loginWithPassword: (
    userId: string,
    password: string
  ) => Action<{ userId: string; password: string }>;
}

const mapDispatchToProps = (
  dispatch: Dispatch<Action<{ userId: string; password: string }>>
) => {
  return {
    loginWithPassword: (userId: string, password: string) =>
      dispatch(
        sessionActions.loginWithPassword({
          userId: userId,
          password: password
        })
      )
  };
};

const mapStateToProps = (appState: AppState) => {};

export default connect(mapStateToProps, mapDispatchToProps)(LoginComponent);
