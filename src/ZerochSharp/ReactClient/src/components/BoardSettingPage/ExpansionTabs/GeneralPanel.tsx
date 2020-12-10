import React, { useState } from 'react';
import {
  ExpansionPanel,
  ExpansionPanelSummary,
  Typography,
  ExpansionPanelDetails,
  TextField,
  Button,
  createStyles,
  makeStyles,
  Theme,
  useTheme,
} from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { Board } from '../../../models/board';
import { useDispatch } from 'react-redux';
import Axios from 'axios';
import { boardListActions } from '../../../actions/boardListActions';

const useStyles = makeStyles((theme: Theme) => createStyles({
  textField: {
    margin: theme.spacing(1),
    width: theme.spacing(30),
  },
}));

interface Props {
  board?: Board;
  setRemoveDialogOpen: (state: React.SetStateAction<boolean>) => void;
}

export const GeneralPanel = (props: Props) => {
  const classes = useStyles();
  const theme = useTheme();
  const dispatch = useDispatch();

  const [boardName, setBoardName] = useState(props.board?.boardName);
  const [boardDefaultName, setBoardDefaultName] = useState(
    props.board?.boardDefaultName
  );
  const [boardSubTitle, setBoardSubtitle] = useState(props.board?.boardSubTitle);

  
  const postSetting = () => {
    Axios.post(`/api/boards/${props.board?.boardKey}/setting`, {
      boardKey: props.board?.boardKey,
      boardDefaultName: boardDefaultName,
      boardName: boardName,
      boardSubTitle: boardSubTitle,
    })
      .then((x) => {
        dispatch(boardListActions.fetchBoardList());
      })
      .catch((x) => console.error(x));
  };

  return (
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
              defaultValue={props.board?.boardKey}
            />
            <TextField
              label="Board Name"
              className={classes.textField}
              defaultValue={props.board?.boardName}
              value={boardName}
              onChange={(e) => setBoardName(e.target.value)}
            />
          </div>
          <div>
            <TextField
              label="Board Default Name"
              className={classes.textField}
              defaultValue={props.board?.boardDefaultName}
              style={{ width: theme.spacing(60) }}
              value={boardDefaultName}
              onChange={(e) => setBoardDefaultName(e.target.value)}
            />
          </div>
          <div>
            <TextField
              label="Board Sub Title"
              className={classes.textField}
              defaultValue={props.board?.boardSubTitle}
              style={{ width: theme.spacing(60) }}
              value={boardSubTitle}
              onChange={(e) => setBoardSubtitle(e.target.value)}
            />
          </div>
          <Button onClick={postSetting}>Save</Button>
          <Button
            variant="outlined"
            color="secondary"
            onClick={() => {
              props.setRemoveDialogOpen(true);
            }}
          >
            Remove this Board
          </Button>
        </form>
      </ExpansionPanelDetails>
    </ExpansionPanel>
  );
};
