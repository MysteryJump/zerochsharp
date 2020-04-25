import actionCreatorFactory from 'typescript-fsa';
import { SnackbarItem } from '../states/snackbarState';

const actionCreator = actionCreatorFactory();

export const snackbarActions = {
  addSnackbar: actionCreator<SnackbarItem>('ACTION_ADD_SNACKBAR'),
  removeSnackbar: actionCreator<{ key: string }>('ACTION_REMOVE_SNACKBAR'),
  dismissSnackbar: actionCreator<{ key: string }>('ACTION_DISMISS_SNACKBAR'),
  allRemoveSnackbar: actionCreator<void>('ACTION_ALL_REMOVE_SNACKBAR'),
};
