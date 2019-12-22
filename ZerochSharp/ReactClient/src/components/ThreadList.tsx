import React, { useState, useEffect } from 'react';
import { ThreadListState } from '../states/threadListState';
import Axios from 'axios';
import {
  TableHead,
  TableRow,
  Paper,
  Table,
  TableCell,
  TableBody,
  Button,
  makeStyles,
  Theme,
  createStyles,
  useTheme,
  Box,
  IconButton,
  Tooltip,
  Fab,
  Dialog,
  DialogTitle,
  DialogActions,
  DialogContent,
  TextField
} from '@material-ui/core';
import RefreshIcon from '@material-ui/icons/Refresh';
import AddIcon from '@material-ui/icons/Add';
import { RouterState } from 'connected-react-router';
import { Link, RouteComponentProps } from 'react-router-dom';
import { ThreadListActions } from '../containers/ThreadListContainer';

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

interface Thread {
  title: string;
  threadId: number;
  responseCount: number;
  modified: string;
  datKey?: number;
  created: string;
  author: string;
  influence: number;
}

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

type Props = OwnProps &
  RouterState &
  RouteComponentProps<{ boardKey: string }> &
  ThreadListActions;

export const ThreadList = (props: Props) => {
  const classes = useStyles();
  const theme = useTheme();

  const [board, setBoard] = useState(initialBoardState);
  const [lastRefreshed, setLastRefreshed] = useState(Date.now());
  const [isCreating, setIsCreating] = useState(false);

  const [creatingName, setCreatingName] = useState('');
  const [creatingMail, setCreatingMail] = useState('');
  const [creatingBody, setCreatingBody] = useState('');
  const [creatingTitle, setCreatingTitle] = useState('')

  const getBoard = (boardKey: string) => {
    Axios.get<BoardState>(`/api/boards/${boardKey}`)
      .then(x => {
        setBoard(x.data);
        props.setCurrentName(x.data.boardName);
      })
      .catch(x => {
        console.error(x);
      });
    setLastRefreshed(Date.now());
  };
  useEffect(() => {
    getBoard(props.match.params.boardKey);
  }, [, props.location.pathname]);

  const sendThread = () => {
    const thread = {
      title: creatingTitle,
      response : {
        name: creatingName,
        mail: creatingMail,
        body: creatingBody
      }
    };
    Axios.post(`/api/boards/${board.boardKey}`, thread)
      .then(x => {
        setCreatingBody('');
        setCreatingMail('');
        setCreatingName('');
        setCreatingTitle('');
        setIsCreating(false);
      })
      .catch(x => {
        console.error(x);
      });
  };

  return (
    <>
      <div className={classes.refreshArea}>
        <Tooltip title="Refresh">
          <IconButton
            onClick={() => {
              getBoard(props.match.params.boardKey);
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
      <Dialog
        open={isCreating}
        onClose={() => setIsCreating(false)}
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
          <Button onClick={() => {sendThread()}} color="primary" disabled={creatingBody.length === 0 || creatingTitle.length === 0}>
            Submit
          </Button>
          <Button onClick={() => setIsCreating(false)} color="primary">
            Cancel
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};