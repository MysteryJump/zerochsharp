import React, { useCallback, useState, useEffect } from 'react';
import { AppState } from '../store';
import { useSelector } from 'react-redux';
import { RouteComponentProps } from 'react-router-dom';
import { Button } from '@material-ui/core';
import Axios from 'axios';
import { TreeTextField, ParentItem } from './general/TreeTextField';

interface OwnProps {
  boardKey: string;
}

type Props = OwnProps & RouteComponentProps<{ pluginPath: string }>;

const initialTree: ParentItem = {
  key: 'parentItem22242',
  name: 'ParentItem',
  children: [
    {
      name: 'unko',
      value: ['2', 'true', '5', '4', '42', '4', '3'],
      key: 'defwgeg'
    },
    {
      name: 'unkoko',
      value: 'fwfger',
      key: 'fwgehre'
    }
  ]
};

interface PluginSettings {
  [key: string]: string | number | boolean | string[];
}

export const BoardPluginSetting = (props: Props) => {
  const router = useSelector((appState: AppState) => appState.router);

  const [changedTree, setChangedTree] = useState(initialTree);

  const getBoardPluginCallback = useCallback(() => {
    let boardKey = props.boardKey;
    if (!boardKey) {
      const pathName = router.location.pathname;
      boardKey = pathName.slice(1, pathName.slice(1, 10000).search('/') + 1);
    }
    Axios.get<PluginSettings>(
      `/api/plugin/${props.match.params.pluginPath}/${boardKey}/`
    )
      .then(x => {
        const tree: ParentItem = {
          key: boardKey + '-plugin-tree',
          name: 'Plugin Setting Tree',
          children: []
        };
        for (let key in x.data) {
          const item = x.data[key];
          if (typeof item === 'object') {
            tree.children.push({
              name: key,
              value: item,
              key: key + '-setting'
            });
          } else {
            tree.children.push({
              name: key,
              value: item,
              key: key + '-setting'
            });
          }
        }
        setChangedTree(tree);
      })
      .catch(e => console.error(e));
  }, [props.match.params.pluginPath, props.boardKey, router]);

  const sendBoardPluginSetting = () => {
    const data: PluginSettings = {};
    let boardKey = props.boardKey;
    if (!boardKey) {
      const pathName = router.location.pathname;
      boardKey = pathName.slice(1, pathName.slice(1, 10000).search('/') + 1);
    }
    changedTree.children.forEach(x => {
      data[x.name] = x.value;
    });
    Axios.post(
      `/api/plugin/${props.match.params.pluginPath}/${boardKey}/`,
      data
    )
      .then(x => {})
      .catch(e => console.error(e));
  };

  useEffect(() => {
    getBoardPluginCallback();
  }, [getBoardPluginCallback]);

  return (
    <>
      <h1>Plugin Setting: {props.match.params.pluginPath}</h1>
      {changedTree === initialTree ? (
        <></>
      ) : (
        <TreeTextField item={changedTree} />
      )}
      <Button
        onClick={() => {
          sendBoardPluginSetting();
        }}
      >
        Save
      </Button>
    </>
  );
};
