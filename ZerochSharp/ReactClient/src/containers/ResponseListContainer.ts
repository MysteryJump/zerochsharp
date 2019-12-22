import { Action } from 'typescript-fsa';
import { Dispatch } from 'redux';
import { AppState } from '../store';
import { connect } from 'react-redux';
import { ResponseList } from '../components/ResponseList';
import { extend } from '../extension/extend';
import { MainContainerActions } from '../actions/mainContainerActions';
import { mainActions } from '../actions/mainActions';
import { withRouter } from 'react-router-dom';

export interface ResponseListActions extends MainContainerActions {}

const mapDispatchToProps = (dispatch: Dispatch<Action<{ name: string }>>) => {
  return {
    setCurrentName: (name: string) =>
      dispatch(mainActions.replaceCurrentName({ name: name }))
  };
};

const mapStateToProps = (appState: AppState) => {
  return Object.assign({}, extend(appState.drawerState, appState.mainState));
};

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(ResponseList));
