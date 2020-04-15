import React, { useCallback, useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';
import {
  ExpansionPanelDetails,
  ExpansionPanel,
  Typography,
  List,
  ListItemText,
  ListItem,
  Button,
  ExpansionPanelSummary,
  ListItemSecondaryAction,
} from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { routerActions } from 'connected-react-router';
import { Board } from '../../../models/board';
import { Plugin } from '../../../models/plugin';
import Axios from 'axios';

interface Props {
  board?: Board;
}

export const PluginsPanel = (props: Props) => {
  const dispatch = useDispatch();
  const [boardPlugins, setBoardPlugins] = useState<Plugin[]>([]);

  const getPluginInfoCallback = useCallback(() => {
    if (props.board) {
      Axios.get<Plugin[]>(`/api/plugin/`)
        .then((x) => {
          const boardPlugins = x.data.filter(
            (x) =>
              x.activatedBoards.findIndex((y) => y === props.board?.boardKey) >=
              0
          );
          setBoardPlugins(boardPlugins);
        })
        .catch((y) => console.error(y));
    }
  }, [props.board]);

  useEffect(() => getPluginInfoCallback(), [getPluginInfoCallback]);

  return (
    <ExpansionPanel>
      <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
        <Typography>Plugins</Typography>
      </ExpansionPanelSummary>
      <ExpansionPanelDetails style={{ display: 'initial' }}>
        <List>
          {boardPlugins.map((x) => {
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
                          `/${props.board?.boardKey}/setting/plugin/${x.pluginPath}/`
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
  );
};
