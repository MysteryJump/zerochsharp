import React, { useEffect, useState } from 'react';
import {
  makeStyles,
  Theme,
  Fab,
  IconButton,
  TextField,
  FormControl,
  Tooltip,
  Box
} from '@material-ui/core';
import EditIcon from '@material-ui/icons/Edit';
import CloseIcon from '@material-ui/icons/Close';
import SendIcon from '@material-ui/icons/Send';
import RefreshIcon from '@material-ui/icons/Refresh';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import DeleteIcon from '@material-ui/icons/Delete';
import { RouteComponentProps } from 'react-router-dom';
import Axios, { AxiosResponse } from 'axios';
import { drawerWidth } from './MainContent';
import { useSelector, useDispatch } from 'react-redux';
import { AppState } from '../store';
import { mainActions } from '../actions/mainActions';
import { routerActions } from 'connected-react-router';
import { Response } from '../models/response';
import { Thread } from '../models/thread';
import { ResponseCard } from './ResponseCard';

const useStyles = makeStyles((theme: Theme) => {
  return {
    refreshArea: {
      float: 'right',
      display: 'flex'
    },
    responseListArea: {
      clear: 'both'
    },
    editIconMargin: {
      position: 'fixed',
      bottom: theme.spacing(2),
      right: theme.spacing(2)
    },
    removeIconMargin: {
      position: 'fixed',
      bottom: theme.spacing(2),
      right: theme.spacing(10)
    },
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
    },
    refreshButton: {
      marginBottom: '0.4rem'
    },
    lastRefreshedStatus: {
      margin: '0.8rem',
      marginLeft: '1.5rem',
      fontSize: '1rem'
    }
  };
});

const initialResponses: Response[] = [
  {
    created: '2019-11-21 03:23:45.56',
    author: 'gtDrhEdr2',
    name: '',
    mail: '',
    body: 'うんちぶり',
    isAboned: false,
    threadId: 3,
    id: 32,
    key: 32
  },
  {
    created: '2019-11-21 03:23:45.56',
    author: 'gtDrhEdr2',
    name: '',
    mail: '',
    body: 'うんちぶり',
    isAboned: false,
    threadId: 3,
    id: 32,
    key: 32
  }
];

interface OwnProps
  extends RouteComponentProps<{ threadId: string; boardKey: string }> {}

type Props = OwnProps;

export const ResponseList = (props: Props) => {
  const classes = useStyles();

  const [responses, setResponses] = useState(initialResponses);
  const [isCreating, setIsCreating] = useState(false);
  const [lastRefreshed, setLastRefreshed] = useState(Date.now());
  const [threadName, setThreadName] = useState('');

  const [creatingName, setCreatingName] = useState('');
  const [creatingMail, setCreatingMail] = useState('');
  const [creatingBody, setCreatingBody] = useState('');
  const [boardDefaultName, setBoardDefaultName] = useState('');

  const [checkedResponses, setCheckedResponses] = useState([] as number[]);

  const drawerState = useSelector((appState: AppState) => appState.drawerState);
  const boardListState = useSelector(
    (appState: AppState) => appState.boardListState
  );
  const dispatch = useDispatch();
  const push = (path: string) => dispatch(routerActions.push(path));

  const responseListDisplayStyle = {
    marginBottom: isCreating ? '11rem' : '0rem'
  };
  const creatingAreaDisplayStyle = {
    display: isCreating ? 'initial' : 'none'
  };
  const editShortTextFieldStyle = {
    width: `calc(50% - 0.2rem)`
  };
  const editShortTextAreaStyle = {
    width: 'calc(100% - 0.2rem)',
    display: 'flex'
  };
  const boardKey = props.match.params.boardKey;
  const threadId = props.match.params.threadId;

  const getThread = (boardKey: string, threadId: string) => {
    const apiUrl = `/api/boards/${boardKey}/${threadId}`;
    Axios.get<Thread>(apiUrl)
      .then((x: AxiosResponse<Thread>) => {
        if (x.data.responses) {
          setResponses(x.data.responses);
        }
        setThreadName(x.data.title);
        setLastRefreshed(Date.now());
        dispatch(mainActions.replaceCurrentName({ name: x.data.title }));
        const defName = boardListState.boards.find(x => x.boardKey === boardKey)
          ?.boardDefaultName;
        if (defName) {
          setBoardDefaultName(defName);
        }
      })
      .catch(x => {
        console.error(x);
      });
  };

  const sendResponse = (
    boardKey: string,
    threadId: string,
    body: string,
    name?: string,
    mail?: string
  ) => {
    if (body.length < 1) {
      return;
    }
    const response = {
      body: body,
      name: name,
      mail: mail
    };
    Axios.post(`/api/boards/${boardKey}/${threadId}`, response)
      .then(x => {
        getThread(boardKey, threadId);
        setCreatingBody('');
      })
      .catch(x => {
        console.error(x);
      });
  };

  useEffect(() => {
    getThread(boardKey, threadId);
  }, [boardKey, threadId]);
  return (
    <>
      <div>
        <h1 style={{ margin: '0.3rem' }}>{threadName}</h1>
        <Tooltip title="Back to thread list">
          <IconButton
            onClick={() => {
              push('/' + boardKey);
            }}
          >
            <ArrowBackIcon />
          </IconButton>
        </Tooltip>

        <div className={classes.refreshArea}>
          <Tooltip title="Refresh">
            <IconButton
              onClick={() => {
                getThread(boardKey, threadId);
              }}
              className={classes.refreshButton}
            >
              <RefreshIcon></RefreshIcon>
            </IconButton>
          </Tooltip>
          <Box className={classes.lastRefreshedStatus}>
            Last Refreshed: {new Date(lastRefreshed).toLocaleString('ja-jp')}
          </Box>
        </div>
      </div>
      <div
        className={classes.responseListArea}
        style={responseListDisplayStyle}
      >
        {responses.map((x, index) => {
          const check = checkedResponses.find(y => y === index);
          const checkedAction = (val: boolean) => {
            if (val) {
              setCheckedResponses([...checkedResponses, index]);
            } else {
              setCheckedResponses(checkedResponses.filter(x => x !== index));
            }
          };
          return (
            <ResponseCard
              index={index}
              response={x}
              boardDefaultName={boardDefaultName}
              checked={check !== undefined}
              checkedAction={checkedAction}
            />
          );
        })}
      </div>
      <div style={{ display: isCreating ? 'none' : 'initial' }}>
        <Tooltip
          title="Create"
          placement="top"
          TransitionProps={{ timeout: 0 }}
        >
          <Fab
            size="medium"
            color="primary"
            aria-label="create"
            className={classes.editIconMargin}
            onClick={() => setIsCreating(true)}
          >
            <EditIcon />
          </Fab>
        </Tooltip>
      </div>
      <div style={{display: checkedResponses.length > 0 ? 'initial' : 'none'}}>
        <Tooltip title="Remove Responses">
          <Fab
            size="medium"
            color="primary"
            aria-label="remote response"
            className={classes.removeIconMargin}
          >
            <DeleteIcon />
          </Fab>
        </Tooltip>
      </div>
      <div
        className={classes.editResponseArea}
        style={creatingAreaDisplayStyle}
      >
        <div style={{ display: 'flex' }}>
          <Tooltip
            title="Close"
            placement="top"
            TransitionProps={{ timeout: 0 }}
          >
            <IconButton onClick={() => setIsCreating(false)} aria-label="close">
              <CloseIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Send" placement="top">
            <IconButton
              aria-label="send"
              disabled={creatingBody.length === 0}
              onClick={() =>
                sendResponse(
                  boardKey,
                  threadId,
                  creatingBody,
                  creatingName,
                  creatingMail
                )
              }
            >
              <SendIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip
            title="Refresh"
            placement="top"
            onClick={() => getThread(boardKey, threadId)}
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
    </>
  );
};
