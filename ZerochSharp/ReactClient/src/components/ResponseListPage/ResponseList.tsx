import React, { useEffect, useState, useCallback } from 'react';
import {
  makeStyles,
  Theme,
  Fab,
  IconButton,
  Tooltip,
  Box
} from '@material-ui/core';
import EditIcon from '@material-ui/icons/Edit';
import RefreshIcon from '@material-ui/icons/Refresh';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import DeleteIcon from '@material-ui/icons/Delete';
import { RouteComponentProps } from 'react-router-dom';
import Axios, { AxiosResponse } from 'axios';
import { useSelector, useDispatch } from 'react-redux';
import { AppState } from '../../store';
import { mainActions } from '../../actions/mainActions';
import { routerActions } from 'connected-react-router';
import { Response } from '../../models/response';
import { Thread } from '../../models/thread';
import { ResponseCard } from './ResponseCard';
import { CreateResponseArea } from './CreateResponseArea';
import { Authority } from '../../models/user';
import { RemoveResponseDialog } from './RemoveResponseDialog';

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

export const ResponseList = (
  props: RouteComponentProps<{ threadId: string; boardKey: string }>
) => {
  const classes = useStyles();

  const [responses, setResponses] = useState(initialResponses);
  const [isCreating, setIsCreating] = useState(false);
  const [lastRefreshed, setLastRefreshed] = useState(Date.now());
  const [threadName, setThreadName] = useState('');

  const [boardDefaultName, setBoardDefaultName] = useState('');
  const [checkedResponses, setCheckedResponses] = useState([] as number[]);

  const [removeResponseDialogOpen, setRemoveResponseDialogOpen] = useState(
    false
  );

  const boardListState = useSelector(
    (appState: AppState) => appState.boardListState
  );
  const sessionState = useSelector(
    (appState: AppState) => appState.sessionState
  );
  const dispatch = useDispatch();
  const push = (path: string) => dispatch(routerActions.push(path));

  const isAdmin =
    sessionState.user != null &&
    (sessionState.user.authority & Authority.Admin) === Authority.Admin;

  const responseListDisplayStyle = {
    marginBottom: isCreating ? '11rem' : '0rem'
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
      })
      .catch(x => {
        console.error(x);
      });
  };
  const getThreadCallback = useCallback(getThread,[]);
  useEffect(() => {
    getThreadCallback(boardKey, threadId);
  }, [boardKey, threadId, getThreadCallback]);
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
              <RefreshIcon />
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
              display={isAdmin}
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
      <div
        style={{
          display: checkedResponses.length > 0 && isAdmin ? 'initial' : 'none'
        }}
      >
        <Tooltip
          title="Remove Responses"
          placement="top"
          TransitionProps={{ timeout: 0 }}
        >
          <Fab
            size="medium"
            color="primary"
            aria-label="remote response"
            className={classes.removeIconMargin}
            onClick={() => setRemoveResponseDialogOpen(true)}
          >
            <DeleteIcon />
          </Fab>
        </Tooltip>
      </div>
      <CreateResponseArea
        boardKey={boardKey}
        threadId={threadId}
        setIsCreating={setIsCreating}
        isCreating={isCreating}
        sendResponse={sendResponse}
        getThread={getThread}
      />
      <RemoveResponseDialog
        checkedResponses={checkedResponses}
        setCheckedResponses={setCheckedResponses}
        removeResponseDialogOpen={removeResponseDialogOpen}
        setRemoveResponseDialogOpen={setRemoveResponseDialogOpen}
        boardKey={boardKey}
        threadId={threadId}
        responses={responses}
        afterActions={() => getThread(boardKey, threadId)}
      />
    </>
  );
};
