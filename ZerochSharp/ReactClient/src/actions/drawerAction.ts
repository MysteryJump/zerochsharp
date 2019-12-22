import actionCreatorFactory from "typescript-fsa";

const actionCreator = actionCreatorFactory();

export const drawerActions = {
  closeDrawer: actionCreator<void>('ACTION_CLOSE_DRAWER'),
  openDrawer: actionCreator<void>('ACTION_OPEN_DRAWER')
}