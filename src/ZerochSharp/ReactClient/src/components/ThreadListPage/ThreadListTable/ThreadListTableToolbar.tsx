import React, { useState } from 'react';
import {
  Toolbar,
  Typography,
  Tooltip,
  IconButton,
  makeStyles,
  createStyles,
  Theme,
  lighten,
  Dialog,
  DialogTitle,
  DialogActions,
  Button,
  DialogContent
} from '@material-ui/core';
import PauseIcon from '@material-ui/icons/Pause';
import DeleteIcon from '@material-ui/icons/Delete';
import clsx from 'clsx';
import { Thread } from '../../../models/thread';
import axios from 'axios';

interface ConfirmDialogProps {
  open: boolean;
  id: string;
  title: string;
  content: string;
  yesClickedCallback: () => void;
  setOpenCallback: (value: React.SetStateAction<boolean>) => void;
}

const ConfirmDialog = (props: ConfirmDialogProps) => {
  return (
    <Dialog open={props.open} id={props.id}>
      <DialogTitle>{props.title}</DialogTitle>
      <DialogContent>{props.content}</DialogContent>
      <DialogActions>
        <Button onClick={() => props.yesClickedCallback()}>Yes</Button>
        <Button onClick={() => props.setOpenCallback(false)}>Cancel</Button>
      </DialogActions>
    </Dialog>
  );
};

interface Props {
  selectedNum: number;
  selectedThreads: Thread[];
  forceRefreshCallback: () => void;
  setSelectedToEmpty: () => void;
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    highlight:
      theme.palette.type === 'light'
        ? {
            color: theme.palette.secondary.main,
            backgroundColor: lighten(theme.palette.secondary.light, 0.85)
          }
        : {
            color: theme.palette.text.primary,
            backgroundColor: theme.palette.secondary.dark
          },
    toolbarRoot: {
      paddingLeft: theme.spacing(2),
      paddingRight: theme.spacing(1)
    },
    title: {
      flex: '1 1 100%'
    }
  })
);

export const ThreadListTableToolbar = (props: Props) => {
  const classes = useStyles();
  const [threadStopDialogOpen, setThreadStopDialogOpen] = useState(false);
  const [threadArchiveDialogOpen, setThreadArchiveDialogOpen] = useState(false);
  const affectThreads = (threads: Thread[], type: string) => {
    const promises: Promise<any>[] = [];
    threads.forEach(x => {
      promises.push(
        axios.patch(`/api/boards/${x.boardKey}/${x.threadId}`, { [type]: true })
      );
    });
    Promise.all(promises)
      .then(x => {
        props.forceRefreshCallback();
        props.setSelectedToEmpty();
      })
      .catch(e => console.error(e));
  };
  const stopThreads = (threads: Thread[]) => {
    affectThreads(threads, 'stopped');
  };
  const archiveThreads = (threads: Thread[]) => {
    affectThreads(threads, 'archived');
  };
  return (
    <Toolbar
      className={clsx(classes.toolbarRoot, {
        [classes.highlight]: props.selectedNum > 0
      })}
    >
      <Typography className={classes.title} color="inherit" variant="subtitle1">
        {props.selectedNum} selected
      </Typography>
      <Tooltip title="Stop" onClick={() => setThreadStopDialogOpen(true)}>
        <IconButton aria-label="stop">
          <PauseIcon />
        </IconButton>
      </Tooltip>
      <Tooltip title="Archive" onClick={() => setThreadArchiveDialogOpen(true)}>
        <IconButton aria-label="archive">
          <DeleteIcon />
        </IconButton>
      </Tooltip>
      <ConfirmDialog
        open={threadArchiveDialogOpen}
        id="archive-threds-dialog"
        title="Archive Threads"
        content={`Do you want to archive ${props.selectedNum} threads?`}
        yesClickedCallback={() => archiveThreads(props.selectedThreads)}
        setOpenCallback={setThreadArchiveDialogOpen}
      />
      <ConfirmDialog
        open={threadStopDialogOpen}
        id="stop-threads-dialog"
        title="Stop Threads"
        content={`Do you want to stop ${props.selectedNum} threads?`}
        yesClickedCallback={() => stopThreads(props.selectedThreads)}
        setOpenCallback={setThreadStopDialogOpen}
      />
    </Toolbar>
  );
};
