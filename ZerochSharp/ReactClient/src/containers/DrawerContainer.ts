import { Action } from 'typescript-fsa';
import { Dispatch } from 'redux';
import { drawerActions } from '../actions/drawerAction';
import { AppState } from '../store';
import { connect } from 'react-redux';
import Drawer from '../components/Drawer';
import { boardListActions } from '../actions/boardListActions';
import { DrawerState } from '../states/drawerState';
import { BoardListState } from '../states/boardListState';
import { extend } from '../extension/extend';
import { TabListState, LeftDrawerTabItem } from '../states/tabState';
import { tabActions } from '../actions/tabActions';
import { RouterState, push } from 'connected-react-router';
import { MainState } from '../states/mainState';
import { mainActions } from '../actions/mainActions';

export interface DrawerActions {
  handleDrawerClose: () => Action<void>;
  getBoardList: () => Action<void>;
  removeTabItem: (
    tabItem: LeftDrawerTabItem
  ) => Action<{ item: LeftDrawerTabItem }>;
  addTabItem: (
    tabItem: LeftDrawerTabItem
  ) => Action<{ item: LeftDrawerTabItem }>;
}

const mapDispatchToProps = (
  dispatch: Dispatch<
    Action<void | { item: LeftDrawerTabItem } | { name: string }>
  >
) => {
  return {
    handleDrawerClose: () => dispatch(drawerActions.closeDrawer()),
    handleDrawerOpen: () => dispatch(drawerActions.openDrawer()),
    getBoardList: () => dispatch(boardListActions.fetchBoardList()),
    removeTabItem: (tabItem: LeftDrawerTabItem) =>
      dispatch(tabActions.removeTabItem({ item: tabItem })),
    addTabItem: (tabItem: LeftDrawerTabItem) =>
      dispatch(tabActions.addTabItem({ item: tabItem }))
  };
};

const mapStateToProps = (
  appState: AppState
): DrawerState & BoardListState & TabListState & RouterState & MainState => {
  return Object.assign(
    {},
    extend(
      extend(appState.boardListState, appState.router),
      extend(
        extend(appState.drawerState, appState.tabState),
        appState.mainState
      )
    )
  );
};

export default connect(mapStateToProps, mapDispatchToProps)(Drawer);
