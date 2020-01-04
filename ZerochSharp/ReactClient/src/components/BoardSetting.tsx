import React, { useState } from 'react';
import {
  Theme,
  makeStyles,
  ExpansionPanel,
  ExpansionPanelSummary,
  Typography,
  ExpansionPanelDetails,
  TextField,
  Button,
  useTheme
} from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { useSelector, useDispatch } from 'react-redux';
import { AppState } from '../store';
import { RouteComponentProps } from 'react-router-dom';
import Axios from 'axios';
import { boardListActions } from '../actions/boardListActions';

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
  const dispatch = useDispatch();

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
            </form>
          </ExpansionPanelDetails>
        </ExpansionPanel>
        <ExpansionPanel>
          <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
            <Typography>Plugins</Typography>
          </ExpansionPanelSummary>
          <ExpansionPanelDetails></ExpansionPanelDetails>
        </ExpansionPanel>
      </div>
    </>
  );
};
