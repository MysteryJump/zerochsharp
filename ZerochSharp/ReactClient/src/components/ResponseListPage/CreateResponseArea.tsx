import React, { useState } from 'react';
import {
  createStyles,
  makeStyles,
  Theme,
  IconButton,
  Tooltip,
  FormControl,
  TextField
} from '@material-ui/core';
import CloseIcon from '@material-ui/icons/Close';
import RefreshIcon from '@material-ui/icons/Refresh';
import SendIcon from '@material-ui/icons/Send';
import { drawerWidth } from '../MainContent';
import { AppState } from '../../store';
import { useSelector } from 'react-redux';

interface Props {
  boardKey: string;
  threadId: string;
  setIsCreating: (value: React.SetStateAction<boolean>) => void;
  isCreating: boolean;
  sendResponse: (
    boardKey: string,
    threadId: string,
    body: string,
    name?: string,
    mail?: string
  ) => void;
  getThread: (boardKey: string, threadId: string) => void;
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    editResponseArea: {
      position: 'fixed',
      bottom: 0,
      backgroundColor: 'white',
      marginLeft: theme.spacing(-3),
      boxShadow: theme.shadows[15],
      height: '12rem',
      [theme.breakpoints.up('sm')]: {
        width: `calc(100% - ${drawerWidth}px)`
      },
      width: `calc(100%)`
    },
    editResponseMargin: {
      marginLeft: '0.7rem'
    }
  })
);

export const CreateResponseArea = (props: Props) => {
  const classes = useStyles();
  const [creatingName, setCreatingName] = useState('');
  const [creatingMail, setCreatingMail] = useState('');
  const [creatingBody, setCreatingBody] = useState('');

  const drawerState = useSelector((appState: AppState) => appState.drawerState);

  const editShortTextFieldStyle = {
    width: `calc(50% - 0.2rem)`
  };
  const editShortTextAreaStyle = {
    width: 'calc(100% - 0.2rem)',
    display: 'flex'
  };
  const creatingAreaDisplayStyle = {
    display: props.isCreating ? 'initial' : 'none'
  };

  return (
    <div className={classes.editResponseArea} style={creatingAreaDisplayStyle}>
      <div style={{ display: 'flex' }}>
        <Tooltip title="Close" placement="top" TransitionProps={{ timeout: 0 }}>
          <IconButton
            onClick={() => props.setIsCreating(false)}
            aria-label="close"
          >
            <CloseIcon fontSize="small" />
          </IconButton>
        </Tooltip>
        <Tooltip title="Send" placement="top">
          <IconButton
            aria-label="send"
            disabled={creatingBody.length === 0}
            onClick={() => {
              props.sendResponse(
                props.boardKey,
                props.threadId,
                creatingBody,
                creatingName,
                creatingMail
              );
              setCreatingBody('');
            }}
          >
            <SendIcon fontSize="small" />
          </IconButton>
        </Tooltip>
        <Tooltip
          title="Refresh"
          placement="top"
          onClick={() => props.getThread(props.boardKey, props.threadId)}
        >
          <IconButton aria-label="refresh">
            <RefreshIcon fontSize="small" />
          </IconButton>
        </Tooltip>
      </div>
      <FormControl
        style={{
          display: 'block',
          paddingLeft: '1rem',
          paddingRight: '1rem'
        }}
      >
        <div style={editShortTextAreaStyle}>
          <TextField
            style={editShortTextFieldStyle}
            placeholder="Name"
            onChange={e => setCreatingName(e.target.value)}
            value={creatingName}
          />
          <TextField
            style={editShortTextFieldStyle}
            className={classes.editResponseMargin}
            placeholder="Mail"
            onChange={e => setCreatingMail(e.target.value)}
            value={creatingMail}
          />
        </div>
        <div>
          <TextField
            multiline
            rows={4}
            style={{
              marginTop: '1rem',
              width: drawerState.isOpening
                ? `calc(100% - ${drawerWidth}px - 0.2rem)`
                : `calc(100% - 0.2rem)`
            }}
            placeholder="Text"
            onChange={e => setCreatingBody(e.target.value)}
            value={creatingBody}
            autoFocus
            required
          />
        </div>
      </FormControl>
    </div>
  );
};
