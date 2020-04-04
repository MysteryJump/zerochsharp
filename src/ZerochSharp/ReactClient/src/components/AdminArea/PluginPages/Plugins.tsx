import React, { useEffect, useState } from 'react';
import Axios from 'axios';
import { PluginCard } from './PluginCard';
import { Plugin } from '../../../models/plugin';
import {
  makeStyles,
  createStyles,
  Tooltip,
  Fab,
  Theme
} from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';
import { useHistory } from 'react-router-dom';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    addIcon: {
      position: 'fixed',
      bottom: theme.spacing(2),
      right: theme.spacing(2)
    }
  })
);

const initialPlugin: Plugin[] = [];

export const Plugins = () => {
  const classes = useStyles();

  const [plugins, setPlugins] = useState(initialPlugin);
  const history = useHistory();
  useEffect(() => {
    Axios.get<Plugin[]>('/api/plugin').then(x => {
      setPlugins(x.data);
    });
  }, []);
  const pluginList = (
    <>
      <h1>Plugin List</h1>
      <div>
        {plugins.map(x => (
          <PluginCard {...x} key={x.pluginPath} />
        ))}
      </div>
      <div>
        <Tooltip
          title="Add Plugin"
          placement="top"
          TransitionProps={{ timeout: 0 }}
        >
          <Fab
            size="medium"
            color="primary"
            aria-label="create"
            className={classes.addIcon}
            onClick={() => history.push('/admin/plugin/add')}
          >
            <AddIcon />
          </Fab>
        </Tooltip>
      </div>
    </>
  );
  return <>{pluginList}</>;
};
