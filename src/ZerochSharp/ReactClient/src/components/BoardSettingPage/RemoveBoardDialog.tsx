import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  TextField,
  DialogActions,
  Button,
} from '@material-ui/core';
import { Board } from '../../models/board';
import axios from 'axios';
import { useDispatch } from 'react-redux';
import { boardListActions } from '../../actions/boardListActions';
import { routerActions } from 'connected-react-router';

interface Props {
  board?: Board;
  removeDialogOpen: boolean;
  setRemoveDialogOpen: (state: React.SetStateAction<boolean>) => void;
}

export const RemoveBoardDialog = (props: Props) => {
  const dispatch = useDispatch();
  const [boardKeyConfirm, setBoardKeyConfirm] = useState('');
  const removeBoard = () => {
    axios.delete(`/api/boards/${props.board?.boardKey}`)
      .then(x => {
        dispatch(boardListActions.fetchBoardList());
        dispatch(routerActions.push('/'));
      })
      .catch(x => console.error(x));
  };
  return (
    <Dialog
      open={props.removeDialogOpen}
      onClose={() => props.setRemoveDialogOpen(false)}
    >
      <DialogTitle>Remove This Board</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Do you want to remove this board '{props.board?.boardName}' permanently? This
          action cannot be undone, but associated threads and responses remain.
          If you want to continue this action, please input full name of this
          board key.
        </DialogContentText>
        <TextField
          value={boardKeyConfirm}
          onChange={(e) => setBoardKeyConfirm(e.target.value)}
          fullWidth
        />
      </DialogContent>
      <DialogActions>
        <Button
          onClick={() => {
            removeBoard();
            props.setRemoveDialogOpen(false);
          }}
          disabled={boardKeyConfirm !== props.board?.boardKey}
        >
          Submit
        </Button>
        <Button
          onClick={() => {
            props.setRemoveDialogOpen(false);
          }}
        >
          Cancel
        </Button>
      </DialogActions>
    </Dialog>
  );
};
