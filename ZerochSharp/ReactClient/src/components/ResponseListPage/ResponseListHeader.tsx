import React from 'react';
import {
  Chip,
  IconButton,
  Tooltip,
  Box,
  makeStyles,
  createStyles,
  Theme
} from '@material-ui/core';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import RefreshIcon from '@material-ui/icons/Refresh';
import { useDispatch } from 'react-redux';
import { routerActions } from 'connected-react-router';

interface Props {
  archived: boolean;
  boardKey: string;
  threadName: string;
  threadId: string;
  lastRefreshed: number;
  getThreadCallback: (boardKey: string, threadId: string) => void;
}

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
    }
  })
);

export const ResponseListHeader = (props: Props) => {
  const dispatch = useDispatch();
  const push = (path: string) => dispatch(routerActions.push(path));
  const classes = useStyles();
  return (
    <>
      <div>
        <h1 style={{ margin: '0.3rem' }}>
          {props.threadName}{' '}
          {props.archived ? <Chip label="Archived" /> : <></>}
        </h1>
        <Tooltip title="Back to thread list">
          <IconButton
            onClick={() => {
              push('/' + props.boardKey);
            }}
          >
            <ArrowBackIcon />
          </IconButton>
        </Tooltip>

        <div className={classes.refreshArea}>
          <Tooltip title="Refresh">
            <IconButton
              onClick={() => {
                props.getThreadCallback(props.boardKey, props.threadId);
              }}
              className={classes.refreshButton}
            >
              <RefreshIcon />
            </IconButton>
          </Tooltip>
          <Box className={classes.lastRefreshedStatus}>
            Last Refreshed:{' '}
            {new Date(props.lastRefreshed).toLocaleString('ja-jp')}
          </Box>
        </div>
      </div>
    </>
  );
};
