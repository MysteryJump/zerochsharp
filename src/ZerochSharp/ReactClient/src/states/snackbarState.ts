import { reducerWithInitialState } from 'typescript-fsa-reducers';
import { snackbarActions } from '../actions/snackbarActions';
import { OptionsObject } from 'notistack';

export interface SnackbarState {
  items: SnackbarItem[];
}

export interface SnackbarItem {
  text: string;
  key: string;
  isDismissed: boolean;
  options: OptionsObject;
}

export const generateDefaultSnackbarItem = (
  text: string,
  key: string
): SnackbarItem => {
  return {
    text,
    key,
    isDismissed: false,
    options: {
      variant: 'default',
    },
  };
};

export const snackbarReducers = reducerWithInitialState<SnackbarState>({
  items: [],
})
  .case(snackbarActions.addSnackbar, (state, payload: SnackbarItem) => {
    return Object.assign({}, { ...state, items: [...state.items, payload] });
  })
  .case(snackbarActions.allRemoveSnackbar, (state) => {
    return { items: [] };
  })
  .case(snackbarActions.dismissSnackbar, (state, payload: { key: string }) => {
    return Object.assign(
      {},
      {
        ...state,
        items: state.items.map((x) =>
          Object.assign({}, { ...x, isDismissed: true })
        ),
      }
    );
  })
  .case(snackbarActions.removeSnackbar, (state, payload: { key: string }) => {
    return Object.assign(
      {},
      { ...state, items: state.items.filter((x) => x.key !== payload.key) }
    );
  });
