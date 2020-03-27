import React from 'react';
import {
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Button
} from '@material-ui/core';
import { Response } from '../../models/response';
import Axios from 'axios';

interface Props {
  checkedResponses: number[];
  setCheckedResponses: (value: React.SetStateAction<number[]>) => void;
  removeResponseDialogOpen: boolean;
  setRemoveResponseDialogOpen: (value: React.SetStateAction<boolean>) => void;
  boardKey: string;
  threadId: string;
  responses: Response[];
  afterActions: () => void;
}

export const RemoveResponseDialog = (props: Props) => {
  const removeResponses = () => {
    const promises: Promise<any>[] = [];
    for (const index of props.checkedResponses) {
      const response = props.responses[index];
      promises.push(
        Axios.delete(
          `/api/boards/${props.boardKey}/${props.threadId}/${response.id}?remove=false`
        )
      );
    }
    Promise.all(promises)
      .then(() => {
        props.afterActions();
        props.setCheckedResponses([]);
      })
      .catch(x => console.error(x));
  };
  return (
    <Dialog
      open={props.removeResponseDialogOpen}
      onClose={() => props.setRemoveResponseDialogOpen(false)}
    >
      <DialogTitle id="remove-response-confirm-dialog">Confirm</DialogTitle>
      <DialogContent>
        Do you want to delete {props.checkedResponses.length} responses from
        thread?
      </DialogContent>
      <DialogActions>
        <Button
          onClick={() => {
            removeResponses();
            props.setRemoveResponseDialogOpen(false);
          }}
        >
          Yes
        </Button>
        <Button onClick={() => props.setRemoveResponseDialogOpen(false)}>
          Cancel
        </Button>
      </DialogActions>
    </Dialog>
  );
};
