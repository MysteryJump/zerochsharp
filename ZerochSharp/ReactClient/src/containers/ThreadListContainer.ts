import { Action } from 'typescript-fsa';
import { Dispatch } from 'redux';
import { AppState } from '../store';
import { connect } from 'react-redux';
import { ThreadList } from '../components/ThreadList';
import { RouterState } from 'connected-react-router';
import { extend } from '../extension/extend';
import { mainActions } from '../actions/mainActions';
import { MainContainerActions } from '../actions/mainContainerActions';

export interface ThreadListActions extends MainContainerActions {
  getThreadList: () => Action<void>;
}

const mapDispatchToProps = (
  dispatch: Dispatch<Action<void | { name: string }>>
) => {
  return {
    // getThreadList: () => dispatch(boardListActions.fetchBoardList())
    setCurrentName: (name: string) =>
      dispatch(mainActions.replaceCurrentName({ name: name }))
  };
};

const mapStateToProps = (appState: AppState): RouterState => {
  return Object.assign(
    {},
    extend(appState.router, extend(appState.mainState, appState.sessionState))
  );
};

export default connect(mapStateToProps, mapDispatchToProps)(ThreadList);
