import { reducerWithInitialState } from 'typescript-fsa-reducers';
import { tabActions } from '../actions/tabActions';

export enum TabType {
  Thread = 'Thread',
  Board = 'Board',
  Home = ''
}

export interface LeftDrawerTabItem {
  tabType: TabType;
  id: number;
  key: string;
  name: string;
}

export interface ThreadTabItem extends LeftDrawerTabItem {
  boardKey: string;
  threadKey: string;
  boardName: string;
}

export interface BoardTabItem extends LeftDrawerTabItem {
  boardKey: string;
}

export const HomeTabItem: LeftDrawerTabItem = {
  tabType: TabType.Home,
  id: -1,
  key: 'home',
  name: 'Home'
};

export interface TabListState {
  tabs: LeftDrawerTabItem[];
}

const initialState: TabListState = {
  tabs: [HomeTabItem]
};

export const tabReducers = reducerWithInitialState(initialState)
  .case(tabActions.addTabItem, (state, payload) => {
    return { ...state, tabs: [...state.tabs, payload.item] };
  })
  .case(tabActions.removeTabItem, (state, payload) => {
    return { ...state, tabs: state.tabs.filter(x => x !== payload.item) };
  });
