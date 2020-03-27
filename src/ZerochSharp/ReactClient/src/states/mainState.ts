import { reducerWithInitialState } from 'typescript-fsa-reducers';
import { mainActions } from '../actions/mainActions';

export interface MainState {
  beforePath: string;
  currentName: string;
}

const initialState: MainState = {
  beforePath: '/',
  currentName: 'Home'
};

export const mainReducers = reducerWithInitialState(initialState)
  .case(mainActions.replaceCurrentPath, (state, payload) => {
    return { ...state, beforePath: payload.path };
  })
  .case(mainActions.replaceCurrentName, (state, payload) => {
    return { ...state, currentName: payload.name };
  });
