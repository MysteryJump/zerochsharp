import React, { useState, useEffect, useCallback } from 'react';
import Axios from 'axios';
import {
  makeStyles,
  Theme,
  createStyles,
  Box,
  IconButton,
  Tooltip,
  Fab
} from '@material-ui/core';
import RefreshIcon from '@material-ui/icons/Refresh';
import AddIcon from '@material-ui/icons/Add';
import SettingsIcon from '@material-ui/icons/Settings';
import ArchiveIcon from '@material-ui/icons/Archive';
import { RouteComponentProps } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { HasSystemAuthority, SystemAuthority } from '../../models/user';
import { AppState } from '../../store';
import { mainActions } from '../../actions/mainActions';
import { CreateThreadDialog } from './CreateThreadDialog';
import { routerActions } from 'connected-react-router';
import { BoardState } from './BoardState';
import { ThreadListTable } from './ThreadListTable/ThreadListTable';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    refreshArea: {
      float: 'right',
      display: 'flex'
    },
    refreshButton: {
      marginBottom: '0.4rem'
    },
    lastRefreshedStatus: {
      margin: '0.8rem',
      marginLeft: '1.5rem',
      fontSize: '1rem'
    },
    addIcon: {
      position: 'fixed',
      bottom: theme.spacing(2),
      right: theme.spacing(2)
    }
  })
);

interface OwnProps {}

const initialBoardState: BoardState = {
  boardKey: '',
  boardName: '',
  children: []
};

type Props = OwnProps & RouteComponentProps<{ boardKey: string }>;

export const ThreadList = (props: Props) => {
  const classes = useStyles();

  const [board, setBoard] = useState(initialBoardState);
  const [lastRefreshed, setLastRefreshed] = useState(Date.now());
  const [isCreating, setIsCreating] = useState(false);

  const logined = useSelector((state: AppState) => state.sessionState.logined);
  const user = useSelector((state: AppState) => state.sessionState.user);
  const dispatch = useDispatch();

  const boardKey = props.match.params.boardKey;
  const getBoard = (boardKey: string) => {
    Axios.get<BoardState>(`/api/boards/${boardKey}`)
      .then(x => {
        setBoard(x.data);
        dispatch(mainActions.replaceCurrentName({ name: x.data.boardName }));
      })
      .catch(x => {
        console.error(x);
      });
    setLastRefreshed(Date.now());
  };
  const getBoardCallback = useCallback(getBoard, []);
  useEffect(() => {
    getBoardCallback(boardKey);
  }, [boardKey, getBoardCallback]);
  const isAdmin = HasSystemAuthority(SystemAuthority.Admin, user);
  return (
    <>
      <div>
        <h1 style={{ margin: '0.3rem' }}>{board.boardName}</h1>
        <div className={classes.refreshArea}>
          <Tooltip title="Archives">
            <IconButton
              className={classes.refreshButton}
              onClick={() => {
                dispatch(routerActions.push(`/${boardKey}/archive`));
              }}
            >
              <ArchiveIcon />
            </IconButton>
          </Tooltip>
          <Tooltip
            title="Setting"
            style={{
              display: logined && isAdmin ? 'initial' : 'none'
            }}
          >
            <IconButton
              className={classes.refreshButton}
              onClick={() =>
                dispatch(routerActions.push(`/${boardKey}/setting`))
              }
            >
              <SettingsIcon />
            </IconButton>
          </Tooltip>
          <Tooltip title="Refresh">
            <IconButton
              onClick={() => {
                getBoard(boardKey);
              }}
              className={classes.refreshButton}
            >
              <RefreshIcon />
            </IconButton>
          </Tooltip>

          <Box className={classes.lastRefreshedStatus}>
            Refreshed:{new Date(lastRefreshed).toLocaleString('ja-jp')}
          </Box>
        </div>
      </div>
      <ThreadListTable
        board={board}
        boardKey={props.match.params.boardKey}
        hasAuthority={isAdmin}
        forceRefreshCallback={() => getBoardCallback(boardKey)}
      />
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
            className={classes.addIcon}
            onClick={() => setIsCreating(true)}
          >
            <AddIcon />
          </Fab>
        </Tooltip>
      </div>
      <CreateThreadDialog
        setCreating={setIsCreating}
        creating={isCreating}
        boardKey={board.boardKey}
        onCreatingCallback={() => getBoardCallback(board.boardKey)}
      />
    </>
  );
};
