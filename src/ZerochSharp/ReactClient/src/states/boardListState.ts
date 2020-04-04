import { reducerWithInitialState } from 'typescript-fsa-reducers';
import { boardListActions } from '../actions/boardListActions';
import { call, put, takeEvery } from 'redux-saga/effects';
import Axios from 'axios';
import { Board } from '../models/board';

export interface BoardListState {
  boards: Board[];
}

const initialState: BoardListState = {
  boards: []
};

const getBoardList = () => {
  return Axios.get<Board[]>('/api/boards')
    .then(x => x.data)
    .catch(x => x);
};

function* fetchBoardList(action: any) {
  try {
    const boards = yield call(getBoardList);
    yield put({
      type: boardListActions.fetchBoardListSucceeded,
      payload: { boards }
    });
  } catch (e) {
    yield put({
      type: boardListActions.fetchBoardListFailed,
      error: e.message
    });
  }
}

export const boardListReducer = reducerWithInitialState(initialState)
  .case(boardListActions.fetchBoardListFailed, (state, payload) => {
    return state;
  })
  .case(boardListActions.fetchBoardListSucceeded, (state, payload) => {
    const boards = payload.boards.map(item => {
      return {
        ...item,
        key: item.id
      }
    });
    return Object.assign({}, state, { boards: boards });
  });

export function* boardListSaga() {
  yield takeEvery(boardListActions.fetchBoardList, fetchBoardList);
}
