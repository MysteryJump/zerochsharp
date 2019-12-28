import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  TextField,
  DialogContent,
  DialogActions,
  Button
} from '@material-ui/core';

interface OwnProps {
  open: boolean;
  setDialogStatus: React.Dispatch<React.SetStateAction<boolean>>;
}

type Props = OwnProps;

export const Signup = (props: Props) => {
  const [userId, setUserId] = useState('');
  const [password, setPassword] = useState('');
  const [isValid, setIsValid] = useState(false);

  return (
    <>
      <Dialog open={props.open} onClose={() => props.setDialogStatus(false)}>
        <DialogTitle id="signup-form-dialog-title">Signup</DialogTitle>
        <DialogContent>
          <TextField
            label="User Name"
            margin="dense"
            fullWidth
            onChange={e => setUserId(e.target.value)}
            value={userId}
            required
          />
          <TextField
            label="Password"
            margin="dense"
            fullWidth
            onChange={e => setPassword(e.target.value)}
            value={password}
            type="password"
            required
          />
          <TextField
            label="Password check"
            margin="dense"
            fullWidth
            type="password"
            onChange={e => {
              if (e.target.value !== password) {
                setIsValid(false);
              } else {
                setIsValid(true);
              }
            }}
            required
            error={!isValid}
          />
        </DialogContent>
        <DialogActions>
          <Button>Signup</Button>
          <Button onClick={() => props.setDialogStatus(false)}>Cancel</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
