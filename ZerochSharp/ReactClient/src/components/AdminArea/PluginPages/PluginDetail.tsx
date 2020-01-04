import React, { useState, useEffect } from 'react';
import { Plugin } from '../../../models/plugin';
import { RouteComponentProps } from 'react-router-dom';
import Axios from 'axios';
import { useSelector } from 'react-redux';
import { AppState } from '../../../store';
import {
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Checkbox,
  Button
} from '@material-ui/core';
import { Board } from '../../../models/board';

interface Props extends RouteComponentProps<{ pluginName: string }> {
  plugin?: Plugin;
}

export const PluginDetail = (props: Props) => {
  const [plugin, setPlugin] = useState(props.plugin);
  const [checkedBoards, setCheckedBoards] = useState(
    [] as string[] | undefined
  );
  const [isChanged, setIsChanged] = useState(false);
  const [initialActivateds, setInitialActivateds] = useState([] as string[]);

  const boardsState = useSelector(
    (appState: AppState) => appState.boardListState
  );

  useEffect(() => {
    const getPluginInfo = () => {
      Axios.get<Plugin[]>('/api/plugin')
        .then(x => {
          const item = x.data.find(
            y => y.pluginPath === props.match.params.pluginName
          );
          setPlugin(item);
          if (item) {
            setCheckedBoards(item.activatedBoards);
            setInitialActivateds(item.activatedBoards);
          }
        })
        .catch(x => console.error(x));
    };
    getPluginInfo();
  }, [props.match.params.pluginName]);
  useEffect(() => {
    const checkChanged = () => {
      const interSec: string[] = [];
      if (initialActivateds) {
        initialActivateds.forEach(x => {
          if (checkedBoards?.find(y => y === x)) {
            interSec.push(x);
          }
        });
        if (
          interSec.length !== checkedBoards?.length ||
          interSec.length !== initialActivateds.length
        ) {
          setIsChanged(true);
        } else {
          setIsChanged(false);
        }
      }
    };
    checkChanged();
  }, [checkedBoards, initialActivateds]);

  const applyChanges = () => {
    Axios.patch<{ activatedBoards: string[] }>(
      `/api/plugin/${props.match.params.pluginName}`,
      { activatedBoards: checkedBoards }
    )
      .then(x => {
        setInitialActivateds(checkedBoards ?? []);
      })
      .catch(x => console.error(x));
  };

  if (plugin && checkedBoards) {
    return (
      <>
        <h1>Plugin Name: {plugin.pluginName}</h1>
        <h3>Activated Boards</h3>
        <List>
          {boardsState.boards.map((board: Board) => {
            return (
              <ListItem>
                <ListItemIcon>
                  <Checkbox
                    checked={(() => {
                      const item = checkedBoards.find(
                        x => x === board.boardKey
                      );
                      return item !== undefined;
                    })()}
                    onChange={e => {
                      const value = e.target.checked;
                      if (!value) {
                        setCheckedBoards(
                          checkedBoards.filter(x => x !== board.boardKey)
                        );
                      } else {
                        setCheckedBoards([...checkedBoards, board.boardKey]);
                      }
                    }}
                  />
                </ListItemIcon>
                <ListItemText primary={board.boardName} />
              </ListItem>
            );
          })}
        </List>
        <Button disabled={!isChanged} onClick={applyChanges}>
          Apply Changes
        </Button>
      </>
    );
  } else {
    return <></>;
  }
};
