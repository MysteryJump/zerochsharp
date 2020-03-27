import React, { useState, useEffect } from 'react';
import {
  TableCell,
  Paper,
  TableRow,
  TableHead,
  TableBody,
  Table,
  Theme,
  makeStyles,
  createStyles,
  TableContainer,
  Checkbox,
  Chip
} from '@material-ui/core';
import { Link } from 'react-router-dom';
import { BoardState } from '../BoardState';
import { ThreadListTableToolbar } from './ThreadListTableToolbar';
import { Thread } from '../../../models/thread';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    extendDetail: {
      [theme.breakpoints.down('sm')]: {
        display: 'none'
      }
    },
    threadListArea: {
      clear: 'both'
    }
  })
);

interface Props {
  board: BoardState;
  boardKey: string;
  hasAuthority: boolean;
  forceRefreshCallback: () => void;
}

export const ThreadListTable = (props: Props) => {
  const classes = useStyles();
  const [selected, setSelected] = useState<number[]>([]);
  const isSelected = (index: number) => selected.indexOf(index) >= 0;
  const onItemCheckboxClicked = (index: number) => {
    if (!isSelected(index)) {
      setSelected([...selected, index]);
    } else {
      setSelected(selected.filter(x => x !== index));
    }
  };
  const onSetHeadCheckbox = (value: boolean) => {
    if (value) {
      setSelected([...Array.from(Array(props.board.children.length).keys())]);
    } else {
      setSelected([]);
    }
  };

  const [selectedThread, setSelectedThread] = useState<Thread[]>([]);
  useEffect(() => {
    if (selected.length > 0 && props.board && props.board.children.length > 0) {
      setSelectedThread(selected.map(x => props.board.children[x]));
    }
  }, [selected, props.board]);
  const setSelectedToEmpty = () => {
    setSelected([]);
  };

  return (
    <Paper className={classes.threadListArea}>
      {selected.length > 0 ? (
        <ThreadListTableToolbar
          selectedNum={selected.length}
          selectedThreads={selectedThread}
          forceRefreshCallback={props.forceRefreshCallback}
          setSelectedToEmpty={setSelectedToEmpty}
        />
      ) : (
        <></>
      )}
      <TableContainer>
        <Table aria-label="thread list tabel">
          <TableHead>
            <TableRow>
              {props.hasAuthority ? (
                <TableCell padding="checkbox">
                  <Checkbox
                    indeterminate={
                      selected.length > 0 &&
                      selected.length < props.board.children.length
                    }
                    checked={
                      props.board.children.length > 0 &&
                      selected.length === props.board.children.length
                    }
                    onChange={(_event, value: boolean) => {
                      onSetHeadCheckbox(value);
                    }}
                  />
                </TableCell>
              ) : (
                <></>
              )}
              <TableCell>#</TableCell>
              <TableCell>Title</TableCell>
              <TableCell className={classes.extendDetail}>Created</TableCell>
              <TableCell className={classes.extendDetail}>Modified</TableCell>
              <TableCell>Count</TableCell>
              <TableCell className={classes.extendDetail}>Influence</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {props.board.children.map((thread, index) => (
              <TableRow>
                {props.hasAuthority ? (
                  <TableCell padding="checkbox">
                    <Checkbox
                      onChange={() => onItemCheckboxClicked(index)}
                      checked={isSelected(index)}
                    />
                  </TableCell>
                ) : (
                  <></>
                )}
                <TableCell>{index + 1}</TableCell>
                <TableCell>
                  <Link to={`/${props.boardKey}/${thread.threadId.toString()}`}>
                    {thread.title}
                  </Link>
                  {'  '}
                  {thread.stopped ? (
                    <Chip color="secondary" size="small" label="Stopped" />
                  ) : (
                    <></>
                  )}
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
      </TableContainer>
    </Paper>
  );
};
