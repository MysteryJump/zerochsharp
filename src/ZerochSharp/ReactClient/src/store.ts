import { combineReducers, createStore, compose, applyMiddleware } from 'redux';
import {
  BoardListState,
  boardListReducer,
  boardListSaga,
} from './states/boardListState';
import createSagaMiddleware from 'redux-saga';
import { DrawerState, drawerReducer } from './states/drawerState';
import { createBrowserHistory } from 'history';
import {
  RouterState,
  connectRouter,
  routerMiddleware,
} from 'connected-react-router';
import { tabReducers, TabListState } from './states/tabState';
import { mainReducers, MainState } from './states/mainState';
import {
  sessionSaga,
  sessionReducers,
  SessionState,
} from './states/sessionState';
import Axios from 'axios';
import { sessionActions } from './actions/sessionActions';
import { boardListActions } from './actions/boardListActions';
import { SnackbarState, snackbarReducers } from './states/snackbarState';

interface ExtendedWindow extends Window {
  __REDUX_DEVTOOLS_EXTENSION__():
    | import('redux').StoreEnhancer<unknown, unknown>
    | undefined;
  __REDUX_DEVTOOLS_EXTENSION_COMPOSE__?: typeof compose;
}
declare var window: ExtendedWindow;

const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;

const sagaMiddleware = createSagaMiddleware();

export const history = createBrowserHistory();

export type AppState = {
  boardListState: BoardListState;
  drawerState: DrawerState;
  tabState: TabListState;
  mainState: MainState;
  router: RouterState;
  sessionState: SessionState;
  snackbarState: SnackbarState;
};

export const store = createStore(
  combineReducers<AppState>({
    boardListState: boardListReducer,
    drawerState: drawerReducer,
    tabState: tabReducers,
    mainState: mainReducers,
    sessionState: sessionReducers,
    snackbarState: snackbarReducers,
    router: connectRouter(history),
  }),
  {},
  composeEnhancers(applyMiddleware(sagaMiddleware, routerMiddleware(history)))
);

const runAllSagas = () => {
  sagaMiddleware.run(boardListSaga);
  sagaMiddleware.run(sessionSaga);
};

runAllSagas();
Axios.interceptors.request.use((config) => {
  const token = store.getState().sessionState.sesssionToken;
  if (token) {
    config.headers.Authorization = token;
  }
  return config;
});

store.dispatch(sessionActions.loginWithCookie());
store.dispatch(boardListActions.fetchBoardList());
