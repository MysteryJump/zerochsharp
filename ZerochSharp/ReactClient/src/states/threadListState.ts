import { reducerWithInitialState } from 'typescript-fsa-reducers';
import { threadListActions } from '../actions/threadListActions';
import Axios from 'axios';
import { call, put, takeEvery } from 'redux-saga/effects';
import { boardListActions } from '../actions/boardListActions';

interface Thread {}
export interface Board {
  threads: Thread[];
  boardKey: string;
  boardName: string;
}

export interface ThreadListState {
  threads: Thread[];
  boardKey: string;
  boardName: string;
}

const initialState: ThreadListState = {
  threads: [],
  boardKey: '',
  boardName: ''
};

const getThreadList = (boardKey: string) => {
  return Axios.get<Board>(`/api/boards/${boardKey}`)
    .then(x => x.data)
    .catch(x => x);
};

function* fetchThreadList(action: any) {
  try {
    const board = call(getThreadList, action.boardKey);
    yield put({
      type: threadListActions.fetchThreadListSucceeded,
      payload: { board: board }
    });
  } catch (e) {
    yield put({
      type: threadListActions.fetchThreadListFailed,
      payload: { error: e }
    });
  }
}

export const threadListReducers = reducerWithInitialState(initialState)
  .case(threadListActions.fetchThreadListSucceeded, (state, payload) => {
    const newState = Object.assign({}, state);
    newState.threads = payload.board.threads;
    newState.boardKey = payload.board.boardKey;
    newState.boardName = payload.board.boardName;
    return newState;
  })
  .case(threadListActions.fetchThreadListFailed, (state, payload) => {
    return state;
  });

export function* threadListSaga() {
  yield takeEvery(boardListActions.fetchBoardList, fetchThreadList);
}
