import React, { useState, useCallback, useEffect } from 'react';
import {
  Theme,
  makeStyles,
  ExpansionPanel,
  ExpansionPanelSummary,
  Typography,
  ExpansionPanelDetails,
  TextField,
  Button,
  useTheme,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  ExpansionPanelActions
} from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { useSelector, useDispatch } from 'react-redux';
import { AppState } from '../store';
import { RouteComponentProps } from 'react-router-dom';
import Axios from 'axios';
import { boardListActions } from '../actions/boardListActions';
import { routerActions } from 'connected-react-router';
import { Plugin } from '../models/plugin';
import { TreeTextField, ParentItem } from './general/TreeTextField';

interface Props {}

const useStyles = makeStyles((theme: Theme) => ({
  textField: {
    margin: theme.spacing(1),
    width: theme.spacing(30)
  }
}));

export const BoardSetting = (
  props: RouteComponentProps<{ boardKey: string }>
) => {
  const theme = useTheme();
  const classes = useStyles();

  const boardsState = useSelector(
    (appState: AppState) => appState.boardListState
  );
  const board = boardsState.boards.find(
    x => x.boardKey === props.match.params.boardKey
  );

  const [boardName, setBoardName] = useState(board?.boardName);
  const [boardDefaultName, setBoardDefaultName] = useState(
    board?.boardDefaultName
  );
  const [boardSubTitle, setBoardSubtitle] = useState(board?.boardSubTitle);
  const [boardKeyConfirm, setBoardKeyConfirm] = useState('');
  const [removeDialogOpen, setRemoveDialogOpen] = useState(false);
  const [boardPlugins, setBoardPlugins] = useState([] as Plugin[]);
  const dispatch = useDispatch();
  const [predicates, setPredicates] = useState(
    undefined as ParentItem | undefined
  );

  const postSetting = () => {
    Axios.post(`/api/boards/${board?.boardKey}/setting`, {
      boardKey: board?.boardKey,
      boardDefaultName: boardDefaultName,
      boardName: boardName,
      boardSubTitle: boardSubTitle
    })
      .then(x => {
        dispatch(boardListActions.fetchBoardList());
      })
      .catch(x => console.error(x));
  };

  const removeBoard = () => {
    Axios.delete(`/api/boards/${board?.boardKey}`)
      .then(x => {
        dispatch(boardListActions.fetchBoardList());
        dispatch(routerActions.push('/'));
      })
      .catch(x => console.error(x));
  };

  const postPredicateSettings = () => {
    // for Chromium edge
    const predsTmp: string[] = predicates?.children[0].value as string[];
    Axios.post(`/api/boards/${board?.boardKey}/setting`, {
      autoArchivingPredicates: predsTmp
    })
      .then(x => {
        dispatch(boardListActions.fetchBoardList());
      })
      .catch(e => console.error(e));
  };

  const getPluginInfoCallback = useCallback(() => {
    if (board) {
      Axios.get<Plugin[]>(`/api/plugin/`)
        .then(x => {
          const boardPlugins = x.data.filter(
            x => x.activatedBoards.findIndex(y => y === board.boardKey) >= 0
          );
          setBoardPlugins(boardPlugins);
        })
        .catch(y => console.error(y));
    }
  }, [board]);

  useEffect(() => getPluginInfoCallback(), [getPluginInfoCallback]);
  useEffect(() => {
    dispatch(boardListActions.fetchBoardList());
  }, [dispatch]);
  useEffect(() => {
    if (board && board.autoArchivingPredicates) {
      const preds: ParentItem = {
        name: 'Predicates',
        key: 'predicates-parent-item-key',
        children: [
          {
            name: 'Predicates',
            value: board.autoArchivingPredicates,
            key: 'predicated-key-' + board.boardKey
          }
        ]
      };
      setPredicates(preds);
    }
  }, [board]);

  return (
    <>
      <h1>Board Setting</h1>
      <div>
        <ExpansionPanel>
          <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
            <Typography>General</Typography>
          </ExpansionPanelSummary>
          <ExpansionPanelDetails>
            <form>
              <div>
                <TextField
                  label="Board Key"
                  className={classes.textField}
                  disabled
                  defaultValue={board?.boardKey}
                />
                <TextField
                  label="Board Name"
                  className={classes.textField}
                  defaultValue={board?.boardName}
                  value={boardName}
                  onChange={e => setBoardName(e.target.value)}
                />
              </div>
              <div>
                <TextField
                  label="Board Default Name"
                  className={classes.textField}
                  defaultValue={board?.boardDefaultName}
                  style={{ width: theme.spacing(60) }}
                  value={boardDefaultName}
                  onChange={e => setBoardDefaultName(e.target.value)}
                />
              </div>
              <div>
                <TextField
                  label="Board Sub Title"
                  className={classes.textField}
                  defaultValue={board?.boardSubTitle}
                  style={{ width: theme.spacing(60) }}
                  value={boardSubTitle}
                  onChange={e => setBoardSubtitle(e.target.value)}
                />
              </div>
              <Button onClick={postSetting}>Save</Button>
              <Button
                variant="outlined"
                color="secondary"
                onClick={() => {
                  setRemoveDialogOpen(true);
                }}
              >
                Remove this Board
              </Button>
            </form>
          </ExpansionPanelDetails>
        </ExpansionPanel>
        <ExpansionPanel>
          <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
            <Typography>Plugins</Typography>
          </ExpansionPanelSummary>
          <ExpansionPanelDetails style={{ display: 'initial' }}>
            <List>
              {boardPlugins.map(x => {
                return (
                  <ListItem style={{ paddingLeft: '2rem' }}>
                    <ListItemText>
                      {x.pluginName} ({x.pluginPath})
                    </ListItemText>
                    <ListItemSecondaryAction>
                      <Button
                        onClick={() =>
                          dispatch(
                            routerActions.push(
                              `/${board?.boardKey}/setting/plugin/${x.pluginPath}/`
                            )
                          )
                        }
                      >
                        Setting
                      </Button>
                    </ListItemSecondaryAction>
                  </ListItem>
                );
              })}
            </List>
          </ExpansionPanelDetails>
        </ExpansionPanel>
        <ExpansionPanel>
          <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
            Auto Thread Archiving
          </ExpansionPanelSummary>
          <ExpansionPanelDetails>
            {/* <h4>Predications</h4> */}
            {predicates == null ? <></> : <TreeTextField item={predicates} />}
          </ExpansionPanelDetails>
          <ExpansionPanelActions>
            <Button
              onClick={() => {
                postPredicateSettings();
              }}
            >
              Save
            </Button>
          </ExpansionPanelActions>
        </ExpansionPanel>
      </div>
      <Dialog
        open={removeDialogOpen}
        onClose={() => setRemoveDialogOpen(false)}
      >
        <DialogTitle>Remove This Board</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Do you want to remove this board '{boardName}' permanently? This
            action cannot be undone, but associated threads and responses
            remain. If you want to continue this action, please input full name
            of this board key.
          </DialogContentText>
          <TextField
            value={boardKeyConfirm}
            onChange={e => setBoardKeyConfirm(e.target.value)}
            fullWidth
          />
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => {
              removeBoard();
              setRemoveDialogOpen(false);
            }}
            disabled={boardKeyConfirm !== board?.boardKey}
          >
            Submit
          </Button>
          <Button
            onClick={() => {
              setRemoveDialogOpen(false);
            }}
          >
            Cancel
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
