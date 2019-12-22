import actionCreatorFactory from 'typescript-fsa';
import { Board } from '../states/threadListState';

const actionCreator = actionCreatorFactory();

export const threadListActions = {
  fetchThreadList: actionCreator<{ boardKey: string }>(
    'ACTION_FETCH_THREAD_LIST'
  ),
  fetchThreadListSucceeded: actionCreator<{ board: Board }>(
    'ACTION_FETCH_THREAD_LIST_SUCCEEDED'
  ),
  fetchThreadListFailed: actionCreator<{ error: any }>(
    'ACTION_FETCH_THREAD_LIST_FAILED'
  )
};
