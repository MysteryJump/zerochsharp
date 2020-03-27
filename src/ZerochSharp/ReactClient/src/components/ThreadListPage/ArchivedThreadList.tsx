import React, { useState, useCallback, useEffect } from 'react';
import {
  Tooltip,
  IconButton,
  Paper,
  TablePagination,
  TableContainer,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell
} from '@material-ui/core';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import { useDispatch } from 'react-redux';
import { routerActions } from 'connected-react-router';
import { RouteComponentProps, Link } from 'react-router-dom';
import { BoardState } from './BoardState';
import Axios from 'axios';
import { Thread } from '../../models/thread';

export const ArchivedThreadList = (
  props: RouteComponentProps<{ boardKey: string }>
) => {
  const page = parseInt(
    props.location.search.match(/\?page=(\d+)/)?.[1] ?? '1'
  );
  const boardKey = props.match.params.boardKey;

  const dispatch = useDispatch();
  const push = (path: string) => dispatch(routerActions.push(path));

  const [threads, setThreads] = useState([] as Thread[]);
  const [count, setCount] = useState(1110);
  const [boardName, setBoardName] = useState('');

  const getArchiveThreadsCallback = useCallback(() => {
    Axios.get<BoardState>(`/api/boards/${boardKey}/archives?page=${page}`)
      .then(x => {
        setThreads(x.data.children);
        setCount(x.data.childrenCount ?? 0);
        setBoardName(x.data.boardName);
      })
      .catch(e => console.error(e));
  }, [boardKey, page]);
  const onChangePageCallback = useCallback(
    (_event, newpage: number) => {
      push('/' + boardKey + '/archive?page=' + (newpage + 1));
    },
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [boardKey, page]
  );
  useEffect(() => {
    getArchiveThreadsCallback();
  }, [getArchiveThreadsCallback]);
  return (
    <>
      <div>
        <h1 style={{ margin: '0.3rem' }}>Archives of {boardName}</h1>
        <Tooltip title="Back to thread list">
          <IconButton
            onClick={() => {
              push('/' + boardKey);
            }}
          >
            <ArrowBackIcon />
          </IconButton>
        </Tooltip>
      </div>
      <Paper>
        <TableContainer>
          <Table aria-label="archived thread list tabel">
            <TableHead>
              <TableRow>
                <TableCell>Thread Title</TableCell>
                <TableCell>Response Count</TableCell>
                <TableCell>Created</TableCell>
                <TableCell>Modified</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {threads.map(x => {
                return (
                  <TableRow>
                    <TableCell>
                      <Link to={`/${boardKey}/${x.threadId.toString()}`}>
                        {x.title}
                      </Link>
                    </TableCell>
                    <TableCell>{x.responseCount}</TableCell>
                    <TableCell>{x.created}</TableCell>
                    <TableCell>{x.modified}</TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>
        <TablePagination
          rowsPerPage={10}
          rowsPerPageOptions={[10]}
          component="div"
          page={page - 1}
          count={count}
          onChangePage={onChangePageCallback}
        />
      </Paper>
    </>
  );
};
