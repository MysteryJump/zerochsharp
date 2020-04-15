import React, { useState, useCallback, useEffect } from 'react';
import {
  ExpansionPanel,
  ExpansionPanelSummary,
  ExpansionPanelDetails,
  TextField,
  ExpansionPanelActions,
  Button,
} from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { Board } from '../../../models/board';
import axios from 'axios';

interface Props {
  board?: Board;
}

export const RestrictUsersPanel = (props: Props) => {
  const [restrictUsers, setRestrictUsers] = useState('');

  const getRestrictedUsers = useCallback(() => {
    axios
      .get<{ restrictedUsers: string }>(
        `/api/boards/${props.board?.boardKey}/restricted-users`
      )
      .then((x) => {
        setRestrictUsers(x.data.restrictedUsers);
      })
      .catch((e) => console.error(e));
  }, [props.board]);
  useEffect(() => {
    getRestrictedUsers();
  }, [getRestrictedUsers]);

  const putRestrictedUsers = useCallback(() => {
    axios
      .put(`/api/boards/${props.board?.boardKey}/restricted-users`, {
        restrictedUsers: restrictUsers,
      })
      .then((x) => {})
      .catch((e) => console.error(e));
  }, [props.board, restrictUsers]);

  return (
    <ExpansionPanel>
      <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
        Restricted Hosts
      </ExpansionPanelSummary>
      <ExpansionPanelDetails>
        <TextField
          fullWidth
          multiline
          rows={18}
          value={restrictUsers}
          onChange={(e) => setRestrictUsers(e.target.value)}
        />
      </ExpansionPanelDetails>
      <ExpansionPanelActions>
        <Button onClick={() => putRestrictedUsers()}>Save</Button>
      </ExpansionPanelActions>
    </ExpansionPanel>
  );
};
