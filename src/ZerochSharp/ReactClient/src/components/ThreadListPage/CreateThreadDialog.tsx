import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogActions,
  DialogContent,
  TextField,
  Button
} from '@material-ui/core';
import Axios from 'axios';

interface Props {
  setCreating: (value: React.SetStateAction<boolean>) => void;
  onCreatingCallback: () => void;
  creating: boolean;
  boardKey: string;
}

export const CreateThreadDialog = (props: Props) => {
  const [creatingTitle, setCreatingTitle] = useState('');
  const [creatingName, setCreatingName] = useState('');
  const [creatingBody, setCreatingBody] = useState('');
  const [creatingMail, setCreatingMail] = useState('');
  const sendThread = () => {
    const thread = {
      title: creatingTitle,
      response: {
        name: creatingName,
        mail: creatingMail,
        body: creatingBody
      }
    };
    Axios.post(`/api/boards/${props.boardKey}`, thread)
      .then(x => {
        setCreatingBody('');
        setCreatingMail('');
        setCreatingName('');
        setCreatingTitle('');
        props.setCreating(false);
        props.onCreatingCallback();
      })
      .catch(x => {
        console.error(x);
      });
  };
  return (
    <Dialog
      open={props.creating}
      onClose={() => props.setCreating(false)}
      aria-labelledby="form-dialog-title"
    >
      <DialogTitle id="create-thread-form-dialog-title">
        Create Thread
      </DialogTitle>
      <DialogContent>
        <TextField
          label="Title"
          fullWidth
          margin="dense"
          onChange={e => setCreatingTitle(e.target.value)}
          value={creatingTitle}
          required
        />
        <TextField
          label="Name"
          fullWidth
          margin="dense"
          onChange={e => setCreatingName(e.target.value)}
          value={creatingName}
        />
        <TextField
          label="Mail"
          fullWidth
          margin="dense"
          onChange={e => setCreatingMail(e.target.value)}
          value={creatingMail}
        />
        <TextField
          label="Body"
          multiline
          autoFocus
          fullWidth
          rows="14"
          margin="dense"
          onChange={e => setCreatingBody(e.target.value)}
          value={creatingBody}
          required
        />
      </DialogContent>
      <DialogActions>
        <Button
          onClick={() => {
            sendThread();
          }}
          color="primary"
          disabled={creatingBody.length === 0 || creatingTitle.length === 0}
        >
          Submit
        </Button>
        <Button onClick={() => props.setCreating(false)} color="primary">
          Cancel
        </Button>
      </DialogActions>
    </Dialog>
  );
};
