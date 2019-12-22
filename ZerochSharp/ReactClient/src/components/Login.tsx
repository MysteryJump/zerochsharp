import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  TextField,
  DialogActions,
  Button
} from '@material-ui/core';
import { LoginActions } from '../containers/LoginContainer';

interface OwnProps {
  open: boolean;
  setDialogStatus: React.Dispatch<React.SetStateAction<boolean>>;
}

type Props = OwnProps & LoginActions;

export const LoginComponent = (props: Props) => {
  const [password, setPassword] = useState('');
  const [userId, setUserId] = useState('');
  return (
    <>
      <Dialog open={props.open} onClose={() => props.setDialogStatus(false)}>
        <DialogTitle id="login-form-dialog-title">Login</DialogTitle>
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
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => {
              props.loginWithPassword(userId, password);
              props.setDialogStatus(false);
            }}
          >
            Login
          </Button>
          <Button onClick={() => props.setDialogStatus(false)}>Cancel</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
