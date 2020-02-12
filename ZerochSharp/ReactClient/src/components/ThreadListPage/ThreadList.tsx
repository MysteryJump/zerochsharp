import React, { useState, useEffect, useCallback } from 'react';
import Axios from 'axios';
import {
  TableHead,
  TableRow,
  Paper,
  Table,
  TableCell,
  TableBody,
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
import { Link, RouteComponentProps } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { HasSystemAuthority, SystemAuthority } from '../../models/user';
import { AppState } from '../../store';
import { mainActions } from '../../actions/mainActions';
import { Thread } from '../../models/thread';
import { CreateThreadDialog } from './CreateThreadDialog';
import { routerActions } from 'connected-react-router';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    extendDetail: {
      [theme.breakpoints.down('sm')]: {
        display: 'none'
      }
    },
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
    threadListArea: {
      clear: 'both'
    },
    addIcon: {
      position: 'fixed',
      bottom: theme.spacing(2),
      right: theme.spacing(2)
    }
  })
);

interface OwnProps {}

interface BoardState {
  boardKey: string;
  boardName: string;
  child: Thread[];
}

const initialBoardState: BoardState = {
  boardKey: '',
  boardName: '',
  child: []
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
          <Tooltip
            title="Setting"
            style={{
              display:
                logined &&
                isAdmin
                  ? 'initial'
                  : 'none'
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
              <RefreshIcon></RefreshIcon>
            </IconButton>
          </Tooltip>
          <Box className={classes.lastRefreshedStatus}>
            Refreshed:{new Date(lastRefreshed).toLocaleString('ja-jp')}
          </Box>
        </div>
      </div>
      <Paper className={classes.threadListArea}>
        <Table aria-label="thread list tabel">
          <TableHead>
            <TableRow>
              <TableCell>#</TableCell>
              <TableCell>Title</TableCell>
              <TableCell className={classes.extendDetail}>Created</TableCell>
              <TableCell className={classes.extendDetail}>Modified</TableCell>
              <TableCell>Count</TableCell>
              <TableCell className={classes.extendDetail}>Influence</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {board.child.map((thread, index) => (
              <TableRow>
                <TableCell>{index + 1}</TableCell>
                <TableCell>
                  <Link
                    to={`/${
                      props.match.params.boardKey
                    }/${thread.threadId.toString()}`}
                  >
                    {thread.title}
                  </Link>
                </TableCell>
                <TableCell className={classes.extendDetail}>
                  {thread.created}
                </TableCell>
                <TableCell className={classes.extendDetail}>
                  {thread.modified}
                </TableCell>
                <TableCell>{thread.responseCount}</TableCell>
                <TableCell className={classes.extendDetail}>
                  {Math.round(thread.influence * 1000) / 1000}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
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
      />
    </>
  );
};
