import { LeftDrawerTabItem } from '../states/tabState';
import actionCreatorFactory from 'typescript-fsa';

const actionCreator = actionCreatorFactory();

export const tabActions = {
  addTabItem: actionCreator<{item: LeftDrawerTabItem}>('ACTION_ADD_TAB_ITEM'),
  removeTabItem: actionCreator<{item: LeftDrawerTabItem}>(
    'ACTION_REMOVE_TAB_ITEM'
  )
};
