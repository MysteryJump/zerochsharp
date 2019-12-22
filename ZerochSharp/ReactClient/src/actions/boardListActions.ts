import actionCreatorFactory from 'typescript-fsa';
import { Board } from '../states/boardListState';

const actionCreator = actionCreatorFactory();

export const boardListActions = {
  fetchBoardList: actionCreator<void>('ACTIONS_FETCH_BOARDLIST'),
  fetchBoardListSucceeded: actionCreator<{ boards: Board[] }>(
    'ACTIONS_FETCH_BOARDLIST_SUCCEEDED'
  ),
  fetchBoardListFailed: actionCreator<{ error: any }>(
    'ACTION_FETCH_BOARDLIST_FAILED'
  )
};
