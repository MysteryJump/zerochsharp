import actionCreatorFactory from 'typescript-fsa';

const actionCreator = actionCreatorFactory();

export const mainActions = {
  replaceCurrentPath: actionCreator<{ path: string }>(
    'ACTION_REPLACE_CURRENT_PATH'
  ),
  replaceCurrentName: actionCreator<{ name: string }>(
    'ACTION_REPLACE_CURRENT_NAME'
  )
};
