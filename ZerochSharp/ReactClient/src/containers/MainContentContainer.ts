import { Action } from 'typescript-fsa';
import { drawerActions } from '../actions/drawerAction';
import { Dispatch } from 'redux';
import { AppState } from '../store';
import { connect } from 'react-redux';
import { MainContent } from '../components/MainContent';
import { extend } from '../extension/extend';
import { MainContainerActions } from '../actions/mainContainerActions';
import { mainActions } from '../actions/mainActions';
import { sessionActions } from '../actions/sessionActions';

export interface MainContentActions extends MainContainerActions {
  handleDrawerOpen: () => Action<void>;
  handleDrawerClose: () => Action<void>;
  fetchCurrentSession: () => Action<void>;
  sessionLogout: () => Action<void>; 
}

const mapDispatchToProps = (
  dispatch: Dispatch<Action<void | { name: string }>>
) => {
  return {
    handleDrawerOpen: () => dispatch(drawerActions.openDrawer()),
    handleDrawerClose: () => dispatch(drawerActions.closeDrawer()),
    setCurrentName: (name: string) =>
      dispatch(mainActions.replaceCurrentName({ name: name })),
    fetchCurrentSession: () => dispatch(sessionActions.loginWithCookie()),
    sessionLogout: () => dispatch(sessionActions.logoutSession())
  };
};

const mapStateToProps = (appState: AppState) => {
  return Object.assign(
    {},
    extend(
      appState.drawerState,
      extend(appState.mainState, appState.sessionState)
    )
  );
};

export default connect(mapStateToProps, mapDispatchToProps)(MainContent);
