import { reducerWithInitialState } from 'typescript-fsa-reducers';
import { drawerActions } from '../actions/drawerAction';

export interface DrawerState {
  isOpening?: boolean;
}

const initialState: DrawerState = {
  isOpening: undefined
};

export const drawerReducer = reducerWithInitialState(initialState)
  .case(drawerActions.closeDrawer, state => {
    return Object.assign({}, { ...state, isOpening: false });
  })
  .case(drawerActions.openDrawer, state => {
    return Object.assign({}, { ...state, isOpening: true });
  });
