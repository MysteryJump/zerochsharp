import React, { useEffect, useState, useCallback } from 'react';
import { makeStyles, Theme, createStyles } from '@material-ui/core';
import { RouteComponentProps } from 'react-router-dom';
import Axios, { AxiosResponse } from 'axios';
import { useSelector, useDispatch } from 'react-redux';
import { AppState } from '../../store';
import { mainActions } from '../../actions/mainActions';
import { Response } from '../../models/response';
import { Thread } from '../../models/thread';
import { ResponseCard } from './ResponseCard';
import { CreateResponseArea } from './CreateResponseArea';
import { HasBoardSettingAuthority } from '../../models/user';
import { RemoveResponseDialog } from './RemoveResponseDialog';
import { ResponseListHeader } from './ResponseListHeader';
import { RemoveResponseFab } from './RemoveResponseFab';
import { CreateResponseFab } from './CreateResponseFab';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    responseListArea: {
      clear: 'both'
    }
  })
);

const initialResponses: Response[] = [];

export const ResponseList = (
  props: RouteComponentProps<{ threadId: string; boardKey: string }>
) => {
  const classes = useStyles();

  const [responses, setResponses] = useState(initialResponses);
  const [isCreating, setIsCreating] = useState(false);
  const [lastRefreshed, setLastRefreshed] = useState(Date.now());
  const [threadName, setThreadName] = useState('');
  const [boardDefaultName, setBoardDefaultName] = useState('');
  const [checkedResponses, setCheckedResponses] = useState<number[]>([]);
  const [archived, setArchived] = useState(false);
  const [stopped, setStopped] = useState(false);
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

  const responseListDisplayStyle = {
    marginBottom: isCreating ? '11rem' : '0rem'
  };

  const boardKey = props.match.params.boardKey;
  const threadId = props.match.params.threadId;
  const isAdmin = HasBoardSettingAuthority(boardKey, sessionState.user);

  const getThread = (boardKey: string, threadId: string) => {
    const apiUrl = `/api/boards/${boardKey}/${threadId}`;
    Axios.get<Thread>(apiUrl)
      .then((x: AxiosResponse<Thread>) => {
        if (x.data.responses) {
          setResponses(x.data.responses);
        }
        setThreadName(x.data.title);
        setLastRefreshed(Date.now());
        setArchived(x.data.archived);
        setStopped(x.data.stopped);
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
  const getThreadCallback = useCallback(getThread, []);
  useEffect(() => {
    getThreadCallback(boardKey, threadId);
  }, [boardKey, threadId, getThreadCallback]);
  return (
    <>
      <ResponseListHeader
        archived={archived}
        boardKey={boardKey}
        threadName={threadName}
        threadId={threadId}
        lastRefreshed={lastRefreshed}
        getThreadCallback={getThread}
      />
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
              user={sessionState.user}
              boardKey={boardKey}
            />
          );
        })}
      </div>
      <CreateResponseFab
        isCreating={isCreating}
        archived={archived}
        stopped={stopped}
        setIsCreatingCallback={setIsCreating}
      />
      <RemoveResponseFab
        isAdmin={isAdmin}
        checkedLength={checkedResponses.length}
        setRemoveResponseDialogOpenCallback={setRemoveResponseDialogOpen}
      />
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
