import React, { useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { AppState } from '../../../store';
import {
  List,
  ListItem,
  Button,
  ListItemText,
  ListItemSecondaryAction,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField
} from '@material-ui/core';
import { routerActions } from 'connected-react-router';
import Axios from 'axios';
import { boardListActions } from '../../../actions/boardListActions';

export const Boards = () => {
  const boardList = useSelector(
    (appState: AppState) => appState.boardListState
  );
  const dispatch = useDispatch();
  const [boardKey, setBoardKey] = useState('');
  const [boardName, setBoardName] = useState('');
  const [boardDefaultName, setBoardDefaultName] = useState('名無しさん');
  const [boardSubTitle, setBoardSubTitle] = useState('');
  const [addBoardDialogStatus, setAddBoardDialogStatus] = useState(false);

  const addBoard = () => {
    Axios.post('/api/boards', {
      boardKey,
      boardDefaultName,
      boardName,
      boardSubTitle
    })
      .then(x => {
        dispatch(boardListActions.fetchBoardList);
      })
      .catch(x => console.error(x));
  };

  return (
    <>
      <p>Hello, Boards Admin Area!</p>
      <List>
        {boardList.boards.map(x => (
          <ListItem>
            <ListItemText>{x.boardName}</ListItemText>
            <ListItemSecondaryAction>
              <Button
                onClick={() =>
                  dispatch(routerActions.push(`/${x.boardKey}/setting`))
                }
              >
                Show Detail
              </Button>
            </ListItemSecondaryAction>
          </ListItem>
        ))}
      </List>
      <Button onClick={() => setAddBoardDialogStatus(true)}>Add Board</Button>
      <Dialog
        open={addBoardDialogStatus}
        onClose={() => setAddBoardDialogStatus(false)}
      >
        <DialogTitle>Add Board</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Board Key"
            required
            value={boardKey}
            onChange={e => setBoardKey(e.target.value)}
          />
          <TextField
            fullWidth
            label="Board Name"
            required
            value={boardName}
            onChange={e => setBoardName(e.target.value)}
          />
          <TextField
            fullWidth
            label="Board Default Name"
            required
            value={boardDefaultName}
            onChange={e => setBoardDefaultName(e.target.value)}
          />
          <TextField
            fullWidth
            label="Board Sub Title"
            value={boardSubTitle}
            onChange={e => setBoardSubTitle(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => {
              setAddBoardDialogStatus(false);
              addBoard();
            }}
          >
            Submit
          </Button>
          <Button onClick={() => setAddBoardDialogStatus(false)}>Cancel</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
