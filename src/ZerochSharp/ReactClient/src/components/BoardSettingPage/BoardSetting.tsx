import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { AppState } from '../../store';
import { RouteComponentProps } from 'react-router-dom';
import { boardListActions } from '../../actions/boardListActions';
import { RemoveBoardDialog } from './RemoveBoardDialog';
import { GeneralPanel } from './ExpansionTabs/GeneralPanel';
import { PluginsPanel } from './ExpansionTabs/PluginsPanel';
import { AutoThreadArchivingPanel } from './ExpansionTabs/AutoThreadArchivingPanel';
import { RestrictUsersPanel } from './ExpansionTabs/RestrictUsersPanel';

export const BoardSetting = (
  props: RouteComponentProps<{ boardKey: string }>
) => {
  const boardsState = useSelector(
    (appState: AppState) => appState.boardListState
  );
  const board = boardsState.boards.find(
    (x) => x.boardKey === props.match.params.boardKey
  );

  const [removeDialogOpen, setRemoveDialogOpen] = useState(false);
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(boardListActions.fetchBoardList());
  }, [dispatch]);

  return (
    <>
      <h1>Board Setting</h1>
      <div>
        <GeneralPanel board={board} setRemoveDialogOpen={setRemoveDialogOpen} />
        <RestrictUsersPanel board={board} />
        <PluginsPanel board={board} />
        <AutoThreadArchivingPanel board={board} />
      </div>
      <RemoveBoardDialog
        board={board}
        removeDialogOpen={removeDialogOpen}
        setRemoveDialogOpen={setRemoveDialogOpen}
      />
    </>
  );
};
