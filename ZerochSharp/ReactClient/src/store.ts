import { combineReducers, createStore, compose, applyMiddleware } from 'redux';
import {
  BoardListState,
  boardListReducer,
  boardListSaga
} from './states/boardListState';
import createSagaMiddleware from 'redux-saga';
import { DrawerState, drawerReducer } from './states/drawerState';
import {
  threadListReducers,
  ThreadListState,
  threadListSaga
} from './states/threadListState';
import { createBrowserHistory } from 'history';
import {
  RouterState,
  connectRouter,
  routerMiddleware
} from 'connected-react-router';
import { tabReducers, TabListState } from './states/tabState';
import { mainReducers, MainState } from './states/mainState';
import {
  sessionSaga,
  sessionReducers,
  SessionState
} from './states/sessionState';
import Axios from 'axios';

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
  threadListState: ThreadListState;
  tabState: TabListState;
  mainState: MainState;
  router: RouterState;
  sessionState: SessionState;
};

export const store = createStore(
  combineReducers<AppState>({
    boardListState: boardListReducer,
    drawerState: drawerReducer,
    threadListState: threadListReducers,
    tabState: tabReducers,
    mainState: mainReducers,
    sessionState: sessionReducers,
    router: connectRouter(history)
  }),
  {},
  composeEnhancers(applyMiddleware(sagaMiddleware, routerMiddleware(history)))
);

sagaMiddleware.run(boardListSaga);
sagaMiddleware.run(threadListSaga);
sagaMiddleware.run(sessionSaga);

Axios.interceptors.request.use(config => {
  const token = store.getState().sessionState.sesssionToken;
  if (token) {
    config.headers.Authorization = token;
  }
  return config;
});
